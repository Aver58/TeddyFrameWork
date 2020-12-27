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
using System.Collections.Generic;

namespace TableML
{
    /// <summary>
    /// 默认的，用于TableFile类的通用行类，所有类被视为string
    /// </summary>
    public partial class TableFileRow : TableRowFieldParser
    {
        /// <summary>
        /// TableFileRow's row number of table
        /// </summary>
        public int RowNumber { get; internal set; }

        /// <summary>
        /// 是否自动使用反射解析，不自动，则使用Parse方法
        /// </summary>
        public virtual bool IsAutoParse
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:TableML.TableFileRow"/> class.
		/// </summary>
		/// <param name="rowNumber">Row number.</param>
		/// <param name="headerInfos">Header infos.</param>
        public TableFileRow(int rowNumber, Dictionary<string, HeaderInfo> headerInfos)
        {
            Ctor(rowNumber, headerInfos);
        }

		/// <summary>
		/// Ctor the specified rowNumber and headerInfos.
		/// </summary>
		/// <param name="rowNumber">Row number.</param>
		/// <param name="headerInfos">Header infos.</param>
        private void Ctor(int rowNumber, Dictionary<string, HeaderInfo> headerInfos)
        {
            RowNumber = rowNumber;
            HeaderInfos = headerInfos;
            Values = new string[headerInfos.Count];
        }

        /// <summary>
        /// When true, will use reflection to map the Tab File
        /// </summary>
        //public virtual bool IsAutoParse
        //{
        //    get { return false; }
        //}

        /// <summary>
        /// Table Header, name and type definition
        /// </summary>
        public Dictionary<string, HeaderInfo> HeaderInfos { get; internal set; }

        /// <summary>
        /// Store values of this row
        /// </summary>
        public string[] Values { get; internal set; }

        /// <summary>
        /// Cache save the row values
        /// </summary>
        /// <param name="cellStrs"></param>
        public virtual void Parse(string[] cellStrs)
        {
            
        }

        /// <summary>
        /// Use first object of array as primary key!
        /// </summary>
        public object PrimaryKey
        {
            get { return GetPrimaryKey(); }
        }

        /// <summary>
        /// By Default, primary key is the first column.
        /// </summary>
        public virtual object GetPrimaryKey()
        {
            return Get(0);
        }

		/// <summary>
		/// Get value by the specified index, real implements of all `Get` method
		/// </summary>
		/// <param name="index">Index.</param>
        public virtual object Get(int index)
        {
            if (index > Values.Length || index < 0)
            {
                throw new Exception(string.Format("Overflow index `{0}`", index));
            }

            return Values[index];
        }

		/// <summary>
		/// Get the specified headerName.
		/// </summary>
		/// <param name="headerName">Header name.</param>
        public string Get(string headerName)
        {
            return this[headerName];
        }

        /// <summary>
        /// Get Value by Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index]
        {
            get
            {
                return Get(index) as string;
            }
            set { Values[index] = value; }
        }

        /// <summary>
        /// Get or set Value by Indexer, be careful the `newline` character!
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public string this[string headerName]
        {
            get
            {
                HeaderInfo headerInfo;
                if (!HeaderInfos.TryGetValue(headerName, out headerInfo))
                {
                    throw new Exception("not found header: " + headerName);
                }

                return this[headerInfo.ColumnIndex];
            }
            set
            {
                HeaderInfo headerInfo;
                if (!HeaderInfos.TryGetValue(headerName, out headerInfo))
                {
                    throw new Exception("not found header: " + headerName);
                }

                this[headerInfo.ColumnIndex] = value;
            }
        }
    }
}
