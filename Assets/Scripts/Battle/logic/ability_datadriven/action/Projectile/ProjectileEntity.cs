using System;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    /// <summary>
    /// 子弹数据层
    /// </summary>
    public class ProjectileEntity : AbsEntity {
        public int Id;
        private string effectName;
        private bool dodgeable;
        private float moveSpeed;
        private float durning = 3f;
        private float flyTime;
        private Vector3 flyToward;
        private AbsEntity casterEntity;
        public ProjectileType ProjectileType;
        public event Action OnProjectileHitUnitEvent;
        public event Action OnProjectileFinishEvent;
        public event Action OnProjectileDodgeEvent;
        public override void OnInit() { }

        public void OnInit(AbsEntity casterEntity, Vector3 startPoint, Quaternion startRotation, AbsEntity targetEntity, 
            AbilityRequestContext abilityRequestContext, string effectName, float moveSpeed, bool dodgeable = false) {
            this.effectName = effectName;
            this.moveSpeed = moveSpeed;
            this.dodgeable = dodgeable;
            this.casterEntity = casterEntity;
            LocalPosition = startPoint;
            LocalRotation = startRotation;
            flyTime = 0f;
            flyToward = startRotation * Vector3.up;

            if (string.IsNullOrEmpty(effectName)) {
                Debug.LogError("[GetActorAsync]子弹特效名为空！");
                return;
            }
            
        }

        public override void OnUpdate() {
            if (moveSpeed > 0) {
                LocalPosition += flyToward * moveSpeed * Time.deltaTime;
            }

            durning += Time.deltaTime;
            if (flyTime >= durning) {
                FlyEnd();
            }
        }

        public override void OnClear() {
            casterEntity = null;
            OnProjectileFinishEvent?.Invoke();
        }

        private void FlyEnd() {
            OnClear();
        }
    }
}