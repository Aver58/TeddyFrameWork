using System;
using UnityEngine;

namespace Test.ReplaySystem {
    public abstract class Actor : MonoBehaviour, IActor {
        public int ActorId { get; set; }
        public int MessageType { get;}
        public abstract string Serialize();
        public abstract void Deserialize(string data);
    }
}