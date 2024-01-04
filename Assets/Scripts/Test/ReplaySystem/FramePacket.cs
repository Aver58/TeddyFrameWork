public struct FramePacket {
    public string Data { get; }
    public int FrameIndex { get; }
    public int MessageType { get; }
    public FramePacket(int frameIndex, int messageType, string data) {
        FrameIndex = frameIndex;
        MessageType = messageType;
        Data = data;
    }
}