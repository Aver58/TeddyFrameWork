using System;
using UnityEngine;

namespace Origins {
    public sealed class RoleActor : Actor {
        private RoleEntity roleEntity;
        private Vector2 inputPos;
        private Transform mTransform;
        private Rigidbody2D rigidBody2D;

        #region Public

        public void SetPosition(Vector2 value) {
            mTransform.localPosition = value;
        }

        public override void OnUpdate() {
            if (roleEntity.RoleType == GameDefine.RoleType.Hero) {
                OnInput();
            }
        }

        public override void OnInit() {
            inputPos = new Vector2();
            mTransform = transform;
            rigidBody2D = transform.GetComponent<Rigidbody2D>();
        }

        public override void OnClear() {
            
        }

        public void SetEntity(RoleEntity value) {
            roleEntity = value;
        }
        
        #endregion

        #region Private

        private void OnInput() {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");

            if (Math.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f) {
                // rigidbody2D.velocity = inputPos;
                // rigidbody2D.MovePosition(targetPos);
                // rigidbody2D 没调试好，跑不起来
                inputPos.y = vertical;
                inputPos.x = horizontal;
                var targetPos = rigidBody2D.position + inputPos * roleEntity.MoveSpeed;
                SetPosition(targetPos);
            }
        }

        #endregion
    }
}