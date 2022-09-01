using UnityEngine;

namespace Origins {
    public abstract class Actor : MonoBehaviour {
        public int InstanceId;

        protected Transform mTransform;
        protected Rigidbody2D rigidBody2D;
        protected Vector2 cacheVector2;

        public abstract void OnUpdate();
        public abstract void OnInit();
        public abstract void OnClear();
        
        public virtual void Move() { }
        public virtual void BeAttack(int value) { }

        public void SetPosition(Vector2 value) {
            mTransform.localPosition = value;
        }

        
    }
}