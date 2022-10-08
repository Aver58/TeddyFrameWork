using System;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    /// <summary>
    /// 子弹数据层
    /// </summary>
    public class ProjectileEntity : AbsEntity {
        private string effectName;
        private AbsEntity casterEntity;
        private AbsEntity targetEntity;
        public ProjectileType ProjectileType;
        public event Action OnProjectileHitUnitEvent;
        public event Action OnProjectileFinishEvent;
        public event Action OnProjectileDodgeEvent;

        public ProjectileEntity() {
            InstanceId = ProjectileManager.instance.AutoIndex++;
        }
        
        public void OnInit(AbsEntity casterEntity, AbsEntity targetEntity, Vector3 sourcePosition, Vector3 sourceForward,
            AbilityRequestContext abilityRequestContext, string effectName) {
            this.effectName = effectName;
            this.casterEntity = casterEntity;
            this.targetEntity = targetEntity;
            Position = sourcePosition;
            Forward = sourceForward;

            GameMsg.instance.DispatchEvent(GameMsgDef.OnProjectileActorCreated, InstanceId, effectName, sourcePosition, sourceForward);
        }

        public override void OnInit() {
            
        }

        public override void OnUpdate() {

        }

        public override void OnClear() {
            casterEntity = null;
            targetEntity = null;
        }
    }
}