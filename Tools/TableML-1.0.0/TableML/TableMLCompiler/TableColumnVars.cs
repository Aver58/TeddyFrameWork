namespace TableML.Compiler
{
	/// <summary>
	/// Table column metadata variables.
	/// </summary>
	public class TableColumnVars
	{
		public int Index { get; set; }
		public string Type { get; set; }

		/// <summary>
		/// 经过格式化，去掉[]的类型字符串，支持数组(int[] -> int_array), 字典(map[string]int) -> map_string_int
		/// </summary>
		public string TypeMethod
		{
			get { return Type.Replace(@"[]", "_array").Replace("<", "_").Replace(">", "").Replace(",", "_"); }
		}

		/// <summary>
		/// 类型
		/// </summary>
		public string FormatType
		{
			get
			{
				return Type;
			}
		}

		public string Name { get; set; }
		public string DefaultValue { get; set; }
		public string Comment { get; set; }
	}

}

