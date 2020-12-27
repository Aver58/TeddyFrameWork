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
using LitJson;

public static class AbilityReader
{
    #region 创建Action映射方法
    public static Dictionary<string, Func<JsonData, AbilityRange, AbilityData, D2Action>> AbilityActionCreateMap = new Dictionary<string, Func<JsonData, AbilityRange, AbilityData, D2Action>>
    {
        { "IsHit" , CreateIsHitAction},
        { "ChangeEnergy" , CreateChangeEnergyAction},
        { "Damage" , CreateDamageAction},
    };

    private static D2Action CreateIsHitAction(JsonData actionJsonData,AbilityRange abilityRange, AbilityData abilityData)
    {
        JsonData onSuccessActionConfig = GetJsonValue(actionJsonData, "OnSuccess");
        if(onSuccessActionConfig == null)
        {
            BattleLog.LogError("技能[%s]中IsHit未找到key[OnSuccess]的配置", abilityData.configFileName);
            return null;
        }
        List<D2Action> successActions = ParseActions(onSuccessActionConfig, abilityData);

        D2Action_IsHit d2Action_IsHit = new D2Action_IsHit(successActions, abilityData);
        return d2Action_IsHit;
    }

    private static D2Action CreateChangeEnergyAction(JsonData actionJsonData, AbilityRange abilityRange, AbilityData abilityData)
    {
        JsonData energyParams = GetJsonValue(actionJsonData, "EnergyParams");
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

        D2Action_ChangeEnergy d2Action_ChangeEnergy = new D2Action_ChangeEnergy(energyParams, abilityData);
        return d2Action_ChangeEnergy;
    }

    /*"Damage": 
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
    private static D2Action CreateDamageAction(JsonData actionJsonData, AbilityRange abilityRange, AbilityData abilityData)
    {
        string damageTypeConfig = GetJsonValueToString(actionJsonData, "DamageType");
        if(damageTypeConfig == null)
            return null;
        AbilityDamageType damageType = GetEnumValue<AbilityDamageType>(damageTypeConfig);

        JsonData damageFlagConfig = GetJsonValue(actionJsonData, "DamageFlags");
        if(damageFlagConfig == null)
            return null;

        AbilityDamageFlag damageFlag = ParseDamageFlagArray(damageFlagConfig);

        AbilityValueSource damageValueSource = ParseValueSource(actionJsonData, abilityData);
        JsonData dealDamageSuccessActionConfig = GetJsonValue(actionJsonData, "OnSuccess");
        List<D2Action> actions = ParseActions(dealDamageSuccessActionConfig, abilityData);
        D2Action_Damage d2Action_Damage = new D2Action_Damage(damageType, damageFlag, damageValueSource, actions, abilityData);
        return d2Action_Damage;
    }

    #endregion

    private static AbilityDamageFlag ParseDamageFlagArray(JsonData jsonData)
    {
        AbilityDamageFlag res = AbilityDamageFlag.DAMAGE_FLAG_NONE;
        if(jsonData == null)
            return res;
        if(jsonData.IsArray)
        {
            for(int i = 0; i < jsonData.Count; i++)
            {
                string item = jsonData[i].ToString();
                AbilityDamageFlag damageFlag = GetEnumValue<AbilityDamageFlag>(item);
                res |= damageFlag;
            }
        }
        return res;
    }

    private static AbilityValueSource ParseValueSource(JsonData jsonData, AbilityData abilityData)
    {
        AbilityValueSource abilityValueSource = new AbilityValueSource();
        JsonData valueSourceJsonData = GetJsonValue(jsonData, "ValueSource");
        if(valueSourceJsonData == null)
            return abilityValueSource;

        JsonData valueBasicParamsConfig = GetJsonValue(valueSourceJsonData, "ValueBasicParams");
        if(valueBasicParamsConfig == null)
            return abilityValueSource;

        if(!valueBasicParamsConfig.IsArray)
        {
            BattleLog.LogError("技能[%s]中的[ValueSource]的[ValueBasicParams]的配置必须是数组类型", abilityData.configFileName);
        }
        else
        {
            string baseJsonData = valueBasicParamsConfig[0].ToString();
            string baseGrowJsonData = valueBasicParamsConfig[1].ToString();
            float baseValue = string.IsNullOrEmpty(baseJsonData) ? 0f : float.Parse(baseJsonData);
            float baseValueGrow = string.IsNullOrEmpty(baseGrowJsonData) ? 0f : float.Parse(baseGrowJsonData);
            abilityValueSource.baseValue = baseValue;
            abilityValueSource.baseValueGrow = baseValueGrow;
        }
        //todo 附加值
        JsonData valueAdditionParamsConfig = GetJsonValue(valueSourceJsonData, "ValueAdditionParams");
        if(valueAdditionParamsConfig != null)
        {
            if(valueAdditionParamsConfig.IsArray)
            {
                for(int i = 0; i < valueAdditionParamsConfig.Count; i++)
                {
                    JsonData additionValueConfig = valueAdditionParamsConfig[i];
                    AbilityValueSourceType valueSourceType = GetJsonValueToEnum<AbilityValueSourceType>(additionValueConfig, "ValueSourceType");
                    JsonData valueSourceParamsConfig = GetJsonValue(additionValueConfig, "ValueSourceParams");
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
                        {
                            BattleLog.LogError("技能[%s]中的[ValueSource]的[ValueSourceParams]的配置必须是数组类型", abilityData.configFileName);
                        }
                    }
                }
            }
            else
            {
                BattleLog.LogError("技能[%s]中的[ValueSource]的[ValueAdditionParams]的配置必须是数组类型", abilityData.configFileName);
            }
        }

        return abilityValueSource;
    }

    private static AbilityRange ParseAbilityRange(JsonData jsonData, AbilityData abilityData)
    {
        if(jsonData == null)
            return null;
        AbilityRange abilityRange = new AbilityRange();
        //string targetTypeJsonData = GetJsonValueToString(jsonData, "AbilityUnitTargetType");todo
        AbilityUnitTargetTeam abilityTargetTeam = GetJsonValueToEnum<AbilityUnitTargetTeam>(jsonData, "AbilityUnitTargetTeam");
        abilityRange.SetTargetTeam(abilityTargetTeam);

        //todo 解析大招的指示器范围
        return abilityRange;
        //return ParseActionRange(jsonData, abilityData);
    }

    private static AbilityRange ParseActionRange(JsonData targetJsonData, AbilityData abilityData)
    {
        if(targetJsonData == null)
            return null;
        AbilityRange actionRange = new AbilityRange();
        if(targetJsonData.IsObject)
        {
            JsonData aoeAreaJsonData = GetJsonValue(targetJsonData, "AoeArea");
            if(aoeAreaJsonData == null)
            {
                BattleLog.LogError("技能[%s]中Target配置是区域目标配置，但是未配置AoeArea或者配置错误", abilityData.configFileName);
                return null;
            }
            string targetCenter = GetJsonValueToString(targetJsonData, "Center");

            string areaType = GetJsonValueToString(aoeAreaJsonData, "AreaType");
            AbilityAreaDamageType abilityAreaDamageType = GetEnumValue<AbilityAreaDamageType>(areaType);
   
            switch(abilityAreaDamageType)
            {
                case AbilityAreaDamageType.Radius:
                    float radius = (int)GetJsonValue(aoeAreaJsonData, "Radius");
                    actionRange.SetRadiusAoe(targetCenter, radius);
                    break;
                case AbilityAreaDamageType.Line:
                    JsonData lineJsonData = (int)GetJsonValue(aoeAreaJsonData, "Line");
                    if(lineJsonData == null)
                    {
                        BattleLog.LogError("技能[{0}]中Target配置是区域目标配置，但是未配置Line", abilityData.configFileName);
                        return null;
                    }
                    float length = (float)GetJsonValue(lineJsonData, "Length");
                    float thickness = (float)GetJsonValue(lineJsonData, "Thickness");
                    actionRange.SetLineAoe(targetCenter, length, thickness);
                    break;
                case AbilityAreaDamageType.Sector:
                    JsonData sectorJsonData = GetJsonValue(targetJsonData, "Sector");
                    if(sectorJsonData == null)
                    {
                        BattleLog.LogError("技能[{0}]中Target配置是区域目标配置，但是未配置Sector", abilityData.configFileName);
                        return null;
                    }
                    float sectorRadius = (float)GetJsonValue(sectorJsonData, "Radius");
                    float angle = (float)GetJsonValue(sectorJsonData, "Angle");
                    actionRange.SetSectorAoe(targetCenter, sectorRadius, angle);
                    break;
                default:
                    break;
            }
            string teamJsonData = GetJsonValueToString(targetJsonData, "Teams");
            AbilityUnitTargetTeam abilityTargetTeam = GetEnumValue<AbilityUnitTargetTeam>(teamJsonData);
            actionRange.SetTargetTeam(abilityTargetTeam);
        }
        else
        {
            AbilitySingTarget target = (AbilitySingTarget)Enum.Parse(typeof(AbilitySingTarget), targetJsonData.ToString());
            actionRange.SetSingTarget(target);
        }
        return actionRange;
    }

    private static List<D2Action> ParseActions(JsonData eventsJsonDatas, AbilityData abilityData)
    {
        if(eventsJsonDatas == null)
            return null;

        List<D2Action> actions = new List<D2Action>();
        foreach(string key in eventsJsonDatas.Keys)
        {
            Func<JsonData, AbilityRange, AbilityData, D2Action> createMethodName;
            if(AbilityActionCreateMap.TryGetValue(key,out createMethodName))
            {
                JsonData actionJsonData = GetJsonValue(eventsJsonDatas, key);
                JsonData targetJsonData = GetJsonValue(actionJsonData, "Target");
                AbilityRange abilityRange = ParseActionRange(targetJsonData, abilityData);
                D2Action d2Action = createMethodName(actionJsonData, abilityRange, abilityData);
                actions.Add(d2Action);
            }
        }

        return actions;
    }

    private static Dictionary<string,D2Event> ParseAbilityEvents(JsonData jsonData, AbilityData abilityData)
    {
        if(jsonData == null)
            return null;
        Dictionary<string, D2Event> eventMap = new Dictionary<string, D2Event>();
        foreach(string key in jsonData.Keys)
        {
            if(Enum.IsDefined(typeof(AbilityEvent), key))
            {
                JsonData eventsJsonData = GetJsonValue(jsonData, key);
                List<D2Action> actions = ParseActions(eventsJsonData, abilityData);
                D2Event d2Event = new D2Event(actions);
                eventMap.Add(key, d2Event);
            }
        }
        return eventMap;
    }

    private static T GetEnumValue<T>(string enumStr)
    {
        if(enumStr == null)
            return default(T);

        T value = (T)Enum.Parse(typeof(T), enumStr.ToString());
        return value;
    }

    private static JsonData GetJsonValue(JsonData jsonData, string key)
    {
        if(jsonData == null || string.IsNullOrEmpty(key))
            return null;

        if(jsonData.Keys.Contains(key))
        {
            return jsonData[key];
        }
        return null;
    }

    private static string GetJsonValueToString(JsonData jsonData, string key)
    {
        JsonData resultJsonData = GetJsonValue(jsonData, key);
        if(resultJsonData != null)
            return resultJsonData.ToString();

        return null;
    }

    private static float GetJsonValueToFloat(JsonData jsonData, string key)
    {
        string resultJsonData = GetJsonValueToString(jsonData, key);
        float res = string.IsNullOrEmpty(resultJsonData) ? 0 : float.Parse(resultJsonData);
        return res;
    }

    private static T GetJsonValueToEnum<T>(JsonData jsonData, string key)
    {
        string resultStr = GetJsonValueToString(jsonData, key);
        return GetEnumValue<T>(resultStr);
    }

    private static AbilityBehavior ParseAbilityBehaviorArray(JsonData jsonData, string key)
    {
        JsonData behaviorConfigs = GetJsonValue(jsonData, key);
        if(behaviorConfigs == null)
            return 0;
        AbilityBehavior behavior = 0;
        if(behaviorConfigs.IsArray)
        {
            for(int i = 0; i < behaviorConfigs.Count; i++)
            {
                string item = behaviorConfigs[i].ToString();
                AbilityBehavior abilityBehavior = GetEnumValue<AbilityBehavior>(item);
                behavior |= abilityBehavior;
            }
        }
        return behavior;
    }

    private static AbilityData CreateAbility(string configPath) 
    {
        JsonData jsonData = LoadModule.instance.LoadJson(configPath);
        if(jsonData == null)
        {
            BattleLog.LogError("[CreateAbility]没有找到指定json配置！", configPath);
            return null;
        }
        AbilityData abilityData = new AbilityData();
        abilityData.configFileName = GetJsonValueToString(jsonData, "Name");
        string abilityType = GetJsonValueToString(jsonData, "AbilityType");
        abilityData.abilityType = abilityType;
        abilityData.abilityBranch = GetJsonValueToEnum<AbilityBranch>(jsonData, "AbilityBranch");
        abilityData.abilityBehavior = ParseAbilityBehaviorArray(jsonData, "AbilityBehavior"); 
        abilityData.aiTargetCondition = GetJsonValueToEnum<AbilityUnitAITargetCondition>(jsonData, "AbilityUnitAiTargetCondition");
        abilityData.castRange = GetJsonValueToFloat(jsonData, "AbilityCastRange");
        abilityData.castPoint = GetJsonValueToFloat(jsonData, "AbilityCastPoint");
        abilityData.castDuration = GetJsonValueToFloat(jsonData, "AbilityCastDuration");
        abilityData.castAnimation = GetJsonValueToString(jsonData, "AbilityCastAnimation");
        abilityData.cooldown = GetJsonValueToFloat(jsonData, "AbilityCooldown");
        abilityData.costType = GetJsonValueToEnum<AbilityCostType>(jsonData, "AbilityCostType");
        abilityData.costValue = GetJsonValueToFloat(jsonData, "AbilityCostValue");

        abilityData.abilityRange = ParseAbilityRange(jsonData, abilityData);

        // 解析技能事件
        abilityData.eventMap = ParseAbilityEvents(jsonData, abilityData);

        // 解析Modifier
        return abilityData;
    }
    
    public static Ability CreateAbility(int id,BattleEntity caster)
    {
        skillItem skillItem = skillTable.Instance.GetTableItem(id);
        if(skillItem == null)
        {
            BattleLog.LogError("skill 表没有找到指定id:", id.ToString());
            return null;
        }

        string config = skillItem.config;
        int priority = skillItem.priority;
        AbilityData abilityData = CreateAbility(config);
        Ability ability = new Ability(id, priority, caster, abilityData);
        return ability;
    }
}