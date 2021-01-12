#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityIndicatorPoint.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/12 10:39:00
=====================================================
*/
#endregion

using UnityEngine;

public class AbilityIndicatorPoint : AbilityIndicator
{
    private float m_range;
    private Vector3 m_position = Vector3.zero;
    public AbilityIndicatorPoint(Transform transform, float range) : base(transform)
    {
        m_range = range;
    }

    protected override void OnShow()
    {
        base.OnShow();

        m_indicatorTransform.localScale = new Vector3(m_range, 1, m_range);
    }

    protected override void OnUpdate(float casterX, float casterZ, float targetX, float targetZ, float targetForwardX, float targetForwardZ)
    {
        base.OnUpdate(casterX, casterZ, targetX, targetZ, targetForwardX, targetForwardZ);
        m_position.Set(targetX, 0, targetZ);
        m_indicatorTransform.position = m_position;
    }
}