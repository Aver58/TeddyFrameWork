using System;
using UnityEngine;

namespace Origins {
    public class HeroActor : AbsActor {
        public HeroEntity entity;

        [SerializeField] private HpSliderActor hpSliderActor = null;

        public override void OnInit() {
            cacheVector2 = new Vector2();
            mTransform = transform;
            rigidBody2D = transform.GetComponent<Rigidbody2D>();

            if (hpSliderActor) {
                hpSliderActor.SetValue(1);
            }
        }

        public override void OnClear() {
            
        }

        public override void OnUpdate() {
            OnPlayerInput();
        }
        
        public void SetEntity(HeroEntity value) {
            entity = value;
            InstanceId = value.InstanceId;
        }

        public override void BeAttack(int downHp) {
            entity.Hp -= downHp;

            var sliderValue = entity.Hp / entity.MaxHp;
            hpSliderActor.SetValue(sliderValue);
        }

        #region Private

        private void SetPositionSync(Vector2 value) {
            SetPosition(value);
            entity.Position = value;
        }

        private void OnPlayerInput() {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");

            if (Math.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f) {
                cacheVector2.x = horizontal;
                cacheVector2.y = vertical;
                // rigidbody2D.MovePosition(targetPos);
                // rigidbody2D 没调试好，跑不起来
                // rigidBody2D.velocity = cacheVector2;
                var targetPos = entity.Position + cacheVector2 * entity.MoveSpeed * Time.deltaTime;
                SetPositionSync(targetPos);
            }
        }

        #endregion
    }
}