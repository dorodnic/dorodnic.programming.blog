#include "ShapeFactory.h"
#include "IShapeMaker.h"
#include "IShape.h"
#include <Windows.h>

namespace Common
{
	ShapeFactory& ShapeFactory::Instance()
	{
		// So called Meyers Singleton implementation,
		// In C++ 11 it is in fact thread-safe
		// In older versions you should ensure thread-safety here
		static ShapeFactory factory;
		return factory;
	}

	void ShapeFactory::RegisterMaker(const std::string& key, IShapeMaker* maker)
	{
		// Validate uniquness and add to the map
		if (_makers.find(key) != _makers.end())
		{
			throw new std::exception("Multiple makers for given key!");
		}
		_makers[key] = maker;
	}

	IShape* ShapeFactory::Create(rapidxml::xml_node<> * node) const
	{
		// If XML node relies on some external library,
		// load it before continuing
		rapidxml::xml_attribute<>* pAttr = node->first_attribute("LibraryName");
		if (pAttr != NULL)
		{
			std::string libraryName(pAttr->value());
			// Note: Unicode has been disabled to avoid extra conversions
			// (This is a toy example)
			LoadLibrary(libraryName.c_str());
		}

		// Look up the maker by nodes name
		std::string key(node->name());
		auto i = _makers.find(key);
		if (i == _makers.end())
		{
			throw new std::exception("Unrecognized object type!");
		}
		IShapeMaker* maker = i->second;
		// Invoke create polymorphiclly
		return maker->Create(node);
	}
}