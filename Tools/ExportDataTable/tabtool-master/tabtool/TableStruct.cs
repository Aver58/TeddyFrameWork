using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace tabtool
{
    class TableStruct
    {
        enum ParseState
        {
            EndStruct,
            BeginStruct,
        }

        ParseState m_parseState = ParseState.EndStruct;
        TableMeta m_tableMeta = null;
        List<TableMeta> m_metaList = new List<TableMeta>();

        public List<TableMeta> GetMetaList()
        {
            return m_metaList;
        }

        public TableMeta GetTableMetaByName(string name)
        {
            foreach (var meta in m_metaList)
            {
                if (meta.TableName == name)
                {
                    return meta;
                }
            }
            return null;
        }

        public bool ImportTableStruct(string filepath)
        {
            m_metaList.Clear();
            string[] lines = File.ReadAllLines(filepath);
            for (int i = 0; i < lines.Count(); i++)
            {
                if (Regex.IsMatch(lines[i], @"^\s*//.*$")) { continue; }//注释
                if (Regex.IsMatch(lines[i], @"^\s*$")) { continue; }//空行
                if (Regex.IsMatch(lines[i], @"^\s*\w+\s+{\s*$"))//结构体开始
                {
                    if (m_parseState != ParseState.EndStruct)
                    {
                        Console.WriteLine("tbs文件错误：第{0}行", i);
                        return false;
                    }
                    m_parseState = ParseState.BeginStruct;
                    Match match = Regex.Match(lines[i], @"^\s*(\w+)\s+{\s*$");
                    m_tableMeta = new TableMeta();
                    m_tableMeta.TableName = match.Groups[1].Value;
                    continue;
                }
                if (Regex.IsMatch(lines[i], @"^\s*}\s*$"))//结构体结束
                {
                    if(m_parseState != ParseState.BeginStruct)
                    {
                        Console.WriteLine("tbs文件错误：第{0}行", i);
                        return false;
                    }
                    m_parseState = ParseState.EndStruct;
                    m_metaList.Add(m_tableMeta);
                    continue;
                }
                if (m_parseState == ParseState.BeginStruct)
                {
                    Match m = Regex.Match(lines[i], @"\s*(\w+)\s+(\w+)\s*$");//var type
                    if (m.Success == false)
                    {
                        Console.WriteLine("tbs文件错误：第{0}行", i);
                        return false;
                    }

                    TableField field = new TableField();
                    field.fieldName = m.Groups[1].Value;
                    field.typeName = m.Groups[2].Value;
                    switch (field.typeName)
                    {
                        case "int": { field.fieldType = TableFieldType.IntField; break; }
                        case "float": { field.fieldType = TableFieldType.FloatField; break; }
                        case "string": { field.fieldType = TableFieldType.StringField; break; }
                        case "int+": { field.fieldType = TableFieldType.IntList; break; }
                        case "float+": { field.fieldType = TableFieldType.FloatList; break; }
                        case "string+": { field.fieldType = TableFieldType.StringList; break; }
                        default:
                            Console.WriteLine("tbs文件错误：第{0}行", i);
                            return false;
                    }
                    m_tableMeta.Fields.Add(field);
                    continue;
                }

                Console.WriteLine("tbs文件错误：第{0}行", i);
                return false;
            }
            return true;
        }
    }
}
