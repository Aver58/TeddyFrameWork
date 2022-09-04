#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ModifierData.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\30 星期六 22:02:43
=====================================================
*/
#endregion

using System.Collections.Generic;

public class ModifierData
{
    public string Name;
    public ModifierProperty Property;
    public float Duration;
    public float ThinkInterval;//触发间隔
    public bool IsBuff;
    public bool IsDebuff;
    public bool IsPurgable;// 可净化
    public bool Passive;// 被动
    public bool IsHidden;

    // Aura
    public string Aura;
    public float Aura_Radius;
    public AbilityUnitTargetTeam Aura_Teams;
    public AbilityUnitTargetType Aura_Types;

    public List<ModifierState> States;
    public List<ModifierPropertyValue> ModifierProperties;
    public Dictionary<ModifierEvents, D2Event> ModifierEventMap;
    // 特效
    public string EffectName;
    public ModifierEffectAttachType EffectAttachType;

    public D2Event GetEvent(ModifierEvents name)
    {
        D2Event d2Event;
        if(ModifierEventMap.TryGetValue(name, out d2Event))
            return d2Event;
        
        return null;
    }
}