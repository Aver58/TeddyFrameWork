using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Test.ReplaySystem {
    // 回放数据持久化
    public class ReplayStreamer {
        private bool isStreaming; // 是否正在录制文件流
        private string fileName;

        // 开始录制文件流
        public void StartStreaming(string filename) {
            this.fileName = filename;
            ReplayHelper.QueuedDemoPackets = new List<FramePacket>(1024);
            // WriteNetworkDemoHeader();
            isStreaming = true;
        }

        // 停止录制文件流
        public void StopStreaming() {
            if (isStreaming) {
                isStreaming = false;
                // 将序列化后的数据写入文件
                SaveReplayFrameData(fileName, ReplayHelper.QueuedDemoPackets);
            }
        }

        // 更新录制文件流
        public void UpdateStreaming() {
            if (!isStreaming) {
                return;
            }

            // 对所有网络对象 Actor 进行序列化
            var activeActorMap = ReplaySystem.Instance.GetActiveActorMap();
            foreach (var keyValuePair in activeActorMap) {
                var actor = keyValuePair.Value;
                var frameIndex = ReplayHelper.FrameIndex;
                var data = actor.Serialize();
                var messageType = actor.MessageType;
                ReplayHelper.QueuedDemoPackets.Add(new FramePacket(frameIndex, messageType, data));
            }
        }

        // 持久化
        private void SaveReplayFrameData(string fileName, List<FramePacket> fQueuedDemoPackets) {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            Debug.Log(path);
            var fileStream = File.Open(path, FileMode.Create);
            var binaryWriter = new BinaryWriter(fileStream);
            foreach (var fQueuedDemoPacket in fQueuedDemoPackets) {
                binaryWriter.Write(fQueuedDemoPacket.FrameIndex);
                binaryWriter.Write('@');
                binaryWriter.Write((int)fQueuedDemoPacket.MessageType);
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
                var messageType = int.Parse(array[1]);
                var packetData = array[2];
                if (!ReplayHelper.PlaybackFrames.ContainsKey(frameIndex)) {
                    ReplayHelper.PlaybackFrames[frameIndex] = new List<FramePacket>();
                }

                ReplayHelper.PlaybackFrames[frameIndex].Add(new FramePacket(frameIndex, messageType, packetData));
            }

            binaryReader.Close();
            fileStream.Close();
        }

        // 文件头
        private static void WriteNetworkDemoHeader() {

        }
    }
}