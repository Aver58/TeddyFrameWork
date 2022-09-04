using System.Collections.Generic;

namespace Battle.logic.ability_dataDriven {
    public struct AbilityConfig {
        public float AbilityCooldown;
        public float AbilityCastPoint;
        public float AbilityCastRange;
        public float AbilityChannelTime;
        public float AbilityChannelledManaCostPerSecond;
        public float AbilityDuration;
        public float AoERadius; //需要"AbilityBehavior" "DOTA_ABILITY_BEHAVIOR_AOE" (适用于POINT和/或UNIT_TARGET)

        public float[] AbilityDamage;
        public float[] AbilityManaCost;

        public string AbilityCastAnimation;
        public string AbilityTextureName;

        public AbilityUnitTargetTeam AbilityUnitTargetTeam;
        public AbilityUnitTargetType AbilityUnitTargetType;
        public AbilityUnitTargetFlags AbilityUnitTargetFlags;
        public AbilityUnitDamageType AbilityUnitDamageType;

        public AbilityBehavior AbilityBehavior;
        public Dictionary<AbilityEvent, D2Event> AbilityEventMap;
    }
}