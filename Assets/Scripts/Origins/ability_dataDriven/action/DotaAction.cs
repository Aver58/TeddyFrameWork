using System.Collections.Generic;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class DotaAction {
        public string AbilityName;
        protected List<DotaAction> actions;
        protected readonly AbilityTarget abilityTarget;
        protected DotaAction(AbilityTarget abilityTarget) {
            this.abilityTarget = abilityTarget;
        }

        public virtual void Execute(AbsEntity entity, AbilityRequestContext abilityRequestContext) {
            Debug.Log($"【DotaAction Execute】名称：{AbilityName} entity：{entity.InstanceId}");
            if (abilityTarget.IsSingleTarget) {
                var singleTarget = EntityManager.instance.GetSingleTarget(entity, abilityTarget, abilityRequestContext);
                ExecuteByUnit(entity, singleTarget, abilityRequestContext);
            } else {
                ExecuteByPoint();
            }
        }

        protected virtual void ExecuteByUnit(AbsEntity casterEntity, AbsEntity targetEntity, AbilityRequestContext abilityRequestContext){}
        protected virtual void ExecuteByPoint(){}

        public void SetAction(List<DotaAction> actions) {
            this.actions = actions;
        }
    }
}