using UnityEngine;
using RenderHeads.Media.AVProMovieCapture;

public class MyScript {
    public static string MyMethod() {
        Debug.Log("这是我的方法");

        string[] commandLineArgs = System.Environment.GetCommandLineArgs();
        string sb = string.Empty;
        foreach (string arg in commandLineArgs)
        {
            Debug.Log("命令行参数：" + arg);
            sb += arg;
        }

        return sb;

    }
}
