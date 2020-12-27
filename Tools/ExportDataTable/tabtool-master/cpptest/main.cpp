#include "tableconfig.h"
#include <iostream>

int main()
{
	if (LoadTableConfig())
	{
		auto item = Singleton<cfgAchiveTable>::Instance()->GetTableItem(1);
		if (item != nullptr)
		{
			std::cout << item->test3[1].value << std::endl;
		}
	}
}