using System;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FFMpegHelper {
    private static string tempFileFullName = Application.dataPath + "/filelist.txt";
#if UNITY_EDITOR
    private static string ffmpegFileFullName = Application.dataPath + "/FFmpegMerge/ffmpeg.exe";
#else
    private static string ffmpegFileFullName = Application.dataPath + "/ffmpeg.exe";
#endif

    public static void MergeVideos(string[] inputFiles, string outputFile) {
        var sb = new StringBuilder();
        for (int i = 0; i < inputFiles.Length; i++) {
            Debug.Log("【输入文件】" + inputFiles[i]);
            sb.Append($"file '{inputFiles[i]}'\r\n");
        }

        File.WriteAllText(tempFileFullName, sb.ToString());

        try {
            var param = $" -f concat -safe 0 -i \"{tempFileFullName}\" -c copy -y \"{outputFile}\"";
            Debug.Log("【输入参数】" + param);
            Debug.Log("【输出路径】" + outputFile);
            Process(ffmpegFileFullName, param);
            // uint ptr = 0;
            // Debug.LogError($"ProcessPath:{ffmpegFileFullName}");
            // if (File.Exists(ffmpegFileFullName)) {
            //     try {
            //         // 构建命令行字符串
            //         var parameters = $"-f concat -safe 0 -i \"{tempFileFullName}\" -c copy -y \"{outputFile}\"";
            //         string commandLine = $"\"{ffmpegFileFullName}\" {parameters}";
            //         Debug.LogError($"Args:{parameters}");
            //         ptr = StartExternalProcess.StartProcess(ffmpegFileFullName, commandLine);
            //         Debug.LogError($"pid:{ptr}");
            //     } catch (Exception e) {
            //         Debug.LogError(e);
            //         throw;
            //     }
            // } else {
            //     Debug.LogError($"File doesnt exist {ffmpegFileFullName}");
            // }
            //
            // File.Delete(tempFileFullName);


            // using (Process p = new Process()) {
            //     p.StartInfo = new ProcessStartInfo(ffmpegFileFullName, $"-f concat -safe 0 -i {tempFileFullName} -c copy {outputFile}");
            //     p.StartInfo.RedirectStandardOutput = true;
            //     p.StartInfo.RedirectStandardError = true;
            //     p.StartInfo.UseShellExecute = false;
            //     p.StartInfo.CreateNoWindow = true;
            //
            //     Debug.Log("【输入参数】" + p.StartInfo.Arguments);
            //     Debug.Log("【输出路径】" + outputFile);
            //     p.Start();
            //     var output = p.StandardOutput.ReadToEnd();
            //     var error = p.StandardError.ReadToEnd();
            //     p.WaitForExit();
            //     p.Close();
            //     Debug.Log("【输出内容】" + output);
            //     Debug.Log("【输出内容】" + error);
            // }
            //
            // File.Delete(tempFileFullName);
        } catch (Exception e) {
            Debug.LogError(e.Message);
            throw;
        }
    }

    public static void Process(string processPath, string command) {
        uint ptr = 0;
        UnityEngine.Debug.Log($"ProcessPath:{processPath}");
        if (File.Exists(processPath)) {
            var args = command;
            UnityEngine.Debug.Log($"Args:{args}");
            if (ptr != 0) {
                UnityEngine.Debug.Log("Internal server process already exists");
            } else {
                var commandLine = $"\"{processPath}\" {args}";
                UnityEngine.Debug.Log(commandLine);
                #if UNITY_EDITOR
                    ptr = StartExternalProcess.Start(commandLine, null);
                #endif
                UnityEngine.Debug.Log($"pid:{ptr}");
            }
        } else {
            UnityEngine.Debug.Log($"File doesnt exist");
        }
    }
}
