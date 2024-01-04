using RenderHeads.Media.AVProMovieCapture;
using UnityEngine;

public class TestCommandLineArgsCall {
    public static void TestCall() {
        Debug.LogError("!TestCall");

        var go = new GameObject();
        var capture = go.AddComponent<CaptureFromScreen>();
        capture.IsRealTime = false;
        capture.FrameRate = 60f;
        capture.StopMode = StopMode.FramesEncoded;
        capture.StopAfterFramesElapsed = (int)(capture.FrameRate * 10f);
        capture.NativeForceVideoCodecIndex = -1;
        capture.VideoCodecPriorityWindows = new string[] { "H264", "HEVC" };
        capture.OutputFolder = CaptureBase.OutputPath.RelativeToProject;
        capture.StartCapture();
    }
}