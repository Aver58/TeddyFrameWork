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

public class ManualCastAbilityRequest : BehaviorRequest
{
    public Ability ability { get; set; }
    public ManualCastAbilityRequest(Ability ability) : base(RequestType.ManualCastAbility)
    {
        this.ability = ability;
    }
}