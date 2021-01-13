/****************************************************************************
 * Description: 
 * 
 * Document: https://github.com/hiramtan/HiProtobuf
 * Author: hiramtan@live.com
 ****************************************************************************/

using System;
using System.IO;
using HiProtobuf.Lib;
using System.Xml.Serialization;

namespace HiProtobuf.UI
{
    internal static class Config
    {
        private static string _path = Environment.CurrentDirectory + "/Config.xml";

        internal static void Load()
        {
            if (File.Exists(_path))
            {
                XmlSerializer xs = XmlSerializer.FromTypes(new Type[] { typeof(PathConfig) })[0];
                Stream stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.Read);
                PathConfig pathCfg = xs.Deserialize(stream) as PathConfig;
                Settings.Export_Folder = pathCfg.Export_Folder;
                Settings.Excel_Folder = pathCfg.Excel_Folder;
                Settings.Compiler_Path = pathCfg.Compiler_Path;
                stream.Close();
            }
        }

        internal static void Save()
        {
            if (File.Exists(_path)) File.Delete(_path);
            var pathCfg = new PathConfig();
            pathCfg.Export_Folder = Settings.Export_Folder;
            pathCfg.Excel_Folder = Settings.Excel_Folder;
            pathCfg.Compiler_Path = Settings.Compiler_Path;
            XmlSerializer xs = XmlSerializer.FromTypes(new Type[] { typeof(PathConfig) })[0];
            Stream stream = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.Read);
            xs.Serialize(stream, pathCfg);
            stream.Close();
        }
    }
    public class PathConfig
    {
        public string Export_Folder;
        public string Excel_Folder;
        public string Compiler_Path;
    }
}
