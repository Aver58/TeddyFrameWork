using System;
using System.Collections;
using UnityEngine;
using System.IO;
using RenderHeads.Media.AVProMovieCapture;
using UnityEngine.UI;
using UnityEngine.Video;

public class MyVideoListWindow : MonoBehaviour {
    public VideoPlayer VideoPlayer;
    public Text text;

    private string directoryPath;
    private string[] files;
    private Vector2 scrollPosition = Vector2.zero;
    private CaptureGUI captureGUI;
    private bool IsShowUI;
    private int maxRecordFiles = 15;
    private CaptureBase movieCapture;
    private bool isStartRecording;
    private float startRecordTime;

    private void Start() {
        // captureGUI = transform.GetComponent<CaptureGUI>();
        // VideoPlayer.gameObject.SetActive(false);
        // movieCapture = captureGUI.MovieCapture;

        string[] commandLineArgs = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < commandLineArgs.Length; i++) {
            if (commandLineArgs[i] == "-is_release") {
                var value = Convert.ToBoolean(commandLineArgs[i + 1]);
                Debug.LogError("命令行参数：" + value);
                text.text = value.ToString();
            }
        }

        var go = new GameObject();
        var capture = go.AddComponent<CaptureFromScreen>();
        capture.IsRealTime = false;
        capture.FrameRate = 60f;
        capture.StopMode = StopMode.FramesEncoded;
        capture.StopAfterFramesElapsed = (int)(capture.FrameRate * 10f);
        capture.NativeForceVideoCodecIndex = -1;
        capture.VideoCodecPriorityWindows = new string[] { "H264", "HEVC" };
        capture.AudioCaptureSource = AudioCaptureSource.Wwise;
        capture.OutputFolder = CaptureBase.OutputPath.RelativeToProject;
        capture.StartCapture();

        directoryPath = capture.OutputFolderPath;
        text.text += directoryPath;

        StartCoroutine(OnQuit());
    }

    private IEnumerator OnQuit() {
        yield return new WaitForSeconds(10);
        Application.Quit();
    }

    private void OnGUI() {
        GUILayout.Space(50);
        if (GUILayout.Button("录制GUI", GUILayout.Width(100), GUILayout.Height(50))) {
            if (captureGUI) {
                captureGUI.ShowUI = !captureGUI.ShowUI;
            }
        }

        if (GUILayout.Button("录制列表", GUILayout.Width(100), GUILayout.Height(50))) {
            // 获取指定目录下的所有MP4文件
            if (!Directory.Exists(directoryPath)) {
                return;
            }

            files = Directory.GetFiles(directoryPath, "*.mp4");
            IsShowUI = !IsShowUI;
            VideoPlayer.gameObject.SetActive(IsShowUI);
        }

        DrawVideoWindow();

        if (GUILayout.Button("开始每秒录制", GUILayout.Width(100), GUILayout.Height(50))) {
            isStartRecording = !isStartRecording;
            startRecordTime = Time.time;
            movieCapture.StartCapture();
            if (!isStartRecording) {
                // 结束录制，合并视频片段
                MergeAllRecord();
            }
        }
    }

    private void DrawVideoWindow() {
        if (IsShowUI == false){
            return;
        }

        // 创建一个窗口
        GUILayout.BeginArea(new Rect(0, 160, 500, 300), GUI.skin.box);

        // 显示窗口标题
        GUILayout.Label("My Video List");

        // 创建一个滚动视图，用于显示文件列表
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(400), GUILayout.Height(200));

        // 遍历文件列表，并创建一个按钮来播放每个文件
        for (int i = 0; i < files.Length; i++) {
            var fileFullName = files[i];
            GUILayout.BeginHorizontal();
            GUILayout.Label(Path.GetFileName(fileFullName));

            if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(50))) {
                // Utils.OpenInDefaultApp(fileFullName);
                VideoPlayer.url = fileFullName;
                VideoPlayer.Play();
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void DeleteOldestVideo() {
        // todo 缓存最旧的索引
        var allFiles = Directory.GetFiles(directoryPath, "*.mp4");
        if (allFiles.Length > maxRecordFiles) {
            string oldestFile = allFiles[0];
            for (int i = 1; i < allFiles.Length; i++) {
                if (File.GetCreationTime(allFiles[i]) < File.GetCreationTime(oldestFile)) {
                    oldestFile = allFiles[i];
                }
            }
            File.Delete(oldestFile);
        }
    }

    private void Update() {
        if (movieCapture == null || !isStartRecording) {
            return;
        }

        // 判断是否需要开始新的视频录制
        if (Time.time - startRecordTime >= 1) {
            startRecordTime = Time.time;

            // 开始新的录制
            movieCapture.StopCapture();
            movieCapture.StartCapture();

            // 删除超出长度的片段
            DeleteOldestVideo();
        }
    }

    private void MergeAllRecord() {
        // todo FFmpeg
    }
}