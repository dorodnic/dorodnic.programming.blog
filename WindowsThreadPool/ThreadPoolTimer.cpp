#include "stdafx.h"
#include "ThreadPoolTimer.h"

using namespace std;

ThreadPoolTimer::ThreadPoolTimer(PTP_TIMER timer, std::function<void()>* context)
	: _timer(timer), _context(context)
{
}

ThreadPoolTimer::ThreadPoolTimer(ThreadPoolTimer&& other)
	: _timer(other._timer), _context(other._context)
{
	other._timer = nullptr;
}

ThreadPoolTimer& ThreadPoolTimer::operator=(ThreadPoolTimer&& other)
{
	Abort();
	swap(_timer, other._timer);
	swap(_context, other._context);
	return *this;
}

void ThreadPoolTimer::Abort()
{
	if (_timer != nullptr)
	{
		CloseThreadpoolTimer(_timer);
		delete _context;
		_timer = nullptr;
	}
}

ThreadPoolTimer::~ThreadPoolTimer()
{
	Abort();
}