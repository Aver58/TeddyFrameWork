#region Copyright (c) 2015 KEngine / Kelly <http://github.com/mr-kelly>, All rights reserved.

// KEngine - AssetBundle framework for Unity3D
// ===================================
// 
// Author:  Kelly
// Email: 23110388@qq.com
// Github: https://github.com/mr-kelly/KEngine
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library.

#endregion
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace TableML
{
    public enum TableFileExceptionType
    {
        DuplicatedKey,

        HeadLineNull,
        MetaLineNull, // 第二行
        NotFoundHeader,
        NotFoundGetMethod,
        NotFoundPrimaryKey,
        NotFoundRow,
    }

    /// <summary>
    /// 表头信息
    /// </summary>
    public class HeaderInfo
    {
        public int ColumnIndex;
        public string HeaderName;
        public string HeaderMeta;
    }

    public delegate void TableFileExceptionDelegate(TableFileExceptionType exceptionType, string[] args);

    public class TableFileStaticConfig
    {
        public static TableFileExceptionDelegate GlobalExceptionEvent;
    }

    public class TableFileConfig
    {
        /// <summary>
        /// Contents use stream, will be more effective
        /// </summary>
        public Stream[] ContentStreams;

        /// <summary>
        /// Use string to parse
        /// </summary>
        public string[] Contents;

        public char[] Separators = new char[] { '\t' };
        /// <summary>
        /// How to handle error
        /// </summary>
        public TableFileExceptionDelegate OnExceptionEvent;
        /// <summary>
        /// Default Encoding : UTF-8
        /// </summary>
        public Encoding Encoding = Encoding.UTF8;
    }

    /// <summary>
    /// Simple way of default table file, all value is string
    /// 默认的TableFile，所有的单元格内容会被视为string类型，包括数字也是string类型
    /// </summary>
    public class TableFile : TableFile<TableFileRow>
    {
        public TableFile(string content) : base(new string[] { content }) { }
        public TableFile(string[] contents) : base(contents) { }
        public TableFile() : base() { }
        public TableFile(string fileFullPath, Encoding encoding) : base(fileFullPath, encoding) { }

        public new static TableFile LoadFromString(params string[] content)
        {
            TableFile tabFile = new TableFile(content);
            return tabFile;
        }

        public new static TableFile LoadFromFile(string fileFullPath, Encoding encoding = null)
        {
            return new TableFile(fileFullPath, encoding);
        }
    }

}
