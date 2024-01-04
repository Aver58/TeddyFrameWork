using System.Collections.Generic;

namespace Test.ReplaySystem.UEReplaySystem {
    public struct QueuedDemoPacket {
        private List<ushort> Data;
        private int SizeInBits;
        private FOutPacketTraits Traits;
        private short SeenLevelIndex;

        public QueuedDemoPacket(List<ushort> inData, int InSizeBits, FOutPacketTraits inTraits, short inSeenLevelIndex = 0) {
            SizeInBits = InSizeBits;
            Data = new List<ushort>();
            Data = inData;
            SeenLevelIndex = inSeenLevelIndex;
            Traits = inTraits;
        }
    }
}