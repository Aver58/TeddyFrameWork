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
    public Vector2 targetPos;
    private BattleUnit m_TargetUnit;

    public AbilityRequestTargetType targetType;

    public void SetUnitTarget(BattleUnit battleEntity)
    {
        targetType = AbilityRequestTargetType.UNIT;
        m_TargetUnit = battleEntity;
    }

    public void SetPointTarget(float x,float z)
    {
        targetType = AbilityRequestTargetType.POINT;
        targetPos.Set(x, z);
    }

    public void GetTarget2DPosition(out float x, out float z)
    {
        if(targetType == AbilityRequestTargetType.UNIT)
        {
            m_TargetUnit.Get2DPosition(out x, out z);
        }
        else
        {
            x = targetPos.x;
            z = targetPos.y;
        }
    }

    public BattleUnit GetTargetUnit()
    {
        return m_TargetUnit;
    }

    public override string ToString()
    {
        if(targetType == AbilityRequestTargetType.UNIT)
        {
            if(m_TargetUnit==null)
                return "null target";

            return m_TargetUnit.GetName();
        }
        else
        {
            return string.Format("position:({0},{1})", targetPos.x.ToString(), targetPos.y.ToString());
        }
    }
}