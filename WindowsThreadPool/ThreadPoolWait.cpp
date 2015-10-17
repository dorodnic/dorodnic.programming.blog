#include "stdafx.h"
#include "ThreadPoolWait.h"

using namespace std;

ThreadPoolWait::ThreadPoolWait(PTP_WAIT wait, Context* context)
	: _wait(wait), _context(context)
{
}

ThreadPoolWait::ThreadPoolWait(ThreadPoolWait&& other)
	: _wait(other._wait), _context(other._context)
{
	other._wait = nullptr;
}

ThreadPoolWait& ThreadPoolWait::operator=(ThreadPoolWait&& other)
{
	Abort();
	swap(_wait, other._wait);
	swap(_context, other._context);
	return *this;
}

void ThreadPoolWait::Abort()
{
	if (_wait != nullptr)
	{
		CloseThreadpoolWait(_wait);
		delete _context;
		_wait = nullptr;
	}
}

ThreadPoolWait::~ThreadPoolWait()
{
	Abort();
}