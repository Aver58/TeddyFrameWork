using System.Runtime.InteropServices;
using RenderHeads.Media.AVProMovieCapture;
using UnityEngine;

public class TestCaptureVideo : MonoBehaviour {
    private const string X264CodecName = "x264vfw - H.264/MPEG-4 AVC codec";
    private const string FallbackCodecName = "Uncompressed";

    [SerializeField]
    private int _width = 512;

    [SerializeField]
    private int _height = 512;

    [SerializeField]
    private int _frameRate = 30;

    [SerializeField]
    private string _filePath;

    // State
    private Codec _videoCodec;
    private int _encoderHandle;
    public Camera camera;
    public Camera uiCamera;

    void Start() {
        // if (NativePlugin.Init()) {
        //     // Find the index for the video codec
        //     _videoCodec = CodecManager.FindCodec(CodecType.Video, X264CodecName);
        //     if (_videoCodec == null) {
        //         _videoCodec = CodecManager.FindCodec(CodecType.Video, FallbackCodecName);
        //     }
        //     string pluginVersionString = NativePlugin.GetPluginVersionString();
        //     Debug.Log("[AVProMovieCapture] Init version: " + NativePlugin.ScriptVersion + " (plugin v" +
        //               pluginVersionString +") with GPU " + SystemInfo.graphicsDeviceName + " " +
        //               SystemInfo.graphicsDeviceVersion + " OS: " + SystemInfo.operatingSystem);
        // } else {
        //     this.enabled = false;
        // }
        //
        // var SettingsPrefix = "AVProMovieCapture.EditorWindow.";
        // var folder = System.IO.Path.GetFullPath(System.IO.Path.Combine(Application.dataPath, ".."));
        //
        // _filePath = folder + "/Captures/test.mp4";
        // Debug.LogError(_filePath);
        // CreateVideoFromByteArray(_filePath, _width, _height, _frameRate);

        GameObject go = new GameObject();
        var capture = go.AddComponent<CaptureFromCamera>();
        capture.SetCamera(camera, new []{ uiCamera });
        capture.IsRealTime = false;
        capture.FrameRate = 60f;
        capture.StopMode = StopMode.FramesEncoded;
        capture.StopAfterFramesElapsed = (int)(capture.FrameRate * 60) ;
        capture.NativeForceVideoCodecIndex = -1;
        capture.VideoCodecPriorityWindows = new string[] { "H264", "HEVC" };
        capture.OutputFolder = CaptureBase.OutputPath.RelativeToProject;
        capture.CameraRenderResolution = CaptureBase.Resolution.Custom;
        capture.CameraRenderCustomResolution = new Vector2(960, 540);

        capture.StartCapture();
    }

    private void OnDestroy() {
        NativePlugin.Deinit();
    }

    public void CreateVideoFromByteArray(string filePath, int width, int height, int frameRate) {
        byte[] frameData = new byte[width * height * 4];
        GCHandle frameHandle = GCHandle.Alloc(frameData, GCHandleType.Pinned);

        // Start the recording session
        int encoderHandle = NativePlugin.CreateRecorderVideo(filePath, (uint)width, (uint)height, frameRate, (int)NativePlugin.PixelFormat.RGBA32, false, false, _videoCodec.Index, AudioCaptureSource.None, 0, 0, -1, -1, true, null);
        if (encoderHandle >= 0) {
            NativePlugin.Start(encoderHandle);

            // Write out 100 frames
            int numFrames = 100;
            for (int i = 0; i < numFrames; i++) {
                // TODO: fill the byte array with your own data :)自己加帧好像可行！
                // CaptureBase.EncodeTexture

                // Wait for the encoder to be ready for the next frame
                int numAttempts = 32;
                while (numAttempts > 0)
                {
                    if (NativePlugin.IsNewFrameDue(encoderHandle))
                    {
                        // Encode the new frame
                        NativePlugin.EncodeFrame(encoderHandle, frameHandle.AddrOfPinnedObject());
                        break;
                    }
                    System.Threading.Thread.Sleep(1);
                    numAttempts--;
                }
            }

            // End the session
            NativePlugin.Stop(encoderHandle, false);
            NativePlugin.FreeRecorder(encoderHandle);
        }

        if (frameHandle.IsAllocated) {
            frameHandle.Free();
        }
    }
}
