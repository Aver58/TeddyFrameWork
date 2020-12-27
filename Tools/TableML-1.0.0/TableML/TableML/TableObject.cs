using System;
using System.Text;

namespace TableML
{
    /// <summary>
    /// 自动适配number(double)和string
    /// </summary>
    public class TableObject : TableFile<TableObjectRow>
    {
        public TableObject(string content) : base(new string[] { content }) { }
        public TableObject(string[] contents) : base(contents) { }
        public TableObject() : base() { }
        public TableObject(string fileFullPath, Encoding encoding) : base(fileFullPath, encoding) { }

		/// <summary>
		/// Loads from string.
		/// </summary>
		/// <returns>The from string.</returns>
		/// <param name="content">Content.</param>
        public new static TableObject LoadFromString(params string[] content)
        {
            TableObject tabFile = new TableObject(content);
            return tabFile;
        }

		/// <summary>
		/// Loads from file.
		/// </summary>
		/// <returns>The from file.</returns>
		/// <param name="fileFullPath">File full path.</param>
		/// <param name="encoding">Encoding.</param>
        public new static TableObject LoadFromFile(string fileFullPath, Encoding encoding = null)
        {
			return new TableObject(fileFullPath, encoding);
        }

		/// <summary>
		/// Finds the by primary key, auto convert number.
		/// </summary>
		/// <returns>The by primary key.</returns>
		/// <param name="primaryKey">Primary key.</param>
		/// <param name="throwError">If set to <c>true</c> throw error.</param>
        public override TableObjectRow FindByPrimaryKey(object primaryKey, bool throwError = true)
        {
            // to number double
            if (primaryKey is short || primaryKey is int || primaryKey is long || primaryKey is decimal ||
                primaryKey is uint || primaryKey is ulong || primaryKey is ushort ||
                primaryKey is float || primaryKey is bool)
            {
                return base.FindByPrimaryKey(Convert.ChangeType(primaryKey, typeof(double)), false);
            }

            return base.FindByPrimaryKey(primaryKey, throwError);

        }
    }

}
