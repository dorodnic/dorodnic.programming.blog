#include "Rectangle.h"

namespace SimpleShapes
{
	REGISTER_SHAPE(Rectangle);

	double Rectangle::GetArea() const
	{
		return _height * _width;
	}

	Rectangle::Rectangle(double width, double height) 
		: _width(width), _height(height)
	{

	}

	Rectangle::Rectangle(rapidxml::xml_node<>* node)
		: _width(0), _height(0)
	{
		rapidxml::xml_attribute<>* pWidthAttr = node->first_attribute("Width");
		_width = atof(pWidthAttr->value());

		rapidxml::xml_attribute<>* pHeightAttr = node->first_attribute("Height");
		_height = atof(pHeightAttr->value());
	}
}