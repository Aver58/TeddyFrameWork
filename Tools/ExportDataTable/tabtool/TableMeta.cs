using System.Collections.Generic;

namespace tabtool
{
    enum TableFieldType
    {
        IntField,
        FloatField,
        StringField,
        StructField,
        IntList,
        FloatList,
        StringList,
        StructList,
    }

    class TableField
    {
        public TableFieldType fieldType;
        public string fieldName;
        public string typeName;

        public string GetCppTypeName()
        {
            string[] ts = { "int", "float", "string", "xxx", "vector<int>", "vector<float>", "vector<string>", "xxx" };
            if (fieldType == TableFieldType.StructField)
            {
                return typeName;
            }
            if (fieldType == TableFieldType.StructList)
            {
                return string.Format("vector<{0}>", typeName.Substring(0, typeName.Length - 1));
            }
            return ts[(int)fieldType];
        }

        public string GetTypeNameOfStructList()
        {
            return typeName.Substring(0, typeName.Length - 1);
        }


        public string GetCsharpTypeName()
        {
            string[] ts = { "int", "float", "string", "xxx", "List<int>", "List<float>", "List<string>", "xxx" };
            if (fieldType == TableFieldType.StructField)
            {
                return typeName;
            }
            if (fieldType == TableFieldType.StructList)
            {
                return string.Format("List<{0}>", typeName.Substring(0, typeName.Length - 1));
            }
            return ts[(int)fieldType];
        }

    }

    class TableMeta
    {
        public string TableName;
        public List<TableField> Fields = new List<TableField>();
        public string GetClassName()
        {
            return TableName + "Table";
        }

        public string GetItemName()
        {
            return TableName + "Item";
        }
    }
}
