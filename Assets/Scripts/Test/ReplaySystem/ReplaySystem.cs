using System.Collections.Generic;
using Test.ReplaySystem.Command;

namespace Test.ReplaySystem {
    public class ReplaySystem : Singleton<ReplaySystem> {
        public static int ActorInstanceId;
        private Dictionary<int, IActor> activeActorMap;
        public void Init() {
            // MessageSystem.Instance.OnInit();
            // MessageSystem.Instance.RegisterMessage<FramePacket>(MessageTypeConst.LoadCubeActor, OnLoadCubeActor);
            // MessageSystem.Instance.RegisterMessage<FramePacket>(MessageTypeConst.CubeActor, OnCubeActor);
        }

        public void UnInit() {
            // MessageSystem.Instance.OnUnInit();
        }

        public void RegisterActor(IActor actor) {
            activeActorMap.Add(actor.ActorId, actor);
        }

        public void UnRegisterActor(int actorId, IActor actor) {

        }

        public Dictionary<int, IActor> GetActiveActorMap() {
            return activeActorMap;
        }

        public void RegisterPacket(int messageType, string data) {
            var frameIndex = ReplayHelper.FrameIndex;
            ReplayHelper.QueuedDemoPackets.Add(new FramePacket(frameIndex, messageType, data));
        }

        #region 业务

        // private void OnLoadCubeActor(Body body, FramePacket framePacket) {
        //     // LoadCommand
        //     // framePacket.Data
        //     // ReplayHelper.LoadCubeActor();
        // }
        //
        // private void OnCubeActor(Body body, FramePacket framePacket) {
        //     // if (activeActorMap.ContainsKey()) {
        //     //
        //     // }
        //     // framePacket.Data
        // }

        #endregion
    }
}