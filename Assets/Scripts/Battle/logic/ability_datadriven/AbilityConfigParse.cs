namespace Battle.logic.ability_dataDriven {
    public static class AbilityConfigParse {
        public static Ability GetAbility(int skillId) {
            var skillItem = skillTable.Instance.GetTableItem(skillId);
            if(skillItem == null)
            {
                BattleLog.LogError("skill 表没有找到指定id:", skillId.ToString());
                return null;
            }

            var path = skillItem.config;
            var abilityConfig = LoadAbilityConfig(path);
            var ability = new Ability(abilityConfig);
            return ability;
        }

        private static AbilityConfig LoadAbilityConfig(string path) {
            var jsonHashTable = LoadModule.GetJsonHashTable(path);
            if(jsonHashTable == null) {
                BattleLog.LogError("[CreateAbility]没有找到指定json配置！", path);
                return default;
            }

            var abilityConfig = new AbilityConfig() {
                AbilityDamage = jsonHashTable.ContainsKey("AbilityDamage")? int.Parse(jsonHashTable["AbilityDamage"].ToString()) : 0,

            };

            return abilityConfig;
        }

        private static int GetIntValue() {
            return -1;
        }
    }
}