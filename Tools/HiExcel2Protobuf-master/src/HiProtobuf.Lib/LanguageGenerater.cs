/****************************************************************************
 * Description: 
 * 
 * Document: https://github.com/hiramtan/HiProtobuf
 * Author: hiramtan@live.com
 ****************************************************************************/
using System.IO;

namespace HiProtobuf.Lib
{
    internal class LanguageGenerater
    {
        private string _languageFolder;
        public void Process()
        {
            _languageFolder = Settings.Export_Folder + Settings.language_folder;
            if (Directory.Exists(_languageFolder))
            {
                Directory.Delete(_languageFolder, true);
            }
            Directory.CreateDirectory(_languageFolder);

            var protoFolder = Settings.Export_Folder + Settings.proto_folder;
            Process_csharp(protoFolder);
            Process_cpp(protoFolder);
            Process_go(protoFolder);
            Process_java(protoFolder);
            Process_python(protoFolder);
        }

        private void Process_csharp(string protoPath)
        {
            var outFolder = _languageFolder + Settings.csharp_folder;
            Directory.CreateDirectory(outFolder);
            //递归查询
            string[] files = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var command = Settings.Protoc_Path + string.Format(" -I={0} --csharp_out={1} {2}", protoPath, outFolder, filePath);
                var log = Common.Cmd(command);
            }
        }

        private void Process_cpp(string protoPath)
        {
            var outFolder = _languageFolder + Settings.cpp_folder;
            Directory.CreateDirectory(outFolder);
            //递归查询
            string[] files = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var command = Settings.Protoc_Path + string.Format(" -I={0} --cpp_out={1} {2}", protoPath, outFolder, filePath);
                var log = Common.Cmd(command);
            }
        }

        private void Process_go(string protoPath)
        {
            var outFolder = _languageFolder + Settings.go_folder;
            Directory.CreateDirectory(outFolder);
            //递归查询
            string[] files = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var command = Settings.Protoc_Path + string.Format(" -I={0} --go_out={1} {2}", protoPath, outFolder, filePath);
                var log = Common.Cmd(command);
            }
        }

        private void Process_java(string protoPath)
        {
            var outFolder = _languageFolder + Settings.java_folder;
            Directory.CreateDirectory(outFolder);
            //递归查询
            string[] files = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var command = Settings.Protoc_Path + string.Format(" -I={0} --java_out={1} {2}", protoPath, outFolder, filePath);
                var log = Common.Cmd(command);
            }
        }

        private void Process_python(string protoPath)
        {
            var outFolder = _languageFolder + Settings.python_folder;
            Directory.CreateDirectory(outFolder);
            //递归查询
            string[] files = Directory.GetFiles(protoPath, "*.proto", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                var filePath = files[i];
                var command = Settings.Protoc_Path + string.Format(" -I={0} --python_out={1} {2}", protoPath, outFolder, filePath);
                var log = Common.Cmd(command);
            }
        }
    }
}
