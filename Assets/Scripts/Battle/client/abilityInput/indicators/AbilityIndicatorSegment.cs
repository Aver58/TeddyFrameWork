#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityIndicatorSegment.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\12 星期二 23:11:01
=====================================================
*/
#endregion

using UnityEngine;

public class AbilityIndicatorSegment : AbilityIndicator
{
    private float m_length = 0f;
    private Vector3 m_scale = Vector3.zero;
    public AbilityIndicatorSegment(Transform indicatorTransform, Transform casterTransform = null) 
        : base(indicatorTransform, casterTransform){ }

    protected override void OnShow()
    {
        base.OnShow();

        m_indicatorTransform.position = m_casterTransform.position;
    }

    protected override void OnUpdate(float casterX, float casterZ, float targetX, float targetZ, float targetForwardX, float targetForwardZ)
    {
        base.OnUpdate(casterX, casterZ, targetX, targetZ, targetForwardX, targetForwardZ);
        m_length = BattleMath.SqartDistance2DMagnitude(casterX, casterZ, targetX, targetZ);

        var scale = m_indicatorTransform.localScale;
        m_scale.Set(scale.x, scale.y, m_length);
        m_indicatorTransform.localScale = m_scale;

        m_position.Set(targetForwardX, 0, targetForwardZ);
        m_indicatorTransform.forward = m_position;
    }
}