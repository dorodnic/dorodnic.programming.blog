#include "stdafx.h"
#include "ThreadPool.h"


using namespace std;

HMODULE GetCurrentModule()
{
	DWORD flags = GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS;
	HMODULE hm = nullptr;
	GetModuleHandleEx(flags, reinterpret_cast<LPCTSTR>(GetCurrentModule), &hm);
	return hm;
}

FILETIME MsToFiletime(unsigned ms)
{
	FILETIME fileDueTime;
	ULARGE_INTEGER ulDueTime;
	ulDueTime.QuadPart = static_cast<ULONGLONG>(-static_cast<LONGLONG>(ms)* (1 * 10 * 1000));
	fileDueTime.dwHighDateTime = ulDueTime.HighPart;
	fileDueTime.dwLowDateTime = ulDueTime.LowPart;
	return fileDueTime;
}

void CALLBACK Callback(PTP_CALLBACK_INSTANCE instance, PVOID context, PTP_WORK work)
{
	auto functionPtr = static_cast<function<void()>*>(context);
	auto functionCopy = *functionPtr;
	delete functionPtr;
	CloseThreadpoolWork(work);

	functionCopy();
}

void CALLBACK TimeoutCallback(PTP_CALLBACK_INSTANCE instance, PVOID context, PTP_TIMER timer)
{
	auto functionPtr = static_cast<function<void()>*>(context);
	auto functionCopy = *functionPtr;

	functionCopy();
}

void CALLBACK WaitCallback(PTP_CALLBACK_INSTANCE instance, PVOID context, PTP_WAIT waitObject, TP_WAIT_RESULT waitResult)
{
	auto typedContext = static_cast<Context*>(context);
	auto workItem = typedContext->WorkItem;
	auto onTimeout = typedContext->WorkItem;
	
	if (waitResult == WAIT_OBJECT_0)
		workItem();
	else if (waitResult == WAIT_TIMEOUT)
		onTimeout();
	// TODO: Add case for ABANDONED if it's something you wish to support
}


ThreadPool::ThreadPool()
{
	_pool = CreateThreadpool(nullptr);

	InitializeThreadpoolEnvironment(&_environment);
	SetThreadpoolCallbackPool(&_environment, _pool);
	SetThreadpoolCallbackLibrary(&_environment, GetCurrentModule());

	_cleanupGroup = CreateThreadpoolCleanupGroup();

	SetThreadpoolCallbackCleanupGroup(&_environment, _cleanupGroup, nullptr);
}

ThreadPool::ThreadPool(unsigned minThreads, unsigned maxThreads)
	: ThreadPool()
{
	SetThreadpoolThreadMaximum(_pool, maxThreads);
	SetThreadpoolThreadMinimum(_pool, minThreads);
}

ThreadPool::ThreadPool(ThreadPool&& other)
	: _pool(other._pool), _environment(other._environment), _cleanupGroup(other._cleanupGroup)
{
	other._pool = nullptr;
}

ThreadPool& ThreadPool::operator=(ThreadPool&& other)
{
	Cleanup();
	swap(_pool, other._pool);
	swap(_environment, other._environment);
	swap(_cleanupGroup, other._cleanupGroup);
	return *this;
}

ThreadPoolWork ThreadPool::SubmitCallback(function<void()> workItem)
{
	auto context = new function<void()>(workItem);

	auto work = CreateThreadpoolWork(Callback, context, &_environment);
	if (work == nullptr)
	{
		delete context;
		throw exception("Failed to create thread-pool work!");
	}

	SubmitThreadpoolWork(work);

	return ThreadPoolWork(work);
}

ThreadPoolTimer ThreadPool::SubmitTimer(unsigned delayMs, unsigned periodMs, std::function<void()> workItem)
{
	auto context = new function<void()>(workItem);

	auto timer = CreateThreadpoolTimer(TimeoutCallback, context, &_environment);
	if (timer == nullptr)
	{
		delete context;
		throw exception("Failed to create thread-pool timer!");
	}

	auto fileDelayTime = MsToFiletime(delayMs);
	SetThreadpoolTimer(timer, &fileDelayTime, periodMs, 0);

	return ThreadPoolTimer(timer, context);
}

ThreadPoolWait ThreadPool::WaitForKernelObject(HANDLE handle, unsigned timeoutMs, std::function<void()> workItem, std::function<void()> onTimeout)
{
	auto context = new Context();
	context->WorkItem = workItem;
	context->OnTimeout = onTimeout;

	auto waitObject = CreateThreadpoolWait(WaitCallback, context, &_environment);
	if (waitObject == nullptr)
	{
		delete context;
		throw exception("Failed to create thread-pool wait!");
	}

	auto fileTimeoutTime = MsToFiletime(timeoutMs);
	SetThreadpoolWait(waitObject, handle, &fileTimeoutTime);

	return ThreadPoolWait(waitObject, context);
}

ThreadPoolWait ThreadPool::WaitForKernelObject(HANDLE handle, std::function<void()> workItem)
{
	return WaitForKernelObject(handle, INFINITE, workItem, [](){});
}

ThreadPool::~ThreadPool()
{
	Cleanup();
}

void ThreadPool::Cleanup()
{
	if (_pool != nullptr)
	{
		CloseThreadpoolCleanupGroupMembers(_cleanupGroup, false, nullptr);
		CloseThreadpoolCleanupGroup(_cleanupGroup);
		DestroyThreadpoolEnvironment(&_environment);
		CloseThreadpool(_pool);
		_pool = nullptr;
	}
}