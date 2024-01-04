using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FFMpegMerge {
    public class BuildPostProcessor_FFMpegMerge {
        private static string ffmpegFileFullName = Application.dataPath + "/FFmpegMerge/ffmpeg.exe";

        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {
            if (target == BuildTarget.StandaloneWindows64) {
                MoveExeToBuildFolder(pathToBuiltProject);
            }
        }

        private static void MoveExeToBuildFolder(string buildPath) {
            var dataPath = $"/{PlayerSettings.productName}_Data/";
            string destinationFolder = Path.GetDirectoryName(buildPath) + dataPath;

            // 创建目标文件夹（如果不存在）
            if (!Directory.Exists(destinationFolder)) {
                Directory.CreateDirectory(destinationFolder);
            }

            string destinationPath = Path.Combine(destinationFolder, "ffmpeg.exe");
            File.Copy(ffmpegFileFullName, destinationPath, true);
            Debug.LogFormat("Moved {0} to {1}", ffmpegFileFullName, destinationPath);
        }
    }
}