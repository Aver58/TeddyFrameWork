#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    TargetCollection.cs
 Author:      Zeng Zhiwei
 Time:        2021/2/1 16:07:08
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class TargetCollection
{
    public List<BattleUnit> units;
    public List<Vector2> points;
    public AbilityRequestTargetType targetType;

    public TargetCollection()
    {
        units = new List<BattleUnit>();
        points = new List<Vector2>();
    }

    public void ClearTargets()
    {
        units.Clear();
    }

    public void AddUnit(BattleUnit battleUnit)
    {
        units.Add(battleUnit);
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
    }
}