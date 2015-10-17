#pragma once

#include "ThreadPoolTimer.h"
#include "ThreadPoolWait.h"
#include "ThreadPoolWork.h"

class ThreadPool
{
public:
	ThreadPool();
	ThreadPool(unsigned minThreads, unsigned maxThreads);

	ThreadPool(const ThreadPool&) = delete;
	ThreadPool& operator=(const ThreadPool&) = delete;

	ThreadPool(ThreadPool&& other);
	ThreadPool& operator=(ThreadPool&& other);

	ThreadPoolWork SubmitCallback(std::function<void()> workItem);

	ThreadPoolTimer SubmitTimer(unsigned delayMs, unsigned periodMs, std::function<void()> workItem);

	ThreadPoolWait WaitForKernelObject(HANDLE handle, unsigned timeoutMs, std::function<void()> workItem, std::function<void()> onTimeout);

	ThreadPoolWait WaitForKernelObject(HANDLE handle, std::function<void()> workItem);

	~ThreadPool();

private:
	void Cleanup();

	PTP_POOL _pool;
	_TP_CALLBACK_ENVIRON_V3 _environment;
	PTP_CLEANUP_GROUP _cleanupGroup;
};