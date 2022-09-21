using UnityEngine;

namespace Origins {
    public class EnemyActor : AbsActor {
        public EnemyEntity entity;
        
        private Rigidbody2D rigidBody2D;
        private float attackCd;

        public override void OnInit() {
            cacheVector3 = new Vector2();
            mTransform = transform;
            rigidBody2D = transform.GetComponent<Rigidbody2D>();

            attackCd = 0;
        }
        
        public override void OnUpdate() {
            MoveToTarget();

            if (attackCd > 0) {
                attackCd--;
            }
        }

        public override void OnClear() {
            
        }

        public void SetEntity(EnemyEntity value) {
            entity = value;
            InstanceId = value.InstanceId;
        }

        public void SetPositionSync(Vector2 value) {
            SetPosition(value);
            entity.LocalPosition = value;
        }

        #region Private

        private void MoveToTarget() {
            var targetEntity = EntityManager.instance.HeroEntity;
            if (targetEntity == null) {
                return;
            }

            var targetPos = targetEntity.LocalPosition;
            var targetVector = targetPos - entity.LocalPosition;
            var newPos = entity.LocalPosition + targetVector.normalized * entity.MoveSpeed * Time.deltaTime;
            SetPositionSync(newPos);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other) {
                var heroActor = other.GetComponent<HeroActor>();
                if (heroActor) {
                    if (attackCd <= 0) {
                        var downHp = entity.PhysicsAttack- heroActor.entity.Defense;
                        heroActor.BeAttack(downHp);

                        attackCd = entity.AttackCooldown;
                    }
                }
            }
        }

        #endregion
    }
}