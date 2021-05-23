#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityData.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/17 13:23:50
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;

public class AbilityData
{
    public Dictionary<string, D2Event> eventMap { get; set; }
    public ActionTarget abilityTarget { get; set; }
    public string configFileName { get; set; }
    public string abilityType { get; set; }
    public AbilityBranch abilityBranch { get; set; }
    public AbilityBehavior abilityBehavior { get; set; }
    public string aiTargetFlag { get; set; }
    public AbilityUnitAITargetCondition aiTargetCondition { get; set; }
    public string hitStopTime { get; set; }
    public string initialCooldown { get; set; }
    public float cooldown { get; set; }
    public float castPoint { get; set; }
    public float castRange { get; set; }
    public float castDuration { get; set; }
    public float channelTime { get; set; }
    public string castAnimation { get; set; }
    public AbilityCostType costType { get; set; }
    public float costValue { get; set; }
    public string abilityEvents { get; set; }
    public string modifierNames { get; set; }
    public Dictionary<string, ModifierData> modifierDataMap { get; set; }

    public void ExecuteEvent(AbilityEvent abilityEvent, BattleUnit source, RequestTarget requestTarget)
    {
        BattleLog.Log("【AbilityData】ExecuteEvent：{0}，source：{1}，target：{2}", abilityEvent.ToString(), source.GetName(), requestTarget.ToString());

        string eventName = Enum.GetName(typeof(AbilityEvent), abilityEvent);
        D2Event @event;
        eventMap.TryGetValue(eventName,out @event);
        if(@event!=null)
        {
            @event.Execute(source, this, requestTarget);
        }
    }

    public ModifierData GetModifierData(string modifierName)
    {
        ModifierData d2Modifier;
        if(modifierDataMap.TryGetValue(modifierName, out d2Modifier))
            return d2Modifier;

        return null;
    }

    public List<ModifierData> GetAllPassiveModifierData()
    {
        List<ModifierData> modifiers = new List<ModifierData>();
        foreach(var item in modifierDataMap)
        {
            var modifierData = item.Value;
            if(modifierData.Passive)
                modifiers.Add(modifierData);
        }
        return modifiers;
    }
}