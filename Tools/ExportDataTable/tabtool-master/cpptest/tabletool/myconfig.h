#pragma once

//说明：这个文件用来定制您的个性化需求

#define ErrorLog printf				//由于去除了日志类，错误日志用printf代替
#define WORK_DIR  "../config/";	//config所在目录

const int MAX_TABLE_ROWS = 8000;	//表最大行数
const int MAX_LINE_LEN = 8000;		//每行最大长度
