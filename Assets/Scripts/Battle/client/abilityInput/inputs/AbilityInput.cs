#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityInput.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\10 星期日 17:55:35
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class AbilityInput
{
    private List<AbilityIndicator> m_AbilityIndicators;
    public void AddAbilityIndicator(AbilityIndicator abilityIndicator)
    {
        m_AbilityIndicators.Add(abilityIndicator);
    }

    public void OnFingerDown()
    {

    }

    public void OnFingerDrag(Vector3 forward)
    {

    }

    public void OnFingerUp()
    {

    }
}