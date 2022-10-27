using System;
using UnityEngine;

namespace Origins {
    public class HeroActor : AbsActor {
        public HeroEntity entity;

        private Rigidbody2D rigidBody2D;
        [SerializeField] private HpSliderActor hpSliderActor = null;

        public override void OnInit() {
            cacheVector = new Vector2();
            cacheQuaternion = new Quaternion();
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

        private void SetLocalPositionSync(Vector2 value) {
            SetLocalPosition(value);
            entity.LocalPosition = value;
        }

        private void SetRotationSync(Vector3 value) {
            cacheQuaternion.SetLookRotation(value);
            SetLocalRotation(cacheQuaternion);
        }

        private void OnPlayerInput() {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");

            if (Math.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f) {
                cacheVector.x = horizontal;
                cacheVector.y = vertical;
                // rigidbody2D.MovePosition(targetPos);
                // rigidBody2D.velocity = cacheVector2;
                var targetPos = entity.LocalPosition + cacheVector * entity.MoveSpeed * Time.deltaTime;
                var targetDirection = targetPos - entity.LocalPosition;
                SetLocalPositionSync(targetPos);
                entity.LocalForward = targetDirection;
            }
        }

        #endregion
    }
}