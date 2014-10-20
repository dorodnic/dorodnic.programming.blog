#pragma once
#ifndef CIRCLE_H
#define CIRCLE_H

#include "../Common/IShape.h"

namespace SimpleShapes
{
	// Toy circle implementation
	// Note: concrete shapes don't have to be exported
	class Circle : public Common::IShape
	{
	public:
		Circle(double radius);

		Circle(rapidxml::xml_node<>* node);

		virtual double GetArea() const;

	private:
		double _radius;
	};
}

#endif //CIRCLE_H