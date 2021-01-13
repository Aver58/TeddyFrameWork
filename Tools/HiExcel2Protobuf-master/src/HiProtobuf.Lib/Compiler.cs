/****************************************************************************
 * Description: 
 * 
 * Document: https://github.com/hiramtan/HiProtobuf
 * Author: hiramtan@live.com
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiProtobuf.Lib
{
    internal class Compiler
    {
        public static readonly string DllName = "/HiProtobuf.Excel.csharp.dll";

        public Compiler()
        {
            var folder = Settings.Export_Folder + Settings.language_folder + Settings.csharp_dll_folder;
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
            Directory.CreateDirectory(folder);
        }

        public void Porcess()
        {
            var commond = @"-target:library -out:{0} -reference:{1} -recurse:{2}\*.cs";
            var dllPath = Settings.Export_Folder + Settings.language_folder + Settings.csharp_dll_folder + DllName;
            var csharpFolder = Settings.Export_Folder + Settings.language_folder + Settings.csharp_folder;
            commond = Settings.Compiler_Path + " " + string.Format(commond, dllPath, Settings.Protobuf_Dll_Path, csharpFolder);
            Common.Cmd(commond);
        }
    }
}
