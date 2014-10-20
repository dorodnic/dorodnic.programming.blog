#pragma once
#ifndef ISHAPE_H
#define ISHAPE_H

#include "PublicAPI.h"

// For convenience, I've decided to include REGISTER_SHAPE into IShape
// This allows you to register the second you inherited from IShape
// without the need to include anything else
#include "ShapeMaker.h"
#define REGISTER_SHAPE(T) static Common::ShapeMaker<T> maker(#T);

namespace Common
{
	// IShape is a public (dll-exported) interface at the base
	// of our hierarchy of shapes
	class PUBLIC_API IShape
	{
	public:
		/// Returns the area of the shape
		virtual double GetArea() const = 0;

		// More virtual functions here

		// Every C++ interface should define a public virtual destructor
		// Why? http://stackoverflow.com/questions/270917/why-should-i-declare-a-virtual-destructor-for-an-abstract-class-in-c
		virtual ~IShape() {};
	};
}

#endif //ISHAPE_H