using System;
using UnityEngine;

namespace Origins {
    public class HeroActor : RoleActor {
        private Vector2 inputPos;

        public override void OnInit() {
            inputPos = new Vector2();
        }

        public override void OnUpdate() {
            OnInput();
        }

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
    }
}