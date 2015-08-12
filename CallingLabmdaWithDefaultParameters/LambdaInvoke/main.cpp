#include <string>
#include <iostream>

template<typename F>
auto Invoke(F f)
-> decltype(DispatchFunctionPointer(f, &F::operator()))
{
	return DispatchFunctionPointer(f, &F::operator());
}

template<typename F, typename R>
R DispatchFunctionPointer(F& f, R(F::*mf)() const)
{
	return (f.*mf)();
}

template<typename F, typename R, typename A, typename... Args>
R DispatchFunctionPointer(F& f, R(F::*mf)(A, Args...) const)
{
	A a{};
	return Invoke([&](Args... args1) {
		return (f.*mf)(a, args1...);
	});
}

void main()
{
	auto res = Invoke([](int a, double b) {
		return std::to_string(a) + " " + std::to_string(b);
	});

	std::cout << res << std::endl;
}