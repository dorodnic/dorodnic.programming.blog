#pragma once

namespace MeaningOfLife
{
	namespace Cpp
	{
		// First a Forward Declaration to Cpp::Logic class:
		class Logic; // This allows us to mention it in this header file
		// without actually including the native version of Logic.h

		namespace CLI
		{
			// Next is the managed wrapper of Logic:
			public ref class Logic
			{
			public:
				// Managed wrappers are generally less concerned 
				// with copy constructors and operators, since .NET will
				// not call them most of the time.
				// The methods that do actually matter are:
				// The constructor, the "destructor" and the finalizer
				Logic();
				~Logic();
				!Logic();

				int Get();

				void Destroy();

				static void InitializeLibrary(System::String^ path);
			private:
				// Pointer to our implementation
				Cpp::Logic* _impl;
			};
		}
	}	
}