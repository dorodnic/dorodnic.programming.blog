#pragma once
#ifndef PUBLIC_API_H
#define PUBLIC_API_H

/// Standard way to limit the export of symbols from DLL
/// Inside current project, ABSTRACT_FACTORY_EXAMPLE macro is defined,
/// hence PUBLIC_API will be translated to dll-export
/// In any other consumer project, it will be translated to dll-import

#ifdef ABSTRACT_FACTORY_EXAMPLE
	#define PUBLIC_API __declspec(dllexport)
#else
	#define PUBLIC_API __declspec(dllimport)
#endif

#endif // PUBLIC_API_H