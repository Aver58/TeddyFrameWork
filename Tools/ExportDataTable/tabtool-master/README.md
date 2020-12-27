# tabtool
导表工具，excel表格导出csv配置文件并生成C++\C#代码解析配表

## 推荐工作流：
   策划案--解决方案--前后端配置需求汇总--表格版本--打表工具--配置文件--代码生成。
   
## 命名规则。
1. 驼峰命名。
2. 表格命名：系统名+表名.xsl 
3. 字段命名：使用通用的单词 id type count 等

## excel表规则。
-    第一行注释，给策划看，也会在生成代码中作为注释
-    第二行name，也是生成代码中的结构体字段名称
-    第三行filter，"client"标识该字段只导出到客户端配置文件，"server"标识只导出到服务器配置文件，"all"标识前后端都需要，"not"标识不导出该字段。
-    第四行type，参考下面的字段类型说明。
-    每个表第一个字段必须是id字段，id必须从1开始，0是读表错误。
-    id字段的filter如果标识为"client"则表示这个表只导出客户端配置文件，不导出服务器配表。反之亦然。

 ## 字段类型
 
-    int 整数和bool
-    float 浮点数
-    string 字符串
-    int+ 整数迭代
-    float+ 浮点数迭代
-    string+ 字符串迭代
-    tbsIdCount 定义在meta.tbs中的结构体
-    tbsIdCount+ 结构体迭代
    
## 复合字段及其迭代
- 一级字段迭代：`11,22,33,44`在type中用`int+`表示。
- 二级字段迭代：`1,1;2,2`在type中用`tbsIdCount+`表示。
- 通过一个结构描述文件支持结构体，`meta.tbs`。
- 我认为表字段结构体嵌套是没有意义的，所以仅支持到二级复合字段。
- 注意excel中填写,时要设置单元格为文本模式，否则会变成数字分隔符。
- tbs文件非常简单，如下就定义一个结构体tbsIdCount:

```c
//表示id和数量
tbsIdCount {
    id int
    count int
}
```

## 代码生成
- C++版本 tbs文件tablestruct.h  csv文件生成生成一对tableconfig.h/tableconfig.cpp
- C#版本  tbs文件生成TableStruct.cs  csv文件生成TableConfig.cs
- Go版本  TODO 暂时没用到，用到了再支持

## 错误检查
    类型模式不匹配的字段会在打表过程中检查出来。
## 导表工具使用
    参考test目录中`一键导出表.bat`的用法。
```
    "../tabtool/bin/Debug/tabtool.exe" --out_client ../csharptest/config/ --out_server ../cpptest/config/ --out_cpp ../cpptest/ --out_cs ../csharptest/ --in_excel ./ --in_tbs ./meta.tbs
```--out_client 指定导出客户端导出配置文件目录
   --out_server 导出服务器配置文件目录
   --out_cpp 导出C++代码目录，可选
   --out_cs 导出C#代码目录，可选
   --in_excel excel文件所在的目录
   --in_tbs tbs文件路径（表中用到的结构体）
   
 ## C++使用
 
 
