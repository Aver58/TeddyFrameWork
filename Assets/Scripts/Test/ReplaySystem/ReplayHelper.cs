using System.Collections.Generic;

namespace Test.ReplaySystem {
    // 回放管理器
    public class ReplayHelper {
        private static bool isPlaying;
        private static float replaySpeed = 1.0f; // 回放速度

        public static bool IsRecording;
        public static int FrameIndex;
        public static ReplayStreamer ReplayStreamer;
        public static List<FramePacket> QueuedDemoPackets;
        public static List<FramePacket> QueuedCheckpointPackets;
        public static Dictionary<int, List<FramePacket>> PlaybackFrames;

        public static void StartRecording(string fileName) {
            IsRecording = true;
            FrameIndex = 0;
            ReplayStreamer.StartStreaming(fileName);
        }

        public static void StopRecording() {
            IsRecording = false;
            ReplayStreamer.StopStreaming();
        }

        public static void OnUpdateRecording() {
            if (!IsRecording) {
                return;
            }

            ReplayStreamer.UpdateStreaming();
            FrameIndex++;
        }

        public static void StartReplay() {
            isPlaying = true;
            FrameIndex = 0;
            PlaybackFrames = new Dictionary<int, List<FramePacket>>();
            ReplayStreamer.LoadReplayData();
        }

        public static void UpdateReplay() {
            if (!isPlaying) {
                return;
            }

            if (PlaybackFrames.ContainsKey(FrameIndex)) {
                for (var i = 0; i < PlaybackFrames[FrameIndex].Count; i++) {
                    var packet = PlaybackFrames[FrameIndex][i];
                    // MessageSystem.Instance.DispatchMessage(packet.MessageType, packet.Data);
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

        #region Test 业务

        public static void LoadCubeActor(string assetName, int actorId) {
            LoadModule.LoadTestModel(assetName, (assetRequest) => {
                var asset = assetRequest.asset as UnityEngine.GameObject;
                var gameObject = UnityEngine.Object.Instantiate(asset);
                var actor = gameObject.GetComponent<CubeActor>();
                actor.ActorId = actorId;
                actor.AssetName = assetName;

                if (IsRecording) {
                    // ReplaySystem.Instance.RegisterPacket(MessageTypeConst.LoadCubeActor, actor.Serialize());
                }
            });
        }

        #endregion
    }
}
