namespace Test.ReplaySystem {
    public interface IActor {
        int ActorId { get; set; }
        int MessageType { get; }
        string Serialize();
        void Deserialize(string data);
    }
}