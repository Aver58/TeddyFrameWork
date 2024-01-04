namespace Test.ReplaySystem.UEReplaySystem {
    public class ReplayNetConnection : NetConnection {
        private int DemoFrameNum;

        public void StartRecording() {
            UpdateLevelVisibility();

            // 注册 OnLevelAddedToWorldHandle
            // 注册 OnLevelRemovedFromWorldHandle

            ReplayHelper.StartRecording(this);
            ReplayHelper.CreateSpectatorController(this);
        }

        public void UpdateLevelVisibility() {

        }

        public void Tick(float DeltaSeconds) {
            DemoFrameNum++;

            ReplayHelper.TickRecording(DeltaSeconds, this);
        }
    }
}