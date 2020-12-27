
# TableML

TableML, Table Markup Language, 基于电子表格的标记语言，

类似JSON, XML, INI，TableML可以作为软件项目的配置标记语言，

与之不同的是，您可以使用Excel等电子表格编辑软件来配置TableML，自由地添加图标、注释、VB脚本和预编译指令，再由TableML编译器导出干净的TSV格式的配置表表格，编辑方便，使用简单。

目前提供C#版本的运行时、编译器、代码生成器。

## Example


您可以使用Excel编译如下内容，并保存为文件setting/test.xls:

| Id            | #Something       | Value    | Comment               |
| ---           | ---              | ---      | ---                   |
| int           | string           | string   | string                |
| 关键字/注释行 | 带#开头的注释列  | 内容     | 带Comment开头的注释列 |
| 1             | 无用注释         | Abcdefg  | 一些注释              |
| #注释行       | 无用注释         | Abcdefg  | 一些注释              |
| Comment注释行 | 无用注释         | Abcdefg  | 一些注释              |
| 2             | 无用注释         | Yuiop    | 一些注释              |
| #if LANG_TW   |                  |          |                       |
| 123           | 这一行不会被编译 | skldfjlj | 一些注释              |
| #endif        |                  |          |                       |


然后使用TableML命令行编译器：
```bash
TableML.exe --Src setting --To setting2 --CodeFile Code.cs

```

执行后，将会生成setting2/test.tml文件，打开可以看见编译后内容：

| Id  | Value   |
| --- | ---     |
| int | string  |
| 1   | Abcdefg |
| 2   | Yuiop   |

另外附带一份Code.cs，自动生成的代码。


## TableML编辑规则

以上的例子中，展示了TableML的大部分特性：

- TableML使用Excel等电子表格软件作为编辑器，并通过编译器导出成tml格式文件
- tml格式文件实质是TSV格式，即Tab Sperated Values，类似CSV
- 行头占3行：
    - 第1行是列名
    - 第2行是列的信息，通常是声明列的类型,可以自定义
    - 第3行是列的注释
    - 除外的所有行为内容
- 列名内容以#开头或Comment开头，改列被视为注释列，编译器忽略
- 行内容的第一个单元格内容，以#开头或Comment开头，改行被视为注释行，编译器忽略
- 可以使用预编译指令#if和#endif，条件式控制编译的行



# 自动读取配置代码生成

TableML编译器内置Liquid模板引擎。您可以自定义模板内容，来为不同的语言生成读表类。

TableML是[KSFramework](https://github.com/mr-kelly/KSFramework)的一部分，用于游戏配置表读取代码，支持热重载、分表等机制。

# TableML for C#/Mono/Xamarin

TableML目前只提供C#版本。当前TableML使用基于Xamarin Studio开发，TableML的C#版本具备了跨平台特性（Windows/Mac/Linux）。
