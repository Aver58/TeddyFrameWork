// 网络驱动层
// NetDriver与World一一对应，在一个游戏世界里面只存在一个NetDriver
// UE里面默认的都是基于 UDPSocket 进行通信的。
public class DemoNetDriver {
    public void Update(float DeltaSeconds) {
        TickDemoRecord(DeltaSeconds);
    }

    private void TickDemoRecord(float DeltaSeconds) {
        // QueuedDemoPackets 赋值

        // ReplayHelper.ReplayStreamer.UpdateStreaming();
        TickDemoRecordFrame(DeltaSeconds);

        // TickCheckpoint
    }

    private void TickDemoRecordFrame(float DeltaSeconds) {

    }
}