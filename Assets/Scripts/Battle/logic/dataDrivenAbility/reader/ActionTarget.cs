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
/// </summary>
public class ActionTarget
{
    public bool isSingleTarget;
    public AbilitySingTarget singTarget { get; set; }
    public AbilityUnitTargetTeam targetTeam { get; set; }
    public AbilityAreaDamageType areaDamageType { get; set; }
    public float Radius { get; set; }
    public float LineLength { get; set; }
    public float LineThickness { get; set; }
    public AbilityUnitTargetCenter Center { get; set; }
    public float SectorRadius { get; set; }
    public float SectorAngle { get; set; }

    public ActionTarget()
    {
        isSingleTarget = false;
    }

    public void SetSingTarget(AbilitySingTarget value)
    {
        singTarget = value;
    }

    public void SetTargetTeam(AbilityUnitTargetTeam value)
    {
        targetTeam = value;
    }

    public void SetRadiusAoe(AbilityUnitTargetCenter targetCenter,float radius)
    {
        isSingleTarget = false;
        areaDamageType = AbilityAreaDamageType.Radius;
        Center = targetCenter;
        Radius = radius;
    }

    public void SetLineAoe(AbilityUnitTargetCenter targetCenter, float lineLength, float lineThickness)
    {
        isSingleTarget = false;
        areaDamageType = AbilityAreaDamageType.Line;
        Center = targetCenter;
        LineLength = lineLength;
        LineThickness = lineThickness;
    }

    public void SetSectorAoe(AbilityUnitTargetCenter targetCenter, float sectorRadius, float sectorAngle)
    {
        isSingleTarget = false;
        areaDamageType = AbilityAreaDamageType.Sector;
        Center = targetCenter;
        SectorRadius = sectorRadius;
        SectorAngle = sectorAngle;
    }
}