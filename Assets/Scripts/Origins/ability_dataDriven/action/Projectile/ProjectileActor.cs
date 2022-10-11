using System;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 子弹表现层
    public sealed class ProjectileActor : ILifeCycle {
        private int id;
        public GameObject GameObject;
        private Vector3 sourcePosition;
        private Vector3 sourceForward;
        private MoveComponent moveComponent;

        public void Clear() {
            GameObject = null;
        }
        
        public void MoveTo(Vector3 position, Vector3 forward) {
            moveComponent.MoveTo(position, forward);
        }
        
        public void Pause() {
            
        }
        
        public void Continue() {
            
        }

        public void OnInit() { }
        
        public void OnInit(int id, GameObject gameObject, Vector3 sourcePosition, Vector3 sourceForward) {
            this.id = id;
            GameObject = gameObject;
            this.sourcePosition = sourcePosition;
            this.sourceForward = sourceForward;

            moveComponent = gameObject.GetComponent<MoveComponent>();

            if (moveComponent != null) {
                moveComponent.Init();
                moveComponent.SetLocalPosition(sourcePosition);
            } else {
                BattleLog.LogError("【ProjectileActor】没有挂载 MoveComponent ！");
            }
        }

        public void OnUpdate() {
        }

        public void OnClear() {
        }
    }
}