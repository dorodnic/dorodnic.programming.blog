#pragma once
#include <Windows.h>
#include <functional>

class ThreadPoolTimer
{
public:
	ThreadPoolTimer(PTP_TIMER timer, std::function<void()>* context);

	ThreadPoolTimer(const ThreadPoolTimer&) = delete;
	ThreadPoolTimer& operator=(const ThreadPoolTimer&) = delete;

	ThreadPoolTimer(ThreadPoolTimer&& other);
	ThreadPoolTimer& operator=(ThreadPoolTimer&& other);

	void Abort();

	~ThreadPoolTimer();
private:
	PTP_TIMER _timer;
	std::function<void()>* _context;
};