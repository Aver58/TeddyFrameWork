namespace Test.ReplaySystem.UEReplaySystem {
    public class GameInstance {
        public ReplaySubsystem ReplaySubSystem;

        public void StartRecordingReplay(string name) {
            ReplaySubSystem.RecordReplay(name);
        }
    }
}