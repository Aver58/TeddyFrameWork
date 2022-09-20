using UnityEngine;

namespace Origins {
    public abstract class AbsActor : MonoBehaviour {
        public int InstanceId;

        protected Transform mTransform;
        protected Vector3 cacheVector3;

        public abstract void OnInit();
        public abstract void OnUpdate();
        public abstract void OnClear();
        
        public virtual void Move() { }
        public virtual void BeAttack(int value) { }

        public void SetPosition(Vector3 value) {
            mTransform.localPosition = value;
        }
    }
}