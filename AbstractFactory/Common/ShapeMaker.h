#pragma once
#ifndef SHAPE_MAKER_H
#define SHAPE_MAKER_H

#include "IShapeMaker.h"
#include <string>
#include "ShapeFactory.h"

namespace Common
{
	/// Helper template to simplify the process of generating shape-maker
	template<typename T>
	class ShapeMaker : public IShapeMaker
	{
	public:
		/// When created, the shape maker will automaticly register itself with the factory
		/// Note - you are discouraged from using ShapeMaker outside REGISTER_SHAPE macro
		/// For example, creating ShapeMaker on the stack will end up badly
		ShapeMaker(const std::string& key)
		{
			ShapeFactory::Instance().RegisterMaker(key, this);
		}

		virtual IShape * Create(rapidxml::xml_node<> * node) const
		{
			// Create new instance of T using constructor from XML
			// Assumes T has a constructor that accepts rapidxml::xml_node<>
			return new T(node);
		}
	};
}

#endif //SHAPE_MAKER_H