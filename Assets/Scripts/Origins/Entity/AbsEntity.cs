using UnityEngine;

namespace Origins {
    public abstract class AbsEntity : ILifeCycle {
        public int InstanceId;

        public Vector2 LocalPosition { get; set; }
        public Vector2 LocalForward { get; set; }

        public abstract void OnInit();
        public abstract void OnUpdate();
        public abstract void OnClear();
    }
}