using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 回放数据持久化
public class NetworkReplayStreamer {
    private bool isStreaming;  // 是否正在录制文件流
    private string fileName;

    // 开始录制文件流
    public void StartStreaming(string filename) {
        this.fileName = filename;
        ReplayHelper.QueuedDemoPackets = new List<FQueuedDemoPacket>(1024);
        // WriteNetworkDemoHeader();
        isStreaming = true;
    }

    // 停止录制文件流
    public void StopStreaming() {
        if (isStreaming) {
            isStreaming = false;
            // 将序列化后的数据写入文件
            SaveReplayData(fileName, ReplayHelper.QueuedDemoPackets);
        }
    }

    // 更新录制文件流
    public void UpdateStreaming() {
        if (!isStreaming) {
            return;
        }

        // 对所有网络对象 Actor 进行序列化
        var allActors = GetAllActor();
        foreach (var actor in allActors) {
            var serialize = actor.Serialize();
            var frameIndex = ReplayHelper.FrameIndex;
            // 将序列化后的数据写入文件或者网络流中
            ReplayHelper.QueuedDemoPackets.Add(new FQueuedDemoPacket(frameIndex, serialize));
        }
    }

    private Actor[] GetAllActor() {
        var actors = GameObject.FindObjectsOfType<Actor>();
        return actors;
    }

    // 持久化
    private void SaveReplayData(string fileName, List<FQueuedDemoPacket> fQueuedDemoPackets) {
        var path = Path.Combine(Application.persistentDataPath, fileName);
        Debug.Log(path);
        var fileStream = File.Open(path, FileMode.Create);
        var binaryWriter = new BinaryWriter(fileStream);
        foreach (var fQueuedDemoPacket in fQueuedDemoPackets) {
            binaryWriter.Write(fQueuedDemoPacket.FrameIndex);
            binaryWriter.Write('@');
            binaryWriter.Write(fQueuedDemoPacket.Data);
            binaryWriter.Write('\t');
        }

        binaryWriter.Close();
        fileStream.Close();
    }

    // 读取持久化的回放数据
    public void LoadReplayData() {
        var path = Path.Combine(Application.persistentDataPath, fileName);
        var fileStream = File.Open(path, FileMode.Open);
        var binaryReader = new BinaryReader(fileStream);
        var data = binaryReader.ReadString();
        var dataArray = data.Split('\t');
        ReplayHelper.PlaybackFrames.Clear();
        foreach (var packetString in dataArray) {
            var array = packetString.Split('@');
            var frameIndex = int.Parse(array[0]);
            var packetData = array[1];
            if (!ReplayHelper.PlaybackFrames.ContainsKey(frameIndex)) {
                ReplayHelper.PlaybackFrames[frameIndex] = new List<FQueuedDemoPacket>();
            }

            ReplayHelper.PlaybackFrames[frameIndex].Add(new FQueuedDemoPacket(frameIndex, packetData));
        }

        binaryReader.Close();
        fileStream.Close();
    }

    // 文件头
    private static void WriteNetworkDemoHeader() {

    }
}