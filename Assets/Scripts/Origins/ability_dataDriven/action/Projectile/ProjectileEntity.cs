using System;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    /// <summary>
    /// 子弹数据层
    /// </summary>
    public class ProjectileEntity : AbsEntity, IProjectile {
        public int Id;
        private string effectName;
        private bool dodgeable;
        private float moveSpeed;
        private float durning = 3f;
        private float flyTime;
        private Vector3 flyToward;
        private AbsEntity casterEntity;
        private AbsEntity targetEntity;

        public ProjectileType ProjectileType;
        public event Action OnProjectileHitUnitEvent;
        public event Action OnProjectileFinishEvent;
        public event Action OnProjectileDodgeEvent;
        public override void OnInit() { }

        public void OnInit(AbsEntity casterEntity, Vector3 sourcePosition, Quaternion sourceRotation, AbsEntity targetEntity,
            AbilityRequestContext abilityRequestContext, string effectName, float moveSpeed, bool dodgeable = false) {
            this.effectName = effectName;
            this.moveSpeed = moveSpeed;
            this.dodgeable = dodgeable;
            this.casterEntity = casterEntity;
            this.targetEntity = targetEntity;
            LocalPosition = sourcePosition;
            LocalRotation = sourceRotation;
            flyTime = 0f;
            flyToward = sourceRotation * Vector3.up;

            if (string.IsNullOrEmpty(effectName)) {
                Debug.LogError("[GetActorAsync]子弹特效名为空！");
                return;
            }

            GameMsg.instance.DispatchEvent(GameMsgDef.OnProjectileActorCreated, Id, effectName, sourcePosition, flyToward);
        }

        public override void OnUpdate() {

        }

        public override void OnClear() {
            casterEntity = null;
            OnProjectileFinishEvent?.Invoke();

            OnProjectileFinishEvent = null;
        }
    }
}