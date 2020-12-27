#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AutoCastAbilityRequest.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/18 13:56:27
=====================================================
*/
#endregion

public class AutoCastAbilityRequest : AIBehaviorRequest
{
    public Ability ability { get; set; }
    public AutoCastAbilityRequest(Ability ability, BattleEntity target) : base(RequestType.AutoCastAbility)
    {
        this.ability = ability;
        this.target = target;

        ability.requestTarget.SetUnitTarget(target);
    }
}