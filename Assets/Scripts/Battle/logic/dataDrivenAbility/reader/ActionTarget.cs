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
public class ActionTarget
{
    public bool isSingleTarget;
    public ActionSingTarget singTarget { get; set; }
    public MultipleTargetsTeam Teams { get; set; }
    public MultipleTargetsType Types { get; set; }
    public MultipleTargetsType ExcludeTypes { get; set; }
    public ActionMultipleTargetsFlag Flags { get; set; }
    public ActionMultipleTargetsFlag ExcludeFlags { get; set; }
    public float Radius { get; set; }
    public int MaxTargets { get; set; }
    public bool Random { get; set; }// Whether to select a random unit if more than MaxTargets exist.
    public float LineLength { get; set; }
    public float LineThickness { get; set; }
    public ActionMultipleTargetsCenter Center { get; set; }
    public AOEType aoeType { get; set; }
    public float SectorRadius { get; set; }
    public float SectorAngle { get; set; }

    public ActionTarget()
    {
        isSingleTarget = false;
    }

    public void SetSingTarget(ActionSingTarget value)
    {
        isSingleTarget = true;
        singTarget = value;
    }

    public void SetTargetTeam(MultipleTargetsTeam value)
    {
        Teams = value;
    }

    public void SetRadiusAoe(ActionMultipleTargetsCenter targetCenter,float radius)
    {
        isSingleTarget = false;
        aoeType = AOEType.Radius;
        Center = targetCenter;
        Radius = radius;
    }

    public void SetLineAoe(ActionMultipleTargetsCenter targetCenter, float lineLength, float lineThickness)
    {
        isSingleTarget = false;
        aoeType = AOEType.Line;
        Center = targetCenter;
        LineLength = lineLength;
        LineThickness = lineThickness;
    }

    public void SetSectorAoe(ActionMultipleTargetsCenter targetCenter, float sectorRadius, float sectorAngle)
    {
        isSingleTarget = false;
        aoeType = AOEType.Sector;
        Center = targetCenter;
        SectorRadius = sectorRadius;
        SectorAngle = sectorAngle;
    }
}