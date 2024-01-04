using System.Collections.Generic;

namespace Test.ReplaySystem.UEReplaySystem {
    public class ReplayHelper {
        private static LocalFileNetworkReplayStreamer ReplayStreamer;

        private static List<QueuedDemoPacket> QueuedDemoPackets;
        private static List<QueuedDemoPacket> QueuedCheckpointPackets;

        public static void StartRecording(NetConnection connection) {
            // 初始化 FStartStreamingParameters

            ReplayStreamer.StartStreaming();

            AddNewLevel();

            WriteNetworkDemoHeader();
        }

        public static void TickRecording(float DeltaSeconds, NetConnection Connection) {
            // RecordFrame
            RecordFrame(DeltaSeconds, Connection);
            // SaveCheckpoint
        }

        public static void CreateSpectatorController(NetConnection connection) {
            // 初始化 复制的 Actor，克隆世界实例化
        }

        private static void AddNewLevel() {

        }

        // 写文件头
        private static void WriteNetworkDemoHeader() {

        }

        private static void RecordFrame(float DeltaSeconds, NetConnection Connection) {

            WriteDemoFrame(Connection, QueuedDemoPackets);
        }

        private static void WriteDemoFrame(NetConnection Connection, List<QueuedDemoPacket> QueuedDemoPackets) {

            for (int i = 0; i < QueuedDemoPackets.Count; i++) {
                var DemoPacket = QueuedDemoPackets[i];

                // WritePacket(DemoPacket);
            }
        }
    }
}