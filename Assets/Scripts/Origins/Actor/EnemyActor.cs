using UnityEngine;

namespace Origins {
    public class EnemyActor : AbsActor {
        private EnemyEntity entity;
        public override void OnInit() {
            cacheVector2 = new Vector2();
            mTransform = transform;
            rigidBody2D = transform.GetComponent<Rigidbody2D>();
        }
        
        public override void OnUpdate() {
            MoveToTarget();
        }

        public override void OnClear() {
            
        }

        public void SetEntity(EnemyEntity value) {
            entity = value;
        }

        public void SetPositionSync(Vector2 value) {
            SetPosition(value);
            entity.Position = value;
        }

        #region Private

        private void MoveToTarget() {
            var targetEntity = EntityManager.instance.HeroEntity;
            if (targetEntity == null) {
                return;
            }

            var targetPos = targetEntity.Position;
            
        }

        #endregion
    }
}