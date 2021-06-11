#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityInputTarget.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/14 14:42:07
=====================================================
*/
#endregion

using UnityEngine;

public class AbilityInputTarget : AbilityInput
{
    public AbilityInputTarget(Ability ability, HeroActor casterActor) : base(ability, casterActor)
    {
    }
}