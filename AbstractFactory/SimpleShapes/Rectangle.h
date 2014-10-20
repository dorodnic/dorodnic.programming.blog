#pragma once
#ifndef RECTANGLE_H
#define RECTANGLE_H

#include "../Common/IShape.h"

namespace SimpleShapes
{
	// Toy Rectangle implementation
	// Note: concrete shapes don't have to be exported
	class Rectangle : public Common::IShape
	{
	public:
		Rectangle(double width, double height);

		Rectangle(rapidxml::xml_node<>* node);

		virtual double GetArea() const;

	private:
		double _width;
		double _height;
	};
}

#endif //RECTANGLE_H