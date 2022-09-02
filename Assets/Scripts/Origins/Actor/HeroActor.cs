using System;
using UnityEngine;

namespace Origins {
    public class HeroActor : AbsActor {
        private HeroEntity entity;

        public override void OnInit() {
            cacheVector2 = new Vector2();
            mTransform = transform;
            rigidBody2D = transform.GetComponent<Rigidbody2D>();
        }

        public override void OnClear() {
            
        }

        public override void OnUpdate() {
            OnInput();
        }
        
        public void SetEntity(HeroEntity value) {
            entity = value;
        }

        private void SetPositionSync(Vector2 value) {
            SetPosition(value);
            entity.Position = value;
        }
        
        private void OnInput() {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");

            if (Math.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f) {
                // rigidbody2D.velocity = inputPos;
                // rigidbody2D.MovePosition(targetPos);
                // rigidbody2D 没调试好，跑不起来
                cacheVector2.x = horizontal;
                cacheVector2.y = vertical;
                var targetPos = rigidBody2D.position + cacheVector2 * entity.MoveSpeed;
                SetPositionSync(targetPos);
            }
        }
    }
}