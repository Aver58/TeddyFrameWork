using System;
using UnityEngine;

namespace Origins {
    public class RoleActor : Actor {
        protected RoleEntity roleEntity;

        #region Public

        public override void OnUpdate() {
            OnInput();
        }

        public override void OnInit() {
            cacheVector2 = new Vector2();
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
                // rigidbody2D.velocity = cacheVector2;
                // rigidbody2D.MovePosition(targetPos);
                // rigidbody2D 没调试好，跑不起来
                cacheVector2.y = vertical;
                cacheVector2.x = horizontal;
                var targetPos = rigidBody2D.position + cacheVector2 * roleEntity.MoveSpeed;
                SetPosition(targetPos);
            }
        }

        #endregion
    }
}