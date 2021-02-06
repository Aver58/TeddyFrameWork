#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityIndicatorLine.cs
 Author:      Zeng Zhiwei
 Time:        2021\1\12 星期二 22:27:48
=====================================================
*/
#endregion

using UnityEngine;

/// <summary>
/// 直线类型技能指示器【扇形指示器也用这个逻辑】
/// </summary>
public class AbilityIndicatorLine : AbilityIndicator
{
    private float m_lineLength;
    private float m_lineThickness;
    public AbilityIndicatorLine(Transform indicatorTransform, Transform casterTransform, float lineLength, float lineThickness) 
        : base(indicatorTransform, casterTransform)
    {
        m_lineLength = lineLength;
        m_lineThickness = lineThickness;
    }

    protected override void OnShow()
    {
        base.OnShow();

        m_indicatorTransform.position = m_casterTransform.position;
        m_indicatorTransform.localScale = new Vector3(m_lineThickness, 1, m_lineLength);
    }

    protected override void OnUpdate(float casterX, float casterZ, float targetX, float targetZ, float targetForwardX, float targetForwardZ)
    {
        base.OnUpdate(casterX, casterZ, targetX, targetZ, targetForwardX, targetForwardZ);

        m_position.Set(targetForwardX, 0, targetForwardZ);
        m_indicatorTransform.forward = m_position;
        m_indicatorTransform.position = m_casterTransform.position;
    }
}