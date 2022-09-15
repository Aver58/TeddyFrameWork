using System.Collections.Generic;
using Origins;

namespace Battle.logic.ability_dataDriven {
    public class DotaAction {
        public string AbilityName;
        protected DotaAction actions;
        protected readonly AbilityTarget abilityTarget;
        protected DotaAction(AbilityTarget abilityTarget) {
            this.abilityTarget = abilityTarget;
        }

        public virtual void Execute(AbsEntity entity) {
            if (abilityTarget.IsSingleTarget) {
                ExecuteBySingle(entity);
            } else {
                ExecuteByMultiple();
            }
        }

        protected virtual void ExecuteBySingle(AbsEntity entity){}
        protected virtual void ExecuteByMultiple(){}

        public void SetAction(List<DotaAction> actions) {
            this.actions = this.actions;
        }
    }
}