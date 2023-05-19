using System.Collections.Generic;

// 回放管理器
public class ReplayHelper {
    private static bool isRecording;
    private static bool isPlaying;
    private static float replaySpeed = 1.0f; // 回放速度

    public static int FrameIndex;
    public static NetworkReplayStreamer ReplayStreamer;
    public static List<FQueuedDemoPacket> QueuedDemoPackets;
    public static List<FQueuedDemoPacket> QueuedCheckpointPackets;
    public static Dictionary<int, List<FQueuedDemoPacket>> PlaybackFrames;

    public static void StartRecording(string fileName) {
        isRecording = true;
        FrameIndex = 0;
        ReplayStreamer.StartStreaming(fileName);
    }

    public static void StopRecording() {
        isRecording = false;
        ReplayStreamer.StopStreaming();
    }

    public static void OnUpdateRecording() {
        if (!isRecording) {
            return;
        }

        ReplayStreamer.UpdateStreaming();
        FrameIndex++;
    }

    public static void StartReplay() {
        isPlaying = true;
        FrameIndex = 0;
        PlaybackFrames = new Dictionary<int, List<FQueuedDemoPacket>>();
        ReplayStreamer.LoadReplayData();
    }

    public static void UpdateReplay() {
        if (!isPlaying) {
            return;
        }

        if (PlaybackFrames.ContainsKey(FrameIndex)) {
            for (var i = 0; i < PlaybackFrames[FrameIndex].Count; i++) {
                var packet = PlaybackFrames[FrameIndex][i];
                // packet.Data
            }
        }

        FrameIndex++;
    }

    // 回放数据停止
    public static void StopReplay() {
        isPlaying = false;
    }

    // 回放数据暂停
    public static void PauseReplay() {

    }

    // 回放数据恢复
    public static void ResumeReplay() {

    }

    // 回放数据速度设置
    public static void SetReplaySpeed(float speed) {
    }

    // 回放数据进度设置
    public static void SetReplayProgress(float progress) {
    }

    // 回放数据帧数设置
    public static void SetReplayProgressByFrame(int frame) {
    }
}
