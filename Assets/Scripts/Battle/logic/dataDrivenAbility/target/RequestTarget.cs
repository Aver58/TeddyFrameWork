#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    RequestTarget.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/28 9:45:54
=====================================================
*/
#endregion

using UnityEngine;
/// <summary>
/// 请求目标 
/// 1:AI请求攻击的目标 
/// 2:手动选择技能选中的目标或者点
/// </summary>
public class RequestTarget
{
    public AbilityRequestTargetType targetType { get; set; }

    private BattleEntity m_TargetUnit;
    private Vector2 m_TargetPos;

    public void SetUnitTarget(BattleEntity battleEntity)
    {
        targetType = AbilityRequestTargetType.UNIT;
        m_TargetUnit = battleEntity;
    }

    public void SetPointTarget(float x,float z)
    {
        targetType = AbilityRequestTargetType.POINT;
        m_TargetPos.Set(x, z);
    }

    public void GetTarget()
    {

    }

    public override string ToString()
    {
        if(targetType == AbilityRequestTargetType.UNIT)
        {
            return m_TargetUnit.GetName();
        }
        else
        {
            return string.Format("position:({0},{1})", m_TargetPos.x.ToString(), m_TargetPos.y.ToString());
        }
    }
}