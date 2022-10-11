using System;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class MoveComponent : MonoBehaviour {
        private bool isMoving;
        private Vector3 targetPosition;
        private float movedTime = 0f;
        private float logicDeltaTime = 0.2f;   // 逻辑间隔时间

        
        public void Init() {
            isMoving = false;
        }

        private void LateUpdate() {
            if (isMoving) {
                movedTime += Time.deltaTime;

                float radio = movedTime / logicDeltaTime;
                transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, radio);
            }
            
            if (movedTime >= logicDeltaTime) {
                isMoving = false;
            }
        }

        public void SetPosition(Vector3 value) {
            transform.position = value;
        }
        
        public void SetLocalPosition(Vector3 value) {
            transform.localPosition = value;
        }
        
        public void SetForward(Vector3 value) {
            transform.forward = value;
        }

        public void SetLocalForward(Vector3 value) {
            
        }

        public void MoveTo(Vector3 position, Vector3 forward) {
            isMoving = true;
            targetPosition = position;
            
            SetLocalForward(forward);
        }
    }
}