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

        public virtual void Execute(AbsEntity entity) {
            Debug.Log($"【DotaAction Execute】名称：{AbilityName} entity：{entity.InstanceId}");
            if (abilityTarget.IsSingleTarget) {
                var singleTarget = EntityManager.instance.GetSingleTarget();
                ExecuteBySingle(singleTarget);
            } else {
                ExecuteByMultiple();
            }
        }

        protected virtual void ExecuteBySingle(AbsEntity entity){}
        protected virtual void ExecuteByMultiple(){}

        public void SetAction(List<DotaAction> actions) {
            this.actions = actions;
        }
    }
}