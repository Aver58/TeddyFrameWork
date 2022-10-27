using UnityEngine;

namespace Origins {
    public abstract class AbsActor : MonoBehaviour {
        public int InstanceId;

        protected Transform mTransform;
        protected Vector2 cacheVector;
        protected Quaternion cacheQuaternion;

        public abstract void OnInit();
        public abstract void OnUpdate();
        public abstract void OnClear();
        
        public virtual void BeAttack(int value) { }

        public void SetLocalPosition(Vector3 value) {
            mTransform.localPosition = value;
        }

        public void SetLocalRotation(Quaternion value) {
            mTransform.localRotation = value;
        }
    }
}