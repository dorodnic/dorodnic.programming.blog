#include "Circle.h"
#define _USE_MATH_DEFINES
#include <math.h>

namespace SimpleShapes
{
	REGISTER_SHAPE(Circle);

	double Circle::GetArea() const
	{
		return 2 * M_PI * _radius;
	}

	Circle::Circle(double radius) 
		: _radius(radius) 
	{

	}

	Circle::Circle(rapidxml::xml_node<>* node)
		: _radius(0)
	{
		rapidxml::xml_attribute<>* pAttr = node->first_attribute("Radius");
		_radius = atof(pAttr->value());
	}
}