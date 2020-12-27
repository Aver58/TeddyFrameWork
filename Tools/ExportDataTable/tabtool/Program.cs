using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Xml;
using System.IO;

namespace tabtool
{
    class Program
    {
        static void Main(string[] args)
        {
            string clientOutDir, serverOutDir, cppOutDir, csOutDir, excelDir, metafile;
            CmdlineHelper cmder = new CmdlineHelper(args);
            if (cmder.Has("--out_client")) { clientOutDir = cmder.Get("--out_client"); } else { return; }
            if (cmder.Has("--out_server")) { serverOutDir = cmder.Get("--out_server"); } else { return; }
            if (cmder.Has("--in_excel")) { excelDir = cmder.Get("--in_excel"); } else { return; }
            if (cmder.Has("--in_tbs")) { metafile = cmder.Get("--in_tbs"); } else { return; }

            //创建导出目录
            if (!Directory.Exists(clientOutDir)) Directory.CreateDirectory(clientOutDir);
            if (!Directory.Exists(serverOutDir)) Directory.CreateDirectory(serverOutDir);

            //先读取tablemata文件
            TableStruct tbs = new TableStruct();
            if(!tbs.ImportTableStruct(metafile))
            {
                Console.WriteLine("解析tbs文件错误！");
                return;
            }
            Console.WriteLine("解析tbs文件成功");

            List<TableMeta> clientTableMetaList = new List<TableMeta>();
            List<TableMeta> serverTableMetaList = new List<TableMeta>();

            //导出文件
            ExcelHelper helper = new ExcelHelper();
            string[] files = Directory.GetFiles(excelDir, "*.xlsx", SearchOption.TopDirectoryOnly);
            foreach (string filepath in files)
            {
                string xmlfile = clientOutDir + Path.GetFileNameWithoutExtension(filepath) + ".txt";
                string txtfile = serverOutDir + Path.GetFileNameWithoutExtension(filepath) + ".txt";
                try
                {
                    DataTable dt = helper.ImportExcelFile(filepath);

                    if (helper.IsExportFile("client", dt))
                    {
                        helper.ExportTxtFileEx(xmlfile, dt, "client", new int[] { 0, 2, 3 });
                        TableMeta meta = helper.ParseTableMeta(Path.GetFileNameWithoutExtension(filepath), dt, "client");
                        clientTableMetaList.Add(meta);
                    }
                    if (helper.IsExportFile("server", dt))
                    {
                        helper.ExportTxtFileEx(txtfile, dt, "server", new int[] { 0, 2, 3 });
                        TableMeta meta = helper.ParseTableMeta(Path.GetFileNameWithoutExtension(filepath), dt, "server");
                        serverTableMetaList.Add(meta);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(filepath + " 文件出错！");
                    Console.WriteLine(e.ToString());
                }
            }
            Console.WriteLine("导出配置文件成功");

            //生成代码
            if (cmder.Has("--out_cpp"))
            {
                cppOutDir = cmder.Get("--out_cpp");
                if (!Directory.Exists(cppOutDir))
                    Directory.CreateDirectory(cppOutDir);

                CodeGen.MakeCppFileTbs(tbs.GetMetaList(), cppOutDir);
                CodeGen.MakeCppFile(serverTableMetaList, cppOutDir);
                Console.WriteLine("生成.cpp代码文件成功");

            }

            if (cmder.Has("--out_cs"))
            {
                csOutDir = cmder.Get("--out_cs");
                if (!Directory.Exists(csOutDir))
                    Directory.CreateDirectory(csOutDir);

                CodeGen.MakeCsharpFileTbs(tbs.GetMetaList(), csOutDir);
                CodeGen.MakeCsharpFile(clientTableMetaList, csOutDir);
                Console.WriteLine("生成.cs代码文件成功");
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey(false);

        }
    }
}
