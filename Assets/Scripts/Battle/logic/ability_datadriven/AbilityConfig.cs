using System.Collections.Generic;

namespace Battle.logic.ability_dataDriven {
    public struct AbilityConfig {
        public int AbilityDamage;
        public int AbilityManaCost;

        public AbilityBehavior AbilityBehavior;
        public AbilityUnitDamageType AbilityUnitDamageType;

        public float AbilityCooldown;
        public float AbilityCastPoint;
        public float AbilityCastRange;
        public float AbilityChannelTime;
        public float AbilityChannelledManaCostPerSecond;
        public float AbilityDuration;
        public float AoERadius;//需要"AbilityBehavior" "DOTA_ABILITY_BEHAVIOR_AOE" (适用于POINT和/或UNIT_TARGET)

        public string AbilityCastAnimation;
        public string AbilityTextureName;

        public Dictionary<AbilityEvent, D2Event> AbilityEventMap;
    }
}