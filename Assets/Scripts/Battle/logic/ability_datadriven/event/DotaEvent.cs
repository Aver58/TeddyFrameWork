using System.Collections.Generic;
using Origins;

namespace Battle.logic.ability_dataDriven {
    public class DotaEvent {
        private readonly List<DotaAction> actions;
        public DotaEvent(List<DotaAction> actions) {
            this.actions = actions;
        }

        public void Execute(AbsEntity heroEntity) {
            BattleLog.Log("【D2Event】Execute {0}，source：{1}", GetType().Name, heroEntity);
            for (int i = 0; i < actions.Count; i++) {
                var action = actions[i];
                action.Execute(heroEntity);
            }
        }
    }
}