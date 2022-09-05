using System.Collections.Generic;
using Origins;

namespace Battle.logic.ability_dataDriven {
    public class DotaAction {
        public string AbilityName;
        protected DotaAction actions;
        private readonly AbilityTarget actionTarget;
        protected DotaAction(AbilityTarget actionTarget) {
            this.actionTarget = actionTarget;
        }

        public virtual void Execute(AbsEntity entity) {
            if (actionTarget.IsSingleTarget) {
                ExecuteBySingle();
            } else {
                ExecuteByMultiple();
            }
        }

        protected virtual void ExecuteBySingle(){}
        protected virtual void ExecuteByMultiple(){}

        public void SetAction(List<DotaAction> actions) {
            this.actions = this.actions;
        }
    }
}