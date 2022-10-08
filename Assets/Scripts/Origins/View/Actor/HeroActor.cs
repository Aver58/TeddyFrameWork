using System;
using UnityEngine;

namespace Origins {
    public class HeroActor : AbsActor {
        public HeroEntity entity;

        private Rigidbody2D rigidBody2D;
        [SerializeField] private HpSliderActor hpSliderActor = null;

        public override void OnInit() {
            cacheVector3 = new Vector3();
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

        private void SetPositionSync(Vector3 value) {
            SetPosition(value);
            entity.Position = value;
        }

        private void OnPlayerInput() {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");

            if (Math.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f) {
                cacheVector3.x = horizontal;
                cacheVector3.y = vertical;
                // rigidbody2D.MovePosition(targetPos);
                // rigidbody2D 没调试好，跑不起来
                // rigidBody2D.velocity = cacheVector2;
                var targetPos = entity.Position + cacheVector3 * entity.MoveSpeed * Time.deltaTime;
                var targetDirection = targetPos - entity.Position;
                SetPositionSync(targetPos);
                entity.Forward = targetDirection;
            }
        }

        #endregion
    }
}