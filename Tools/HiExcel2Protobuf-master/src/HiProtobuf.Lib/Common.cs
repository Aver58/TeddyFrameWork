/****************************************************************************
 * Description: 
 * 
 * Document: https://github.com/hiramtan/HiProtobuf
 * Author: hiramtan@live.com
 ****************************************************************************/

using System.IO;

namespace HiProtobuf.Lib
{
    internal class Common
    {
        //double
        //float
        //int32
        //int64
        //uint32
        //uint64
        //sint32
        //sint64
        //fixed32
        //fixed64
        //sfixed32
        //sfixed64
        //bool
        //string
        //bytes
        public const string double_ = "double";
        public const string float_ = "float";
        public const string int32_ = "int32";
        public const string int64_ = "int64";
        public const string uint32_ = "uint32";
        public const string uint64_ = "uint64";
        public const string sint32_ = "sint32";
        public const string sint64_ = "sint64";
        public const string fixed32_ = "fixed32";
        public const string fixed64_ = "fixed64";
        public const string sfixed32_ = "sfixed32";
        public const string sfixed64_ = "sfixed64";
        public const string bool_ = "bool";
        public const string string_ = "string";
        public const string bytes_ = "bytes";

        public const string double_s = "double[]";
        public const string float_s = "float[]";
        public const string int32_s = "int32[]";
        public const string int64_s = "int64[]";
        public const string uint32_s = "uint32[]";
        public const string uint64_s = "uint64[]";
        public const string sint32_s = "sint32[]";
        public const string sint64_s = "sint64[]";
        public const string fixed32_s = "fixed32[]";
        public const string fixed64_s = "fixed64[]";
        public const string sfixed32_s = "sfixed32[]";
        public const string sfixed64_s = "sfixed64[]";
        public const string bool_s = "bool[]";
        public const string string_s = "string[]";
        public const string bytes_s = "bytes[]";


        public static readonly string[] VariableType = new[] {
            "double", "float", "int32", "int64", "uint32", "uint64", "sint32", "sint64", "fixed32", "fixed64","sfixed32", "sfixed64", "bool", "string", "bytes",
            "double[]", "float[]", "int32[]", "int64[]", "uint32[]", "uint64[]", "sint32[]", "sint64[]", "fixed32[]", "fixed64[]","sfixed32[]", "sfixed64[]", "bool[]", "string[]", "bytes[]"
        };

        internal static string Cmd(string str)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.Start();

            process.StandardInput.WriteLine(str);
            process.StandardInput.AutoFlush = true;
            process.StandardInput.WriteLine("exit");

            StreamReader reader = process.StandardOutput;//截取输出流

            string output = reader.ReadLine();//每次读取一行

            while (!reader.EndOfStream)
            {
                output += reader.ReadLine();
            }

            process.WaitForExit();
            return output;
        }
    }
}
