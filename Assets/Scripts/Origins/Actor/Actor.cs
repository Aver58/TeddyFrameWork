using UnityEngine;

namespace Origins {
    public abstract class Actor : MonoBehaviour {
        public int InstanceId;
        
        public abstract void OnUpdate();
        public abstract void OnInit();
        public abstract void OnClear();
        
        public virtual void Move() { }
        public virtual void BeAttack(int value) { }
        
    }
}