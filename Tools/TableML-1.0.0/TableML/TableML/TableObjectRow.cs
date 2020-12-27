using System;
using System.Collections.Generic;

namespace TableML
{
    /// <summary>
    /// Row parser for TableObject
    /// </summary>
    public class TableObjectRow : TableFileRow
    {
        public TableObjectRow(int rowNumber, Dictionary<string, HeaderInfo> headerInfos) : base(rowNumber, headerInfos)
        {
        }

        public override object GetPrimaryKey()
        {
            var key = base.GetPrimaryKey();
            double num;
            if (key is string && double.TryParse(key.ToString(), out num))
            {
                return num;
            }

            return key;
        }

        public new object Get(int index)
        {
            return this[index];
        }

        public new object Get(string headerName)
        {
            return this[headerName];
        }

        /// <summary>
        /// Get Value by Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public new object this[int index]
        {
            get
            {
                if (index > Values.Length || index < 0)
                {
                    throw new Exception(string.Format("Overflow index `{0}`", index));
                }

                var value = Values[index];
                object result;
                double number;
                if (!double.TryParse(value, out number))
                {
                    result = value;
                }
                else
                {
                    result = number;
                }
                return result;
            }
            set
            {
                Values[index] = value.ToString();
            }
        }

        /// <summary>
        /// Get or set Value by Indexer, be careful the `newline` character!
        /// </summary>
        /// <param name="headerName"></param>
        /// <returns></returns>
        public new object this[string headerName]
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
