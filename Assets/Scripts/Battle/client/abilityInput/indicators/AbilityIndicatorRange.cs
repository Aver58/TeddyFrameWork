#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityIndicatorRange.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/12 10:03:06
=====================================================
*/
#endregion

using UnityEngine;

/// <summary>
/// 技能范围指示器
/// </summary>
public class AbilityIndicatorRange : AbilityIndicator
{
    private float m_range;
    public AbilityIndicatorRange(Transform indicatorTransform, Transform casterTransform, float range) 
        : base(indicatorTransform, casterTransform)
    {
        m_range = range;
    }

    protected override void OnShow()
    {
        base.OnShow();

        m_indicatorTransform.position = m_casterTransform.position;
        m_indicatorTransform.localScale = new Vector3(m_range,1,m_range);
    }
}