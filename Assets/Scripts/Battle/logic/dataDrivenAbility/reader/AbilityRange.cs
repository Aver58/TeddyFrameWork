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

public class AbilityRange
{
    public bool isSingleTarget;
    public AbilitySingTarget singTarget { get; set; }
    public AbilityUnitTargetTeam targetTeam { get; set; }
    public AbilityAreaDamageType areaDamageType { get; set; }

    private string m_Center;
    private float m_Radius;
    private float m_LineLength;
    private float m_LineThickness;
    private float m_SectorRadius;
    private float m_SectorAngle;

    public AbilityRange()
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

    public void SetRadiusAoe(string targetCenter,float radius)
    {
        isSingleTarget = false;
        areaDamageType = AbilityAreaDamageType.Radius;
        m_Center = targetCenter;
        m_Radius = radius;
    }

    public void SetLineAoe(string targetCenter, float lineLength, float lineThickness)
    {
        isSingleTarget = false;
        areaDamageType = AbilityAreaDamageType.Line;
        m_Center = targetCenter;
        m_LineLength = lineLength;
        m_LineThickness = lineThickness;
    }

    public void SetSectorAoe(string targetCenter, float sectorRadius, float sectorAngle)
    {
        isSingleTarget = false;
        areaDamageType = AbilityAreaDamageType.Sector;
        m_Center = targetCenter;
        m_SectorRadius = sectorRadius;
        m_SectorAngle = sectorAngle;
    }
}