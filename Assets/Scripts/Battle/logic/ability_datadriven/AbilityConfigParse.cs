using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public static class AbilityConfigParse {
        public static Ability GetAbility(int id) {
            var skillMapItem = SkillMapTable.Instance.Get(id);
            if(skillMapItem == null) {
                return null;
            }

            var path = skillMapItem.config;
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

            var abilityConfig = new AbilityConfig ();
            abilityConfig.AbilityDamage = jsonData.GetFloatArrayValue("AbilityDamage");
            abilityConfig.AbilityManaCost = jsonData.GetFloatArrayValue("AbilityManaCost");
            abilityConfig.AbilityCooldowns = jsonData.GetFloatArrayValue("AbilityCooldown");
            abilityConfig.AbilityCastPoint = jsonData.GetFloatValue("AbilityCastPoint");
            abilityConfig.AbilityCastRange = jsonData.GetFloatValue("AbilityCastRange");
            abilityConfig.AbilityChannelTime = jsonData.GetFloatValue("AbilityChannelTime");
            abilityConfig.AbilityChannelledManaCostPerSecond = jsonData.GetFloatValue("AbilityChannelledManaCostPerSecond");
            abilityConfig.AbilityDuration = jsonData.GetFloatValue("AbilityDuration");
            abilityConfig.AoERadius = jsonData.GetFloatValue("AoERadius");

            abilityConfig.Name = jsonData.GetStringValue("Name");
            abilityConfig.AbilityCastAnimation = jsonData.GetStringValue("AbilityCastAnimation");
            abilityConfig.AbilityTextureName = jsonData.GetStringValue("AbilityTextureName");

            abilityConfig.AbilityUnitTargetTeam = jsonData.GetEnumValue<AbilityUnitTargetTeam>("AbilityUnitTargetTeam");
            abilityConfig.AbilityUnitTargetType = jsonData.GetEnumValue<AbilityUnitTargetType>("AbilityUnitTargetType");
            abilityConfig.AbilityUnitTargetFlag = jsonData.GetEnumValue<AbilityUnitTargetFlag>("AbilityUnitTargetFlags");
            abilityConfig.AbilityUnitDamageType = jsonData.GetEnumValue<AbilityUnitDamageType>("AbilityUnitDamageType");

            abilityConfig.AbilityBehavior = ParseAbilityBehaviorArray(jsonData, "AbilityBehavior");

            abilityConfig.AbilityEventMap = ParseAbilityEvents(jsonData, abilityConfig);

            return abilityConfig;
        }

        // 解析技能行为数组
        private static AbilityBehavior ParseAbilityBehaviorArray(JsonData json, string key)
        {
            var jsonData = GetJsonValue(json, key);
            if (jsonData == null) {
                return default;
            }

            AbilityBehavior behavior = 0;
            if(jsonData.IsArray) {
                for(var i = 0; i < jsonData.Count; i++) {
                    var item = jsonData[i];
                    var abilityBehavior = GetEnumValue<AbilityBehavior>(item.ToString());
                    behavior |= abilityBehavior;
                }
            }
            return behavior;
        }

        private static Dictionary<AbilityEvent, DotaEvent> ParseAbilityEvents(JsonData jsonData, AbilityConfig abilityConfig) {
            if (jsonData == null) {
                return null;
            }

            var eventMap = new Dictionary<AbilityEvent, DotaEvent>();
            foreach(var key in jsonData.Keys)
            {
                if(Enum.IsDefined(typeof(AbilityEvent), key))
                {
                    var eventsJsonData = GetJsonValue(jsonData, key);
                    var actions = ParseActions(eventsJsonData, abilityConfig);
                    var d2Event = new DotaEvent(actions);
                    eventMap.Add((AbilityEvent)Enum.Parse(typeof(AbilityEvent), key), d2Event);
                }
            }
            return eventMap;
        }

        #region Action

        private static List<DotaAction> ParseActions(JsonData jsonData, AbilityConfig abilityConfig) {
            if (jsonData == null) {
                return null;
            }

            var actions = new List<DotaAction>();
            var abilityConfigParseType = typeof(AbilityConfigParse);
            foreach(var key in jsonData.Keys) {
                var methodName = key;
                var method = abilityConfigParseType.GetMethod(methodName, BindingFlags.Static| BindingFlags.NonPublic);
                if (method != null) {
                    var actionJsonData = jsonData[key];
                    var abilityTarget = ParseActionTarget(actionJsonData, abilityConfig);
                    var action = method.Invoke(null, new object[] {actionJsonData, abilityTarget, abilityConfig});
                    if (action is DotaAction dotaAction) {
                        // 嵌套Action
                        var nestActions = ParseActions(actionJsonData, abilityConfig);
                        if (nestActions != null && nestActions.Count > 0 ) {
                            dotaAction.SetAction(nestActions);
                        }
                        actions.Add(dotaAction);
                    }
                }
            }

            return actions;
        }

        private static AbilityTarget ParseActionTarget(JsonData actionJsonData, AbilityConfig abilityConfig) {
            if (actionJsonData == null) {
                return null;
            }

            if (!actionJsonData.ContainsKey("Target")) {
                BattleLog.LogError("【技能对象解析】{0}没有配置Target字段！", abilityConfig.Name);
                return null;
            }

            var abilityTarget = new AbilityTarget();
            if (actionJsonData.IsArray) {
                // var aoeAreaJsonData = GetJsonValue(json, "AoeArea");
                // if (aoeAreaJsonData == null) {
                //     BattleLog.LogError("技能[%s]中Target配置是区域目标配置，但是未配置AoeArea或者配置错误", abilityData.configFileName);
                //     return null;
                // }
                //
                // var targetCenter = GetEnumValue<ActionMultipleTargets>(json, "Center");
                // var areaType = GetStringValue(aoeAreaJsonData, "AreaType");
                // var abilityAreaDamageType = GetEnumValue<AOEType>(areaType);
                //
                // switch (abilityAreaDamageType) {
                //     case AOEType.Radius:
                //         float radius = (int) GetJsonValue(aoeAreaJsonData, "Radius");
                //         actionTarget.SetRadiusAoe(targetCenter, radius);
                //         break;
                //     case AOEType.Line:
                //         JsonData lineJsonData = (int) GetJsonValue(aoeAreaJsonData, "Line");
                //         if (lineJsonData == null) {
                //             BattleLog.LogError("技能[{0}]中Target配置是区域目标配置，但是未配置Line", abilityData.configFileName);
                //             return null;
                //         }
                //
                //         var length = (float) GetJsonValue(lineJsonData, "Length");
                //         var thickness = (float) GetJsonValue(lineJsonData, "Thickness");
                //         actionTarget.SetLineAoe(targetCenter, length, thickness);
                //         break;
                //     case AOEType.Sector:
                //         JsonData sectorJsonData = GetJsonValue(json, "Sector");
                //         if (sectorJsonData == null) {
                //             BattleLog.LogError("技能[{0}]中Target配置是区域目标配置，但是未配置Sector", abilityData.configFileName);
                //             return null;
                //         }
                //
                //         var sectorRadius = (float) GetJsonValue(sectorJsonData, "Radius");
                //         var angle = (float) GetJsonValue(sectorJsonData, "Angle");
                //         actionTarget.SetSectorAoe(targetCenter, sectorRadius, angle);
                //         break;
                //     default:
                //         break;
                // }
                //
                // var teamJsonData = GetStringValue(json, "Teams");
                // var abilityTargetTeam = GetEnumValue<AbilityUnitTargetTeam>(teamJsonData);
                // actionTarget.SetTargetTeam(abilityTargetTeam);
            }
            else {
                var actionSingTarget = actionJsonData.GetEnumValue<ActionSingTarget>("Target");
                abilityTarget.SetConfigSingleTarget(actionSingTarget);
            }

            return abilityTarget;
        }

        #region Create Action Static Method

        private static DotaAction Damage(JsonData jsonData, AbilityTarget abilityTarget, AbilityConfig abilityConfig) {
            var damageType = jsonData.GetEnumValue<AbilityUnitDamageType>("Type");
            var damages = jsonData.GetFloatArrayValue("Damage");
            var action = new DotaAction_Damage(abilityTarget, damages, damageType) {AbilityName = abilityConfig.Name};
            return action;
        }

        private static DotaAction LinearProjectile(JsonData jsonData, AbilityTarget abilityTarget, AbilityConfig abilityConfig) {
            var effectName = jsonData.GetStringValue("EffectName");
            var sourceAttachment = jsonData.GetStringValue("SourceAttachment");
            var dodgeable = jsonData.GetBoolValue("Dodgeable");
            var moveSpeed = jsonData.GetFloatValue("MoveSpeed");
            var providesVision = jsonData.GetBoolValue("ProvidesVision");
            var visionRadius = jsonData.GetFloatValue("VisionRadius");

            var action = new DotaAction_LinearProjectile(abilityTarget, effectName, sourceAttachment, dodgeable, moveSpeed){AbilityName = abilityConfig.Name};
            action.SetVisionParam(providesVision, visionRadius);
            return action;
        }

        #endregion

        #endregion


        #region Modifier

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
            // modifier.ModifierEventMap = ParseModifierEvents(json);
            // aura
            // ParseModifierAura(json, abilityData, modifier);
            // ModifierProperties
            // ParseModifierProperties(json, abilityData, modifier);
            // States
            // ParseModifierStates(json, abilityData, modifier);

            return modifier;
        }

        private static Dictionary<ModifierEvents, DotaEvent> ParseModifierEvents(JsonData jsonData, AbilityConfig abilityConfig) {
            if (jsonData == null) {
                return null;
            }

            var eventMap = new Dictionary<ModifierEvents, DotaEvent>();
            foreach(var key in jsonData.Keys) {
                if(Enum.IsDefined(typeof(ModifierEvents), key)) {
                    var eventsJsonData = GetJsonValue(jsonData, key);
                    var actions = ParseActions(eventsJsonData, abilityConfig);
                    var d2Event = new DotaEvent(actions);
                    var eventName = GetEnumValue<ModifierEvents>(key);
                    eventMap.Add(eventName, d2Event);
                }
            }
            return eventMap;
        }


        #endregion

        #region LitJson Extension

        private static JsonData GetJsonValue(JsonData json, string key) {
            if (json == null || string.IsNullOrEmpty(key)) {
                return null;
            }

            return json.Keys.Contains(key) ? json[key] : null;
        }

        private static int GetIntValue(this JsonData json, string key) {
            var res = GetJsonValue(json, key);
            return res != null ? int.Parse(res.ToString()) : -1;
        }

        private static float[] GetFloatArrayValue(this JsonData json, string key) {
            var res = GetJsonValue(json, key);
            if (res != null && res.IsArray) {
                var array = new float[res.Count];
                for (int i = 0; i < res.Count; i++) {
                    var value = res[i];
                    array[i] = float.Parse(value.ToString());
                }

                return array;
            }
            return null;
        }

        private static float GetFloatValue(this JsonData json, string key) {
            var res = GetJsonValue(json, key);
            return res != null ? float.Parse(res.ToString()) : 0f;
        }

        private static bool GetBoolValue(this JsonData json, string key) {
            var res = GetJsonValue(json, key);
            if(res!=null && res.IsBoolean)
                return (bool)res;

            return false;
        }

        private static string GetStringValue(this JsonData json, string key) {
            var res = GetJsonValue(json, key);
            return res?.ToString();
        }

        private static T GetEnumValue<T>(this JsonData json, string key) {
            var str = GetStringValue(json, key);
            return str == null? default : GetEnumValue<T>(str);
        }

        private static T GetEnumValue<T>(string str) {
            if (string.IsNullOrEmpty(str)) {
                return default;
            }

            try {
                var value = Enum.Parse(typeof(T), str);
                return (T)value;
            } catch (Exception e) {
                Debug.LogError($"【枚举解析失败】{typeof(T).Name}没有找到指定类型:{str}  \r\nException:{e.Message}");
                throw;
            }
        }

        #endregion
    }
}