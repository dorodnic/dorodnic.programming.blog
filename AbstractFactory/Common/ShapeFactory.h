#pragma once
#ifndef SHAPE_FACTORY_H
#define SHAPE_FACTORY_H

#include <map>
#include <string>
#include "RapidXML/rapidxml.hpp"
#include "PublicAPI.h"
#include "IShape.h"
#include "IShapeMaker.h"

namespace Common
{
	/// Abstract-Factory Pattern Implementation
	class PUBLIC_API ShapeFactory
	{
	public:
		/// Factory is implemented as a Singleton
		static ShapeFactory& Instance();

		/// Adds new shape maker with given key
		void RegisterMaker(const std::string& key, IShapeMaker * maker);

		/// Creates new IShape from unknow XML node
		IShape * Create(rapidxml::xml_node<> * node) const;

	private:
		ShapeFactory() {}

		// Disable copying and assignment
		ShapeFactory(const ShapeFactory& other); 
		ShapeFactory& operator=(const ShapeFactory& other);

		/// Maps keys to shape makers
		/// Note: using either the map or string makes our code not binary-compatible
		/// Memory layout and implementation of these classes will change from compiler to compiler
		std::map<std::string, IShapeMaker*> _makers;
	};
}

#endif //SHAPE_FACTORY_H