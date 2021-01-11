#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityActor.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\10 星期日 20:35:23
=====================================================
*/
#endregion

using UnityEngine;

/// <summary>
/// 技能Actor【技能指示器、特效】
/// </summary>
public class AbilityActor
{
    private AbilityInput m_abilityInput;

    public AbilityActor(Ability ability)
    {
        // 解析技能指示器
        //m_abilityInput = new AbilityInput();
    }

    public void OnFingerDown()
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerDown();
    }

    public void OnFingerDrag(Vector3 forward)
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerDrag(forward);
    }

    public void OnFingerUp()
    {
        if(m_abilityInput != null)
            m_abilityInput.OnFingerUp();
    }
}