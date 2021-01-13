/****************************************************************************
 * Description: 
 * 
 * Document: https://github.com/hiramtan/HiProtobuf
 * Author: hiramtan@live.com
 ****************************************************************************/

using Google.Protobuf;
using Google.Protobuf.Collections;
using HiFramework.Assert;
using Microsoft.Office.Interop.Excel;
using System;
using System.IO;
using System.Reflection;

namespace HiProtobuf.Lib
{
    internal class DataHandler
    {
        private Assembly _assembly;
        private object _excelIns;
        public DataHandler()
        {
            var folder = Settings.Export_Folder + Settings.dat_folder;
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
        }

        public void Process()
        {
            var dllPath = Settings.Export_Folder + Settings.language_folder + Settings.csharp_dll_folder + Compiler.DllName;
            _assembly = Assembly.LoadFrom(dllPath);
            var protoFolder = Settings.Export_Folder + Settings.proto_folder;
            string[] files = Directory.GetFiles(protoFolder, "*.proto", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                string protoPath = files[i];
                string name = Path.GetFileNameWithoutExtension(protoPath);
                string excelInsName = "HiProtobuf.Excel_" + name;
                _excelIns = _assembly.CreateInstance(excelInsName);
                string excelPath = Settings.Excel_Folder + "/" + name + ".xlsx";
                ProcessData(excelPath);
            }
        }

        private void ProcessData(string path)
        {
            AssertThat.IsTrue(File.Exists(path), "Excel file can not find");
            var name = Path.GetFileNameWithoutExtension(path);
            var excelApp = new Application();
            var workbooks = excelApp.Workbooks.Open(path);
            try
            {
                var sheet = workbooks.Sheets[1];
                AssertThat.IsNotNull(sheet, "Excel's sheet is null");
                Worksheet worksheet = sheet as Worksheet;
                AssertThat.IsNotNull(sheet, "Excel's worksheet is null");
                var usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;
                int colCount = usedRange.Columns.Count;
                for (int i = 4; i <= rowCount; i++)
                {
                    var excel_Type = _excelIns.GetType();
                    var dataProp = excel_Type.GetProperty("Data");
                    var dataIns = dataProp.GetValue(_excelIns);
                    var dataType = dataProp.PropertyType;
                    var ins = _assembly.CreateInstance("HiProtobuf." + name);
                    var addMethod = dataType.GetMethod("Add", new Type[] {typeof(int), ins.GetType()});
                    int id = (int) ((Range) usedRange.Cells[i, 1]).Value2;
                    addMethod.Invoke(dataIns, new[] {id, ins});
                    for (int j = 1; j <= colCount; j++)
                    {
                        var variableType = ((Range) usedRange.Cells[2, j]).Text.ToString();
                        var variableName = ((Range) usedRange.Cells[3, j]).Text.ToString();
                        var variableValue = ((Range) usedRange.Cells[i, j]).Text.ToString();
                        var insType = ins.GetType();
                        var fieldName = variableName + "_";
                        FieldInfo insField =
                            insType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                        var value = GetVariableValue(variableType, variableValue);
                        insField.SetValue(ins, value);
                    }
                }
                Serialize(_excelIns);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                workbooks.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }

        object GetVariableValue(string type, string value)
        {
            if (type == Common.double_)
                return double.Parse(value);
            if (type == Common.float_)
                return float.Parse(value);
            if (type == Common.int32_)
                return int.Parse(value);
            if (type == Common.int64_)
                return long.Parse(value);
            if (type == Common.uint32_)
                return uint.Parse(value);
            if (type == Common.uint64_)
                return ulong.Parse(value);
            if (type == Common.sint32_)
                return int.Parse(value);
            if (type == Common.sint64_)
                return long.Parse(value);
            if (type == Common.fixed32_)
                return uint.Parse(value);
            if (type == Common.fixed64_)
                return ulong.Parse(value);
            if (type == Common.sfixed32_)
                return int.Parse(value);
            if (type == Common.sfixed64_)
                return long.Parse(value);
            if (type == Common.bool_)
                return value == "1";
            if (type == Common.string_)
                return value.ToString();
            if (type == Common.bytes_)
                return ByteString.CopyFromUtf8(value.ToString());
            if (type == Common.double_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<double> newValue = new RepeatedField<double>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(double.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.float_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<float> newValue = new RepeatedField<float>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(float.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.int32_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<int> newValue = new RepeatedField<int>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(int.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.int64_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<long> newValue = new RepeatedField<long>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(long.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.uint32_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<uint> newValue = new RepeatedField<uint>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(uint.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.uint64_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<ulong> newValue = new RepeatedField<ulong>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(ulong.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.sint32_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<int> newValue = new RepeatedField<int>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(int.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.sint64_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<long> newValue = new RepeatedField<long>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(long.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.fixed32_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<uint> newValue = new RepeatedField<uint>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(uint.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.fixed64_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<ulong> newValue = new RepeatedField<ulong>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(ulong.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.sfixed32_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<int> newValue = new RepeatedField<int>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(int.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.sfixed64_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<long> newValue = new RepeatedField<long>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(long.Parse(datas[i]));
                }
                return newValue;
            }
            if (type == Common.bool_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<bool> newValue = new RepeatedField<bool>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(datas[i] == "1");
                }
                return newValue;
            }
            if (type == Common.string_s)
            {
                string data = value.Trim('"');
                string[] datas = data.Split('|');
                RepeatedField<string> newValue = new RepeatedField<string>();
                for (int i = 0; i < datas.Length; i++)
                {
                    newValue.Add(datas[i]);
                }
                return newValue;
            }
            AssertThat.Fail("Type error");
            return null;
        }

        void Serialize(object obj)
        {
            var type = obj.GetType();
            var path = Settings.Export_Folder + Settings.dat_folder + "/" + type.Name + ".dat";
            using (var output = File.Create(path))
            {
                MessageExtensions.WriteTo((IMessage)obj, output);
            }
        }
    }
}