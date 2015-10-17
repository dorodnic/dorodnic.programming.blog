#include "stdafx.h"
#include "ThreadPoolWork.h"
#include <utility>

using namespace std;

ThreadPoolWork::ThreadPoolWork(PTP_WORK work)
	: _work(work)
{
}

ThreadPoolWork::ThreadPoolWork(ThreadPoolWork&& other)
	: _work(other._work)
{
	other._work = nullptr;
}

ThreadPoolWork& ThreadPoolWork::operator=(ThreadPoolWork&& other)
{
	_work = nullptr;
	swap(_work, other._work);
	return *this;
}

void ThreadPoolWork::Wait()
{
	if (_work != nullptr)
	{
		WaitForThreadpoolWorkCallbacks(_work, false);
		_work = nullptr;
	}
}