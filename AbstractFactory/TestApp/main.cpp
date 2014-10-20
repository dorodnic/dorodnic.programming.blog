#include "../Common/ShapeFactory.h"

using namespace rapidxml;
using namespace Common;

void main()
{
	char * str = "<Rectangle LibraryName=\"SimpleShapes.dll\" Width=\"5.3\" Height=\"3.7\" />";
	std::string content(str);

	// Parse XML string
	xml_document<> doc;
	doc.parse<0>(&content[0]);

	// Use factory to recover the shape
	auto shape = ShapeFactory::Instance().Create(doc.first_node());
	auto area = shape->GetArea();
}