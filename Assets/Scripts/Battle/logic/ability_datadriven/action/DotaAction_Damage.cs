using Origins;

namespace Battle.logic.ability_dataDriven {
    // 目标,类型,最小伤害/最大伤害，伤害，基于当前生命百分比伤害，基于最大生命百分比伤害
    // Target, Type, MinDamage/MaxDamage, Damage, CurrentHealthPercentBasedDamage, MaxHealthPercentBasedDamage
    public sealed class DotaAction_Damage : DotaAction {
        // Require 必须参数
        private readonly float[] damages;
        private readonly AbilityUnitDamageType abilityUnitDamageType;
        // Optional 可选参数
        private AbilityDamageFlag abilityDamageFlag;
        private float minDamage;
        private float maxDamage;
        private bool currentHealthPercentBasedDamage;
        private bool maxHealthPercentBasedDamage;

        public DotaAction_Damage(AbilityTarget abilityTarget, float[] damages, AbilityUnitDamageType abilityUnitDamageType) : base(abilityTarget) {
            this.damages = damages;
            this.abilityUnitDamageType = abilityUnitDamageType;
        }
    }
}