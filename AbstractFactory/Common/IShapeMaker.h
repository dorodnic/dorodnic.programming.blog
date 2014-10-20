#pragma once
#ifndef ISHAPE_MAKER_H
#define ISHAPE_MAKER_H

#include "PublicAPI.h"
#include "RapidXML/rapidxml.hpp"

namespace Common
{
	class IShape;
	
	// IShapeMaker is a public parent of all shape makers
	// It represents a function to be invoked when creating new shape
	class PUBLIC_API IShapeMaker
	{
	public:
		/// Accepts node to pass into shapes constructor
		/// Returns new shape object
		virtual IShape * Create(rapidxml::xml_node<> * node) const = 0;

		// Every C++ interface should define a public virtual destructor
		// Why? http://stackoverflow.com/questions/270917/why-should-i-declare-a-virtual-destructor-for-an-abstract-class-in-c
		virtual ~IShapeMaker() {}
	};
}

#endif //ISHAPE_MAKER_H