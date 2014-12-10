#include "Logic.h"
#include "..\MeaningOfLife.Cpp\Logic.h"
#include <string>
#include <Windows.h>

using namespace std;

MeaningOfLife::Cpp::CLI::Logic::Logic()
	: _impl(new Cpp::Logic()) 
	// Allocate some memory for the native implementation
{
}

int MeaningOfLife::Cpp::CLI::Logic::Get()
{
	return _impl->Get(); // Call native Get
}

void MeaningOfLife::Cpp::CLI::Logic::Destroy()
{
	if (_impl != nullptr)
	{
		delete _impl;
		_impl = nullptr;
	}
}

MeaningOfLife::Cpp::CLI::Logic::~Logic()
{
	// C++ CLI compiler will automaticly make all ref classes implement IDisposable.
	// The default implementation will invoke this method + call GC.SuspendFinalize.
	Destroy(); // Clean-up any native resources 
}

MeaningOfLife::Cpp::CLI::Logic::!Logic()
{
	// This is the finalizer
	// It's essentially a fail-safe, and will get called
	// in case Logic was not used inside a using block.
	Destroy(); // Clean-up any native resources 
}

string ManagedStringToStdString(System::String^ str)
{
	cli::array<unsigned char>^ bytes = System::Text::Encoding::ASCII->GetBytes(str);
	pin_ptr<unsigned char> pinned = &bytes[0];
	std::string nativeString((char*)pinned, bytes->Length);
	return nativeString;
}

void MeaningOfLife::Cpp::CLI::Logic::InitializeLibrary(System::String^ path)
{
	string nativePath = ManagedStringToStdString(path);
	LoadLibrary(nativePath.c_str()); // Actually load the delayed library from specific location
}
