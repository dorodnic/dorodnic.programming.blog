#pragma once
#include <Windows.h>

class ThreadPoolWork
{
public:
	explicit ThreadPoolWork(PTP_WORK work);

	ThreadPoolWork(const ThreadPoolWork&) = delete;
	ThreadPoolWork& operator=(const ThreadPoolWork&) = delete;

	ThreadPoolWork(ThreadPoolWork&& other);
	ThreadPoolWork& operator=(ThreadPoolWork&& other);

	void Wait();
private:
	PTP_WORK _work;
};