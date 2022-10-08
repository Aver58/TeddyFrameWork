using UnityEngine;

namespace Origins {
    public abstract class AbsEntity : ILifeCycle {
        public int InstanceId;

        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }

        public abstract void OnInit();
        public abstract void OnUpdate();
        public abstract void OnClear();
    }
}