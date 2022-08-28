using System;
using System.Collections.Generic;
using System.Linq;
using LitJson;

namespace Battle.logic.ability_dataDriven {
    public static class AbilityConfigParse {
        public static Ability GetAbility(int id) {
            var skillItem = skillTable.Instance.GetTableItem(id);
            if(skillItem == null)
            {
                BattleLog.LogError("skill 表没有找到指定id:", id.ToString());
                return null;
            }

            var path = skillItem.config;
            var priority = skillItem.priority;
            var abilityConfig = LoadAbilityConfig(path);
            var ability = new Ability(abilityConfig);
            return ability;
        }

        private static AbilityConfig LoadAbilityConfig(string path) {
            var jsonData = LoadModule.LoadJson(path);
            if(jsonData == null) {
                BattleLog.LogError("[CreateAbility]没有找到指定json配置！", path);
                return default;
            }

            var abilityConfig = new AbilityConfig() {
                AbilityDamage = jsonData.GetIntValue("AbilityDamage"),
                AbilityManaCost = jsonData.GetIntValue("AbilityManaCost"),

                AbilityBehavior = ParseAbilityBehaviorArray(jsonData, "AbilityBehavior"),
                AbilityUnitDamageType = jsonData.GetEnumValue<AbilityUnitDamageType>("AbilityUnitDamageType"),

                AbilityCooldown = jsonData.GetFloatValue("AbilityCooldown"),
                AbilityCastPoint = jsonData.GetFloatValue("AbilityCastPoint"),
                AbilityCastRange = jsonData.GetFloatValue("AbilityCastRange"),
                AbilityChannelTime = jsonData.GetFloatValue("AbilityChannelTime"),
                AbilityChannelledManaCostPerSecond = jsonData.GetFloatValue("AbilityChannelledManaCostPerSecond"),
                AbilityDuration = jsonData.GetFloatValue("AbilityDuration"),
                AoERadius = jsonData.GetFloatValue("AoERadius"),

                AbilityCastAnimation = jsonData.GetStringValue("AbilityCastAnimation"),
                AbilityTextureName = jsonData.GetStringValue("AbilityTextureName"),

                AbilityEventMap = ParseAbilityEvents(jsonData),
            };

            return abilityConfig;
        }

        // 解析技能行为数组
        private static AbilityBehavior ParseAbilityBehaviorArray(JsonData json, string key)
        {
            var behaviorConfigs = GetJsonValue(json, key);
            if (behaviorConfigs == null) {
                return default;
            }

            AbilityBehavior behavior = 0;
            if(behaviorConfigs.IsArray)
            {
                for(var i = 0; i < behaviorConfigs.Count; i++)
                {
                    var item = behaviorConfigs[i].ToString();
                    var abilityBehavior = GetEnumValue<AbilityBehavior>(item);
                    behavior |= abilityBehavior;
                }
            }
            return behavior;
        }

        private static Dictionary<AbilityEvent, D2Event> ParseAbilityEvents(JsonData json) {
            if (json == null) {
                return null;
            }

            var eventMap = new Dictionary<AbilityEvent, D2Event>();
            foreach(var key in json.Keys)
            {
                if(Enum.IsDefined(typeof(AbilityEvent), key))
                {
                    var eventsJsonData = GetJsonValue(json, key);
                    var actions = ParseActions(eventsJsonData);
                    var d2Event = new D2Event(actions);
                    eventMap.Add((AbilityEvent)Enum.Parse(typeof(AbilityEvent), key), d2Event);
                }
            }
            return eventMap;
        }

        private static List<D2Action> ParseActions(JsonData json)
        {
            if (json == null) {
                return null;
            }

            var actions = new List<D2Action>();
            var abilityConfigParseType = typeof(AbilityConfigParse);
            foreach(var key in json.Keys) {
                var methodName = key;
                var method = abilityConfigParseType.GetMethod(methodName);
                if (method != null) {
                    method.Invoke(null, new object[0]);
                }
            }

            return actions;
        }

        #region Create Action Static Method



        #endregion

        private static Dictionary<string, ModifierData> ParseModifiers(JsonData json)
        {
            var res = new Dictionary<string, ModifierData>();
            var modifiers = GetJsonValue(json, "Modifiers");
            if(modifiers != null)
            {
                var keys = modifiers.Keys.ToList();
                for(int i = 0; i < modifiers.Count; i++)
                {
                    var modifierJson = modifiers[i];
                    var modifier = ParseModifier(modifierJson);
                    modifier.Name = keys[i];
                    res.Add(modifier.Name, modifier);
                }
            }
            return res;
        }

        private static ModifierData ParseModifier(JsonData json)
        {
            var modifier = new ModifierData();

            //modifier.Name = json; //json.key
            modifier.Duration = GetFloatValue(json, "Duration");
            modifier.ThinkInterval = GetFloatValue(json, "ThinkInterval");
            modifier.IsDebuff = GetBoolValue(json, "IsDebuff");
            modifier.IsBuff = GetBoolValue(json, "IsBuff");
            modifier.Passive = GetBoolValue(json, "IsPassive");
            modifier.IsHidden = GetBoolValue(json, "IsHidden");
            modifier.IsPurgable = GetBoolValue(json, "IsPurgable");
            // effect
            modifier.EffectName = GetStringValue(json, "EffectName");
            modifier.EffectAttachType = GetEnumValue<ModifierEffectAttachType>(json, "EffectAttachType");
            // event
            modifier.ModifierEventMap = ParseModifierEvents(json);
            // aura
            // ParseModifierAura(json, abilityData, modifier);
            // ModifierProperties
            // ParseModifierProperties(json, abilityData, modifier);
            // States
            // ParseModifierStates(json, abilityData, modifier);

            return modifier;
        }

        private static Dictionary<ModifierEvents, D2Event> ParseModifierEvents(JsonData json)
        {
            if (json == null) {
                return null;
            }

            var eventMap = new Dictionary<ModifierEvents, D2Event>();
            foreach(var key in json.Keys)
            {
                if(Enum.IsDefined(typeof(ModifierEvents), key))
                {
                    var eventsJsonData = GetJsonValue(json, key);
                    var actions = ParseActions(eventsJsonData);
                    var d2Event = new D2Event(actions);
                    var eventName = GetEnumValue<ModifierEvents>(key);
                    eventMap.Add(eventName, d2Event);
                }
            }
            return eventMap;
        }

        #region LitJson Extension

        private static JsonData GetJsonValue(JsonData json, string key)
        {
            if (json == null || string.IsNullOrEmpty(key)) {
                return null;
            }

            return json.Keys.Contains(key) ? json[key] : null;
        }

        public static int GetIntValue(this JsonData json, string key)
        {
            var res = GetJsonValue(json, key);
            return res != null ? int.Parse(res.ToString()) : -1;
        }

        public static float GetFloatValue(this JsonData json, string key)
        {
            var res = GetJsonValue(json, key);
            return res != null ? float.Parse(res.ToString()) : 0f;
        }

        public static bool GetBoolValue(this JsonData json, string key)
        {
            var res = GetJsonValue(json, key);
            if(res!=null && res.IsBoolean)
                return (bool)res;

            return false;
        }

        public static string GetStringValue(this JsonData json, string key)
        {
            var res = GetJsonValue(json, key);
            return res?.ToString();
        }

        public static T GetEnumValue<T>(this JsonData json, string key)
        {
            var str = GetStringValue(json, key);
            return GetEnumValue<T>(str);
        }

        private static T GetEnumValue<T>(string str)
        {
            if(str == null)
                return default(T);

            var value = (T)Enum.Parse(typeof(T), str.ToString());
            return value;
        }

        #endregion
    }
}