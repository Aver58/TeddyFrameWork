#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Action_ApplyModifier.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\30 星期六 15:37:06
=====================================================
*/
#endregion

using System.Collections.Generic;

/// <summary>
/// 应用Modifier
/// </summary>
public class D2Action_ApplyModifier : D2Action
{
    private string modifierName;

    public D2Action_ApplyModifier(string modifierName, ActionTarget actionTarget) : base(actionTarget)
    {
        this.modifierName = modifierName;
    }

    //protected override void ExecuteByPoint(BattleUnit source, List<BattleUnit> targets)
    //{
    //    for(int i = 0; i < targets.Count; i++)
    //    {
    //        var target = targets[i];
    //        target.ApplyModifierByName(source, abilityData, modifierName);
    //    }
    //}
}