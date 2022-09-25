using Origins;

namespace Battle.logic.ability_dataDriven {
    public class AbilityTarget {
        private int maxTargets;
        private bool isRandom;// 如果有多个目标，是否随机选择一个目标

        public bool IsSingleTarget;
        public ActionSingleTarget ActionSingleTarget;
        public AbilityUnitTargetTeam AbilityUnitTargetTeam;
        public AbilityUnitTargetType AbilityUnitTargetType;
        public AbilityUnitTargetType AbilityExcludeUnitTargetType;
        public AbilityUnitTargetFlag AbilityUnitTargetFlag;
        public AbilityUnitTargetFlag AbilityExcludeUnitTargetFlag;

        // 单体目标
        public void SetConfigSingleTarget(ActionSingleTarget value) {
            IsSingleTarget = true;
            ActionSingleTarget = value;
        }

        public AbsEntity GetSingleTarget() {
            return default;
        }

        // 范围目标
        public void SetRangeTarget() {

        }
    }
}