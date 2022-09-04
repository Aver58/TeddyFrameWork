#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Action_Target.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/22 21:05:54
=====================================================
*/
#endregion

/// <summary>
/// 技能目标及范围
/// Within an action the Target key can be given a value to specify what it is that the target is. 
/// This exposes some choices to make as to what you may want to target.
/// </summary>
public class AbilityTarget
{
    public bool isSingleTarget;
    public ActionSingTarget singTarget;
    public AbilityUnitTargetTeam targetTeam;
    public AbilityUnitTargetType targetType;
    public AbilityUnitTargetType excludeTypes;
    public AbilityUnitTargetFlags targetFlags;
    public AbilityUnitTargetFlags excludeFlags;
    public ActionMultipleTargets Target;
    public float radius;
    public int maxTargets;
    public bool randomOneTarget;// 如果有多个目标，是否随机选择一个目标
    public float lineLength;
    public float lineThickness;
    public AOEType aoeType;
    public float sectorRadius;
    public float sectorAngle;

    public AbilityTarget()
    {
        isSingleTarget = false;
    }

    public void SetSingTarget(ActionSingTarget value)
    {
        isSingleTarget = true;
        singTarget = value;
    }

    public void SetTargetTeam(AbilityUnitTargetTeam value)
    {
        targetTeam = value;
    }

    public void SetRadiusAoe(ActionMultipleTargets target,float radius)
    {
        isSingleTarget = false;
        aoeType = AOEType.Radius;
        this.Target = target;
        this.radius = radius;
    }

    public void SetLineAoe(ActionMultipleTargets target, float lineLength, float lineThickness)
    {
        isSingleTarget = false;
        aoeType = AOEType.Line;
        this.Target = target;
        this.lineLength = lineLength;
        this.lineThickness = lineThickness;
    }

    public void SetSectorAoe(ActionMultipleTargets target, float sectorRadius, float sectorAngle)
    {
        isSingleTarget = false;
        aoeType = AOEType.Sector;
        this.Target = target;
        this.sectorRadius = sectorRadius;
        this.sectorAngle = sectorAngle;
    }
}