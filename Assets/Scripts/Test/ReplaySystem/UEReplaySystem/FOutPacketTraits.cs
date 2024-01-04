namespace Test.ReplaySystem.UEReplaySystem {
    // Contains metadata and flags, which provide information on the traits of a packet - what it contains and how to process it.
    public struct FOutPacketTraits {
        public bool bAllowCompression;

        /** The number of ack bits in the packet - reflecting UNetConnection.NumAckBits */
        public short NumAckBits;

        /** The number of bunch bits in the packet - reflecting UNetConnection.NumBunchBits */
        public short NumBunchBits;

        /** Whether or not this is a keepalive packet */
        public bool bIsKeepAlive;

        /** Whether or not the packet has been compressed */
        public bool bIsCompressed;

        public FOutPacketTraits(bool allowCompression = true) {
            bAllowCompression = true;
            NumAckBits = 0;
            NumBunchBits = 0;
            bIsKeepAlive = false;
            bIsCompressed = false;
        }
    }
}