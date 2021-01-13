/****************************************************************************
 * Description: 
 * 
 * Document: https://github.com/hiramtan/HiProtobuf
 * Author: hiramtan@live.com
 ****************************************************************************/

using System;
using HiFramework.Assert;
using Microsoft.Office.Interop.Excel;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HiProtobuf.Lib
{
    internal class ProtoHandler
    {
        public ProtoHandler()
        {
            var path = Settings.Export_Folder + Settings.proto_folder;
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }

        public void Process()
        {
            //递归查询
            string[] files = Directory.GetFiles(Settings.Excel_Folder, "*.xlsx", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var path = files[i];
                if (path.Contains("~$"))//已打开的表格格式
                {
                    continue;
                }
                ProcessExcel(path);
            }
        }

        void ProcessExcel(string path)
        {
            AssertThat.IsNotNullOrEmpty(path);
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
                //for (int i = 1; i <= rowCount; i++)
                //{
                //    for (int j = 1; j <= colCount; j++)
                //    {
                //        var value = ((Range)usedRange.Cells[i, j]).Value2;
                //        var str = value.ToString();
                //    }
                //}
                var name = Path.GetFileNameWithoutExtension(path);
                new ProtoGenerater(name, rowCount, colCount, usedRange).Process();
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
    }
}
