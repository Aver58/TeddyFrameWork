using System;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 子弹表现层
    public sealed class ProjectileActor : ILifeCycle {
        private int id;
        private GameObject gameObject;
        private Vector3 sourcePosition;
        private Vector3 sourceForward;

        public void Clear() {
            gameObject = null;
            
        }
        
        public void MoveTo(Vector3 targetPosition, Vector3 targetForward) {
            
        }
        
        public void Pause() {
            
        }
        
        public void Continue() {
            
        }

        public void OnInit() { }
        public void OnInit(int id, GameObject gameObject, Vector3 sourcePosition, Vector3 sourceForward) {
            this.id = id;
            this.gameObject = gameObject;
            this.sourcePosition = sourcePosition;
            this.sourceForward = sourceForward;
        }

        public void OnUpdate() {
        }

        public void OnClear() {
        }

    }
}