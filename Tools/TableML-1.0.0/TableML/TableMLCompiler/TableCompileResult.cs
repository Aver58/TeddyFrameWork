using System;
using System.Collections.Generic;
namespace TableML.Compiler
{
    /// <summary>
    /// Invali Excel Exception
    /// </summary>
    public class InvalidExcelException : Exception
    {
        public InvalidExcelException(string msg)
            : base(msg)
        {
        }
    }

    /// <summary>
    /// 返回编译结果
    /// </summary>
    public class TableCompileResult
    {
        public string TabFileFullPath { get; set; }
        public string TabFileRelativePath { get; set; }
        public List<TableColumnVars> FieldsInternal { get; set; } // column + type

        public string PrimaryKey { get; set; }
        public ITableSourceFile ExcelFile { get; internal set; }

        public TableCompileResult()
        {
            FieldsInternal = new List<TableColumnVars>();
        }

    }

}
