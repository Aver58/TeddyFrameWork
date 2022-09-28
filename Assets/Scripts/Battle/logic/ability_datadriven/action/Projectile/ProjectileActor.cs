using System;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 子弹表现层
    public sealed class ProjectileActor {
        private Transform transform;
        public void Init(Transform transform, Vector3 sourcePosition, Vector3 sourceForward) {
            this.transform = transform;
            
            
        }

        public void Clear() {
            transform = null;
            
        }
        
        public void MoveTo(Vector3 targetPosition, Vector3 targetForward) {
            
        }
        
        public void Pause() {
            
        }
        
        public void Continue() {
            
        }
    }
}