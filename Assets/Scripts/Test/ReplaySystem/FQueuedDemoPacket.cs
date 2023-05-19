using System;

// bit 数据
public struct FQueuedDemoPacket {
    // private Array Data;
    // private int SizeInBits;
    // private int Traits;
    // private short SeenLevelIndex;
    public string Data { get; }
    public int FrameIndex { get; }

    public FQueuedDemoPacket(int frameIndex, string data) {
        FrameIndex = frameIndex;
        Data = data;
    }
}