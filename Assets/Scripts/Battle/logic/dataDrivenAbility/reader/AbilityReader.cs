#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityReader.cs
 Author:      Zeng Zhiwei
 Time:        2020\6\17 星期三 23:00:19
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

using LitJson;

public static class AbilityReader
{

    #region Json Parse

    private static JsonData GetJsonValue(JsonData json, string key)
    {
        if(json == null || string.IsNullOrEmpty(key))
            return null;

        if(json.Keys.Contains(key))
            return json[key];

        return null;
    }

    private static float GetIntValue(JsonData json, string key)
    {
        var res = GetJsonValue(json, key);
        return res != null ? (int)res : -1;
    }

    private static float GetFloatValue(JsonData json, string key)
    {
        var res = GetJsonValue(json, key);
        return res != null ? float.Parse(res.ToString()) : 0f;
    }

    private static bool GetBoolValue(JsonData json, string key)
    {
        var res = GetJsonValue(json, key);
        if(res!=null && res.IsBoolean)
            return (bool)res;

        return false;
    }

    private static string GetStringValue(JsonData json, string key)
    {
        var res = GetJsonValue(json, key);
        return res != null ? res.ToString() : null;
    }

    private static T GetEnumValue<T>(JsonData json, string key)
    {
        string str = GetStringValue(json, key);
        return GetEnumValue<T>(str);
    }

    private static T GetEnumValue<T>(string str)
    {
        if(str == null)
            return default(T);

        T value = (T)Enum.Parse(typeof(T), str.ToString());
        return value;
    }

    #endregion

    /// <summary>
    /// 解析技能行为数组
    /// </summary>
    private static AbilityBehavior ParseAbilityBehaviorArray(JsonData json, string key)
    {
        var behaviorConfigs = GetJsonValue(json, key);
        if(behaviorConfigs == null)
            return 0;
        AbilityBehavior behavior = 0;
        if(behaviorConfigs.IsArray)
        {
            for(int i = 0; i < behaviorConfigs.Count; i++)
            {
                string item = behaviorConfigs[i].ToString();
                var abilityBehavior = GetEnumValue<AbilityBehavior>(item);
                behavior |= abilityBehavior;
            }
        }
        return behavior;
    }

    #region Action

    #region Action Map

    public static Dictionary<string, Func<JsonData, ActionTarget, AbilityData, D2Action>> AbilityActionCreateMap = new Dictionary<string, Func<JsonData, ActionTarget, AbilityData, D2Action>>
    {
        { "IsHit" , CreateIsHitAction},
        { "ChangeEnergy" , CreateChangeEnergyAction},
        { "Damage" , CreateDamageAction},
        { "Heal" , CreateHealAction},
        { "ApplyModifier" , CreateApplyModifierAction},
    };

    private static D2Action CreateIsHitAction(JsonData json, ActionTarget actionTarget, AbilityData abilityData)
    {
        var onSuccessActionConfig = GetJsonValue(json, "OnSuccess");
        if(onSuccessActionConfig == null)
        {
            BattleLog.LogError("技能[%s]中IsHit未找到key[OnSuccess]的配置", abilityData.configFileName);
            return null;
        }
        var successActions = ParseActions(onSuccessActionConfig, abilityData);

        var d2Action_IsHit = new D2Action_IsHit(successActions, actionTarget);
        return d2Action_IsHit;
    }

    private static D2Action CreateChangeEnergyAction(JsonData json, ActionTarget actionTarget, AbilityData abilityData)
    {
        var energyParams = GetJsonValue(json, "EnergyParams");
        if(energyParams == null)
        {
            BattleLog.LogError("技能[{0}]中ChangeEnergy中未找到key[EnergyParams]的配置", abilityData.configFileName);
            return null;
        }
        if(!energyParams.IsArray)
        {
            BattleLog.LogError("技能[{0}]中ChangeEnergy中key[EnergyParams]的配置必须是数组类型", abilityData.configFileName);
            return null;
        }

        var d2Action_ChangeEnergy = new D2Action_ChangeEnergy(energyParams, actionTarget);
        return d2Action_ChangeEnergy;
    }

    /*
    "Damage": 
      {
          "Target":"TARGET",
          "DamageFlags" : ["DAMAGE_FLAG_CRIT","DAMAGE_FLAG_LIFELINK"],
          "DamageType" : "DAMAGE_TYPE_PHYSICAL",
          "ValueSource":
          {
            "ValueBasicParams":[0, 0],
            "ValueAdditionParams":
            [
             {
              "ValueSourceType":"SOURCE_TYPE_PHYSICAL_ATTACK",
              "ValueSourceParams":[27500, 0]
             }
            ]
          }
          "OnSuccess":
          {
              "FireSound":
              {
                  "SoundType":"SOUND_NORMAL",
                  "SoundName":99082
              }
          }
      }*/
    private static D2Action CreateDamageAction(JsonData json, ActionTarget actionTarget, AbilityData abilityData)
    {
        var damageTypeConfig = GetStringValue(json, "DamageType");
        if(damageTypeConfig == null)
            return null;

        var damageType = GetEnumValue<AbilityDamageType>(damageTypeConfig);
        var damageFlagConfig = GetJsonValue(json, "DamageFlags");
        if(damageFlagConfig == null)
            return null;

        AbilityDamageFlag damageFlag = ParseDamageFlagArray(damageFlagConfig);

        var damageValueSource = ParseValueSource(json, abilityData);
        var dealDamageSuccessActionConfig = GetJsonValue(json, "OnSuccess");
        var actions = ParseActions(dealDamageSuccessActionConfig, abilityData);
        var d2Action_Damage = new D2Action_Damage(damageType, damageFlag, damageValueSource, actions, actionTarget);
        return d2Action_Damage;
    }

    /*
           "Heal": {
              "Target": "CASTER",

              "HealFlags": [ "HEAL_FLAG_NONE" ],
              "ValueSource": {
                "ValueBasicParams": [ 8, 4 ],
                "ValueAdditionParams": [
                  {
                    "ValueSourceType": "SOURCE_TYPE_PHYSICAL_ATTACK",
                    "ValueSourceParams": [ 0, 0 ]
                  }
                ]
              }
            }
     */
    private static D2Action CreateHealAction(JsonData json, ActionTarget actionTarget, AbilityData abilityData)
    {
        var flagConfig = GetJsonValue(json, "HealFlags");
        if(flagConfig == null)
            return null;

        var healFlag = ParseHealFlagArray(flagConfig);

        var valueSource = ParseValueSource(json, abilityData);

        var d2Action_heal = new D2Action_Heal(healFlag, valueSource, actionTarget);
        return d2Action_heal;
    }
    
    /*
        "ApplyModifier":
         {
             "Target":"TARGET",
             "ModifierName":"dunji"
         }
         */
    private static D2Action CreateApplyModifierAction(JsonData json, ActionTarget actionTarget, AbilityData abilityData)
    {
        var modifierName = GetStringValue(json, "ModifierName");
        if(string.IsNullOrEmpty(modifierName))
        {
            BattleLog.LogError("技能[{0}]中ApplyModifier未找到key[ModifierName]的配置", abilityData.configFileName);
            return null;
        }

        var applyModifier = new D2Action_ApplyModifier(modifierName, actionTarget);
        return applyModifier;
    }
    #endregion

    private static AbilityDamageFlag ParseDamageFlagArray(JsonData json)
    {
        var res = AbilityDamageFlag.DAMAGE_FLAG_NONE;
        if(json == null)
            return res;
        if(json.IsArray)
        {
            for(int i = 0; i < json.Count; i++)
            {
                string item = json[i].ToString();
                var damageFlag = GetEnumValue<AbilityDamageFlag>(item);
                res |= damageFlag;
            }
        }
        return res;
    }

    private static AbilityHealFlag ParseHealFlagArray(JsonData json)
    {
        var res = AbilityHealFlag.HEAL_FLAG_NONE;
        if(json == null)
            return res;
        if(json.IsArray)
        {
            for(int i = 0; i < json.Count; i++)
            {
                string item = json[i].ToString();
                var flag = GetEnumValue<AbilityHealFlag>(item);
                res |= flag;
            }
        }
        return res;
    }

    private static AbilityValueSource ParseValueSource(JsonData json, AbilityData abilityData)
    {
        var abilityValueSource = new AbilityValueSource();
        var valueSourceJsonData = GetJsonValue(json, "ValueSource");
        if(valueSourceJsonData == null)
            return abilityValueSource;

        var valueBasicParamsConfig = GetJsonValue(valueSourceJsonData, "ValueBasicParams");
        if(valueBasicParamsConfig == null)
            return abilityValueSource;

        if(!valueBasicParamsConfig.IsArray)
        {
            BattleLog.LogError("技能[%s]中的[ValueSource]的[ValueBasicParams]的配置必须是数组类型", abilityData.configFileName);
        }
        else
        {
            var baseJsonData = valueBasicParamsConfig[0].ToString();
            var baseGrowJsonData = valueBasicParamsConfig[1].ToString();
            var baseValue = string.IsNullOrEmpty(baseJsonData) ? 0f : float.Parse(baseJsonData);
            var baseValueGrow = string.IsNullOrEmpty(baseGrowJsonData) ? 0f : float.Parse(baseGrowJsonData);
            abilityValueSource.baseValue = baseValue;
            abilityValueSource.baseValueGrow = baseValueGrow;
        }
        //todo 附加值
        var valueAdditionParamsConfig = GetJsonValue(valueSourceJsonData, "ValueAdditionParams");
        if(valueAdditionParamsConfig != null)
        {
            if(valueAdditionParamsConfig.IsArray)
            {
                for(int i = 0; i < valueAdditionParamsConfig.Count; i++)
                {
                    var additionValueConfig = valueAdditionParamsConfig[i];
                    var valueSourceType = GetEnumValue<AbilityValueSourceType>(additionValueConfig, "ValueSourceType");
                    var valueSourceParamsConfig = GetJsonValue(additionValueConfig, "ValueSourceParams");
                    if(valueSourceParamsConfig != null)
                    {
                        if(valueSourceParamsConfig.IsArray)
                        {
                            int sourceValueCoefficient = valueSourceParamsConfig.Count >= 1 ? (int)valueSourceParamsConfig[0] : 0;
                            int sourceValueCoefficientGrow = valueSourceParamsConfig.Count >= 2 ? (int)valueSourceParamsConfig[1] : 0;
                            int limitBasicValue = valueSourceParamsConfig.Count >= 3 ? (int)valueSourceParamsConfig[2] : 0;
                            int limitBasicValueGrow = valueSourceParamsConfig.Count >= 4 ? (int)valueSourceParamsConfig[3] : 0;
                            abilityValueSource.AddAdditionValueParams(valueSourceType,
                                sourceValueCoefficient, sourceValueCoefficientGrow, limitBasicValue, limitBasicValueGrow);
                        }
                        else
                            BattleLog.LogError("技能[%s]中的[ValueSource]的[ValueSourceParams]的配置必须是数组类型", abilityData.configFileName);
                    }
                }
            }
            else
                BattleLog.LogError("技能[%s]中的[ValueSource]的[ValueAdditionParams]的配置必须是数组类型", abilityData.configFileName);
        }

        return abilityValueSource;
    }

    // 大招技能指示器解析，这部分有点冗余，是自己抽出来的，不是dota源解析，那dota是怎么实现技能指示器的
    private static ActionTarget ParseAbilityRange(JsonData json, AbilityData abilityData)
    {
        if(json == null)
            return null;

        var actionTarget = new ActionTarget();
        var targetTeam = GetEnumValue<MultipleTargetsTeam>(json, "AbilityUnitTargetTeam");

        actionTarget.SetTargetTeam(targetTeam);

        var abilityBehavior = abilityData.abilityBehavior;
        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_DIRECTIONAL) != 0)
        {
            var lineJsonData = GetJsonValue(json, "AbilityAoeLine");
            if(lineJsonData == null)
            {
                BattleLog.LogError("技能[{0}]行为是ABILITY_BEHAVIOR_LINE_AOE，未找到AbilityAoeLine配置", abilityData.configFileName);
                return null;
            }

            var length = (float)GetJsonValue(lineJsonData, "Length");
            var thickness = (float)GetJsonValue(lineJsonData, "Thickness");
            actionTarget.SetLineAoe(ActionMultipleTargetsCenter.CASTER, length, thickness);
        }

        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_SECTOR_AOE) != 0)
        {
            var sectorJsonData = GetJsonValue(json, "AbilityAoeSector");
            if(sectorJsonData == null)
            {
                BattleLog.LogError("技能[{0}]行为是ABILITY_BEHAVIOR_SECTOR_AOE，未找到AbilityAoeSector配置", abilityData.configFileName);
                return null;
            }

            var sectorRadius = (float)GetJsonValue(sectorJsonData, "Radius");
            var angle = (float)GetJsonValue(sectorJsonData, "Angle");
            actionTarget.SetSectorAoe(ActionMultipleTargetsCenter.CASTER, sectorRadius, angle);
        }

        if((abilityBehavior & AbilityBehavior.ABILITY_BEHAVIOR_RADIUS_AOE) != 0)
        {
            var radiusJson = GetJsonValue(json, "AbilityAoeRadius");
            if(radiusJson == null)
            {
                BattleLog.LogError("技能[{0}]行为是ABILITY_BEHAVIOR_RADIUS_AOE，未找到AbilityAoeRadius配置", abilityData.configFileName);
                return null;
            }

            float radius = (float)radiusJson;
            actionTarget.SetRadiusAoe(ActionMultipleTargetsCenter.CASTER, radius);
        }

        return actionTarget;
    }

    private static ActionTarget ParseActionTarget(JsonData json, AbilityData abilityData)
    {
        if(json == null)
            return null;
        var actionTarget = new ActionTarget();
        if(json.IsObject)
        {
            var aoeAreaJsonData = GetJsonValue(json, "AoeArea");
            if(aoeAreaJsonData == null)
            {
                BattleLog.LogError("技能[%s]中Target配置是区域目标配置，但是未配置AoeArea或者配置错误", abilityData.configFileName);
                return null;
            }
            var targetCenter = GetEnumValue<ActionMultipleTargetsCenter>(json, "Center");

            var areaType = GetStringValue(aoeAreaJsonData, "AreaType");
            var abilityAreaDamageType = GetEnumValue<AOEType>(areaType);

            switch(abilityAreaDamageType)
            {
                case AOEType.Radius:
                    float radius = (int)GetJsonValue(aoeAreaJsonData, "Radius");
                    actionTarget.SetRadiusAoe(targetCenter, radius);
                    break;
                case AOEType.Line:
                    JsonData lineJsonData = (int)GetJsonValue(aoeAreaJsonData, "Line");
                    if(lineJsonData == null)
                    {
                        BattleLog.LogError("技能[{0}]中Target配置是区域目标配置，但是未配置Line", abilityData.configFileName);
                        return null;
                    }
                    var length = (float)GetJsonValue(lineJsonData, "Length");
                    var thickness = (float)GetJsonValue(lineJsonData, "Thickness");
                    actionTarget.SetLineAoe(targetCenter, length, thickness);
                    break;
                case AOEType.Sector:
                    JsonData sectorJsonData = GetJsonValue(json, "Sector");
                    if(sectorJsonData == null)
                    {
                        BattleLog.LogError("技能[{0}]中Target配置是区域目标配置，但是未配置Sector", abilityData.configFileName);
                        return null;
                    }
                    var sectorRadius = (float)GetJsonValue(sectorJsonData, "Radius");
                    var angle = (float)GetJsonValue(sectorJsonData, "Angle");
                    actionTarget.SetSectorAoe(targetCenter, sectorRadius, angle);
                    break;
                default:
                    break;
            }
            var teamJsonData = GetStringValue(json, "Teams");
            var abilityTargetTeam = GetEnumValue<MultipleTargetsTeam>(teamJsonData);
            actionTarget.SetTargetTeam(abilityTargetTeam);
        }
        else
        {
            var target = (ActionSingTarget)Enum.Parse(typeof(ActionSingTarget), json.ToString());
            actionTarget.SetSingTarget(target);
        }
        return actionTarget;
    }

    private static List<D2Action> ParseActions(JsonData json, AbilityData abilityData)
    {
        if(json == null)
            return null;

        var actions = new List<D2Action>();
        foreach(string key in json.Keys)
        {
            Func<JsonData, ActionTarget, AbilityData, D2Action> createMethodName;
            if(AbilityActionCreateMap.TryGetValue(key, out createMethodName))
            {
                var actionJsonData = GetJsonValue(json, key);
                var targetJsonData = GetJsonValue(actionJsonData, "Target");
                var actionTarget = ParseActionTarget(targetJsonData, abilityData);
                var d2Action = createMethodName(actionJsonData, actionTarget, abilityData);
                actions.Add(d2Action);
            }
        }

        return actions;
    }

    #endregion

    #region Event

    private static Dictionary<string, D2Event> ParseAbilityEvents(JsonData json, AbilityData abilityData)
    {
        if(json == null)
            return null;

        var eventMap = new Dictionary<string, D2Event>();
        foreach(string key in json.Keys)
        {
            if(Enum.IsDefined(typeof(AbilityEvent), key))
            {
                var eventsJsonData = GetJsonValue(json, key);
                var actions = ParseActions(eventsJsonData, abilityData);
                var d2Event = new D2Event(actions);
                eventMap.Add(key, d2Event);
            }
        }
        return eventMap;
    }

    #endregion

    #region Modifier

    private static Dictionary<ModifierEvents, D2Event> ParseModifierEvents(JsonData json, AbilityData abilityData)
    {
        if(json == null)
            return null;

        var eventMap = new Dictionary<ModifierEvents, D2Event>();
        foreach(var key in json.Keys)
        {
            if(Enum.IsDefined(typeof(ModifierEvents), key))
            {
                var eventsJsonData = GetJsonValue(json, key);
                var actions = ParseActions(eventsJsonData, abilityData);
                var d2Event = new D2Event(actions);
                var eventName = GetEnumValue<ModifierEvents>(key);
                eventMap.Add(eventName, d2Event);
            }
        }
        return eventMap;
    }

    private static void ParseModifierAura(JsonData json, AbilityData abilityData, ModifierData modifier)
    {
        modifier.Aura = GetStringValue(json, "Aura");
        if(modifier.Aura != null)
        {
            modifier.Aura_Teams = GetEnumValue<MultipleTargetsTeam>(json, "Aura_Teams");
            if(modifier.Aura_Teams == default)
            {
                BattleLog.LogError("技能[{0}]的Modifier[{1}]中配置了光环类型，但是Aura_Teams未找到或者配置有错误", abilityData.configFileName, modifier.Name);
                return;
            }
            modifier.Aura_Types = GetEnumValue<MultipleTargetsType>(json, "Aura_Types");
            if(modifier.Aura_Teams == default)
            {
                BattleLog.LogError("技能[{0}]的Modifier[{1}]中配置了光环类型，但是Aura_Types未找到或者配置有错误", abilityData.configFileName, modifier.Name);
                return;
            }
            modifier.Aura_Radius = GetFloatValue(json, "Aura_Radius");
            if(modifier.Aura_Teams == default)
            {
                BattleLog.LogError("技能[{0}]的Modifier[{1}]中配置了光环类型，但是Aura_Radius未配置", abilityData.configFileName, modifier.Name);
                return;
            }
        }
    }

    private static void ParseModifierProperties(JsonData json, AbilityData abilityData, ModifierData modifier)
    {
        var propertyDataConfig = GetJsonValue(json, "Properties");
        if(propertyDataConfig != null)
        {
            var modifierPropertyDatas = new List<ModifierPropertyValue>();
            foreach(var propertyType in propertyDataConfig.Keys)
            {
                var propertyEnum = GetEnumValue<ModifierProperties>(propertyType.ToString());
                if(propertyEnum == default)
                {
                    BattleLog.LogError("技能[{0}]中有未定义的ModifierProperty类型[{1}]", abilityData.configFileName, propertyType);
                }
                else
                {
                    var modifierPropertyValue = new ModifierPropertyValue();
                    var propertyValueConfig = propertyDataConfig[propertyType];
                    if(propertyValueConfig!=null)
                    {
                        var valueSourceJson = GetJsonValue(propertyValueConfig, "ValueSource");
                        if(valueSourceJson == null)
                        {
                            GameLog.Log("todo 解析数值");
                            //modifierPropertyValue.
                        }
                        else
                        {
                            //var damageValueSource = ParseValueSource(valueSourceJson, abilityData);
                        }
                        modifierPropertyDatas.Add(modifierPropertyValue);
                    }
                    else
                    {
                        BattleLog.LogError("技能[{0}]中的ModifierProperty类型[{1}]不是数组配置或者ValueSource类型", abilityData.configFileName, propertyType);
                    }
                }
            }

            modifier.ModifierProperties = modifierPropertyDatas;
        }
    }

    private static void ParseModifierStates(JsonData json, AbilityData abilityData, ModifierData modifier)
    {
        var stateDataConfig = GetJsonValue(json, "States");
        if(stateDataConfig != null)
        {
            var modifierStates = new List<ModifierState>();
            foreach(string stateName in stateDataConfig.Keys)
            {
                var stateEnum = GetEnumValue<ModifierStates>(stateName);
                if(stateEnum == default)
                {
                    BattleLog.LogError("技能[{0}]中有未定义的ModifierStates类型[{1}]", abilityData.configFileName, stateName);
                }
                else
                {
                    //var stateValue = GetIntValue(stateDataConfig, stateName);
                    // todo 解析数值
                    //modifierStates.Add();
                }
            }
            modifier.States = modifierStates;
        }
    }

    private static ModifierData ParseModifier(JsonData json, AbilityData abilityData)
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
        modifier.ModifierEventMap = ParseModifierEvents(json, abilityData);
        // aura
        ParseModifierAura(json, abilityData, modifier);
        // ModifierProperties
        ParseModifierProperties(json, abilityData, modifier);
        // States
        ParseModifierStates(json, abilityData, modifier);

        return modifier;
    }

    /// <summary>
    /// 解析modifier
    /// </summary>
    private static Dictionary<string, ModifierData> ParseModifiers(JsonData json, AbilityData abilityData)
    {
        var res = new Dictionary<string, ModifierData>();
        var Modifiers = GetJsonValue(json, "Modifiers");
        if(Modifiers != null)
        {
            var keys = Modifiers.Keys.ToList();
            for(int i = 0; i < Modifiers.Count; i++)
            {
                var modifierJson = Modifiers[i];
                var modifier = ParseModifier(modifierJson, abilityData);
                modifier.Name = keys[i];
                res.Add(modifier.Name, modifier);
            }
        }
        return res;
    }


    #endregion

    private static AbilityData CreateAbility(string path) 
    {
        JsonData jsonData = LoadModule.LoadJson(path);
        if(jsonData == null)
        {
            BattleLog.LogError("[CreateAbility]没有找到指定json配置！", path);
            return null;
        }

        AbilityData abilityData = new AbilityData();
        abilityData.configFileName = GetStringValue(jsonData, "Name");
        string abilityType = GetStringValue(jsonData, "AbilityType");
        abilityData.abilityType = abilityType;
        abilityData.abilityBranch = GetEnumValue<AbilityBranch>(jsonData, "AbilityBranch");
        abilityData.abilityBehavior = ParseAbilityBehaviorArray(jsonData, "AbilityBehavior"); 
        abilityData.aiTargetCondition = GetEnumValue<AbilityUnitAITargetCondition>(jsonData, "AbilityUnitAiTargetCondition");
        abilityData.castRange = GetFloatValue(jsonData, "AbilityCastRange");
        abilityData.castPoint = GetFloatValue(jsonData, "AbilityCastPoint");
        abilityData.castDuration = GetFloatValue(jsonData, "AbilityCastDuration");
        abilityData.castAnimation = GetStringValue(jsonData, "AbilityCastAnimation");
        abilityData.cooldown = GetFloatValue(jsonData, "AbilityCooldown");
        abilityData.costType = GetEnumValue<AbilityCostType>(jsonData, "AbilityCostType");
        abilityData.costValue = GetFloatValue(jsonData, "AbilityCostValue");

        abilityData.abilityTarget = ParseAbilityRange(jsonData, abilityData);

        // 解析技能事件
        abilityData.eventMap = ParseAbilityEvents(jsonData, abilityData);

        // 解析Modifier
        abilityData.modifierDataMap = ParseModifiers(jsonData, abilityData);

        return abilityData;
    }
    
    public static Ability CreateAbility(int id, BattleUnit caster)
    {
        skillItem skillItem = skillTable.Instance.GetTableItem(id);
        if(skillItem == null)
        {
            BattleLog.LogError("skill 表没有找到指定id:", id.ToString());
            return null;
        }

        string path = skillItem.config;
        int priority = skillItem.priority;
        AbilityData abilityData = CreateAbility(path);
        Ability ability = new Ability(id, priority, caster, abilityData);
        return ability;
    }
}