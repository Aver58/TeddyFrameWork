using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 子弹表现层
    public class ProjectileActor : MonoBehaviour {
        private bool dodgeable;
        private ProjectileEntity entity;

        public delegate void OnProjectileHitUnit();
        public delegate void OnProjectileFinish();
        public delegate void OnProjectileDodge();

        public event OnProjectileHitUnit OnProjectileHitUnitEvent;
        public event OnProjectileFinish OnProjectileFinishEvent;
        public event OnProjectileDodge OnProjectileDodgeEvent;

        public virtual void OnUpdate() {
            transform.position = entity.Position;
        }

        public virtual void OnClear() {
            OnProjectileFinishEvent?.Invoke();
        }

        public void SetEntity(ProjectileEntity entity) {
            this.entity = entity;
        }

        protected void OnTriggerEnter2D(Collider2D other) {
            if (other) {
                // var heroActor = other.GetComponent<Origins.HeroActor>();
                // if (heroActor != null) {
                //     if (dodgeable) {
                //         // 计算闪避
                //         OnProjectileDodgeEvent?.Invoke();
                //     } else {
                //         OnProjectileHitUnitEvent?.Invoke();
                //     }
                // }

                var enemyActor = other.GetComponent<EnemyActor>();
                if (enemyActor != null) {
                    if (dodgeable) {
                        // 计算闪避
                        OnProjectileDodgeEvent?.Invoke();
                    } else {
                        OnProjectileHitUnitEvent?.Invoke();
                    }
                }
            }
        }
    }
}