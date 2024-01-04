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

        public ProjectileEntity() {
            InstanceId = ProjectileManager.Instance.AutoIndex++;
        }
        
        public virtual void OnInit(AbsEntity casterEntity, AbsEntity targetEntity, Vector2 sourcePosition, Vector2 sourceForward,
            AbilityRequestContext abilityRequestContext, string effectName) {
            this.effectName = effectName;
            this.casterEntity = casterEntity;
            this.targetEntity = targetEntity;
            LocalPosition = sourcePosition;
            LocalForward = sourceForward;

            GameMsg.Instance.DispatchEvent(GameMsgDef.OnProjectileActorCreated, InstanceId, effectName, sourcePosition, sourceForward);
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