// 网络驱动层
// NetDriver与World一一对应，在一个游戏世界里面只存在一个NetDriver
// UE里面默认的都是基于 UDPSocket 进行通信的。
public class DemoNetDriver {
    public void Update() {
        TickDemoRecord();
    }

    private void TickDemoRecord() {
        // QueuedDemoPackets 赋值

        // ReplayHelper.ReplayStreamer.UpdateStreaming();
    }
}