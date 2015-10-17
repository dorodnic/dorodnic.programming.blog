#pragma once
#include <Windows.h>
#include <functional>

struct Context
{
	std::function<void()> WorkItem;
	std::function<void()> OnTimeout;
};

class ThreadPoolWait
{
public:
	ThreadPoolWait(PTP_WAIT wait, Context* context);

	ThreadPoolWait(const ThreadPoolWait&) = delete;
	ThreadPoolWait& operator=(const ThreadPoolWait&) = delete;

	ThreadPoolWait(ThreadPoolWait&& other);
	ThreadPoolWait& operator=(ThreadPoolWait&& other);

	void Abort();

	~ThreadPoolWait();
private:
	PTP_WAIT _wait;
	Context* _context;
};