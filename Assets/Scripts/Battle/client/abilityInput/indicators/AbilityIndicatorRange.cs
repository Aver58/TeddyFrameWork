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

public class AbilityIndicatorRange : AbilityIndicator
{
    private float m_range;
    public AbilityIndicatorRange(Transform transform, float range) : base(transform)
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