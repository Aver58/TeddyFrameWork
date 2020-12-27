using System;
using System.Linq;
using System.Text;
using NPOI.SS.UserModel;
using System.IO;
using System.Data;
using NPOI.XSSF.UserModel;
using System.Xml;

namespace tabtool
{
    public class ExcelHelper
    {
        IWorkbook hssfworkbook;
        public DataTable ImportExcelFile(string filePath)
        {
            try
            {
                using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    hssfworkbook = new XSSFWorkbook(file);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            ISheet sheet = hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();
            IRow row0 = sheet.GetRow(0);
            for (int j = row0.FirstCellNum; j < (row0.LastCellNum); j++)
            {
                dt.Columns.Add(row0.GetCell(j).ToString());
            }

            while (rows.MoveNext())
            {
                IRow row = (XSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    ICell cell = row.GetCell(i);
                    if (cell == null)
                    {
                        dr[i] = null;
                    }
                    else
                    {
                        dr[i] = cell.ToString();
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public void ExportXmlFile(string filepath, DataTable dt, int firstrow)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                XmlWriterSettings setting = new XmlWriterSettings();
                setting.Indent = true;
                setting.Encoding = new UTF8Encoding(false);
                setting.NewLineChars = Environment.NewLine;

                using (XmlWriter xw = XmlWriter.Create(fs, setting))
                {
                    xw.WriteStartDocument(true);
                    xw.WriteStartElement("datas");
                    for (int i = firstrow; i < dt.Rows.Count; i++)
                    {
                        xw.WriteStartElement("data");
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            xw.WriteAttributeString(dt.Columns[j].ToString(), dt.Rows[i].ItemArray[j].ToString());
                        }
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                    xw.WriteEndDocument();
                }
            }
        }

        public void ExportTxtFile(string filepath, DataTable dt, int firstrow)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                for (int i = firstrow; i < dt.Rows.Count; i++)
                {
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j == dt.Columns.Count - 1)
                        {
                            sw.WriteLine(dt.Rows[i].ItemArray[j].ToString());
                        }
                        else
                        {
                            sw.Write(dt.Rows[i].ItemArray[j].ToString() + "\t");
                        }
                    }
                }
                sw.Close();
            }
        }

        public bool IsExportFile(string key, DataTable dt)
        {
            return IsExportField(key, dt, 0);
        }

        public bool IsExportField(string key, DataTable dt, int col)
        {
            if (string.Compare(dt.Rows[3].ItemArray[col].ToString(), "all", true) == 0)
            {
                return true;
            }
            if (string.Compare(dt.Rows[3].ItemArray[col].ToString(), key, true) == 0)
            {
                return true;
            }
            return false;
        }

        internal TableMeta ParseTableMeta(string filename, DataTable dt, string cmp)
        {
            TableMeta meta = new TableMeta();
            meta.TableName = filename;
            //第0行注释 第一行name 第二行type
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (!IsExportField(cmp, dt, i)) { continue; }
                TableField field = new TableField();
                field.fieldName = dt.Rows[1].ItemArray[i].ToString();
                field.typeName = dt.Rows[2].ItemArray[i].ToString();
                if (field.typeName == "int") { field.fieldType = TableFieldType.IntField; }
                else if (field.typeName == "float") { field.fieldType = TableFieldType.FloatField; }
                else if (field.typeName == "string") { field.fieldType = TableFieldType.StringField; }
                else if (field.typeName == "int+") { field.fieldType = TableFieldType.IntList; }
                else if (field.typeName == "float+") { field.fieldType = TableFieldType.FloatList; }
                else if (field.typeName == "string+") { field.fieldType = TableFieldType.StringList; }
                else if (field.typeName[field.typeName.Length - 1] == '+') { field.fieldType = TableFieldType.StructList; }
                else { field.fieldType = TableFieldType.StructField; }
                meta.Fields.Add(field);
            }
            return meta;
        }

        public void ExportTxtFileEx(string filepath, DataTable dt, string key, int[] ignorerows)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (ignorerows.Contains(i)) continue;
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (!IsExportField(key,dt,j))
                        {
                            if (j == dt.Columns.Count - 1)
                            {
                                sw.Write("\n");
                            }
                            continue;
                        }
                        if (j == dt.Columns.Count - 1)
                        {
                            sw.WriteLine("\t" + dt.Rows[i].ItemArray[j].ToString());
                        }
                        else if (j == 0)
                        {
                            sw.Write(dt.Rows[i].ItemArray[j].ToString());
                        }
                        else
                        {
                            sw.Write("\t" + dt.Rows[i].ItemArray[j].ToString());
                        }
                    }
                }
                sw.Close();
            }
        }

    }
}
