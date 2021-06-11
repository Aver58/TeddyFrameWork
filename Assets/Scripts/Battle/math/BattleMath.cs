#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleMath.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/27 8:59:39
=====================================================
*/
#endregion

using System;
using UnityEngine;

public static class BattleMath
{
    // 距离平方 x*x + y*y
    public static float GetDistance2DSquare(float x1, float z1, float x2, float z2)
    {
        return (x1 - x2) * (x1 - x2) + (z2 - z1) * (z2 - z1);
    }

    // 距离
    public static float GetDistance2D(float x1, float z1, float x2, float z2)
    {
        return (float)Math.Sqrt(GetDistance2DSquare(x1, z1, x2, z2));
    }

    public static float GetDistance3DSquare(Vector3 left, Vector3 right)
    {
        return GetDistance3DSquare(left.x, left.y, left.z,right.x, right.y, right.z);
    }

    // 距离平方 x*x + y*y + z*z
    public static float GetDistance3DSquare(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z2 - z1) * (z2 - z1);
    }

    /// <summary>
    /// 绕y轴旋转矩阵
    /// 顺时针角度为负，逆时针角度为正
    /// </summary>
    public static void RotateByYAxis2D(float x, float z, float radian, out float x1, out float z1)
    {
        float sinValue = (float)Math.Sin(radian);
        float cosValue = (float)Math.Cos(radian);
        x1 = x * cosValue - z * sinValue;
        z1 = x * sinValue + z * cosValue;
    }

    /// <summary>
    /// 点是否在圆内
    /// </summary>
    public static bool IsPointInCircle(float targetX,float targetZ, float circleCenterX,float circleCenterZ,float circleRadius)
    {
        // 判断点到圆心的距离是否小于半径
        var distanceSquare = GetDistance2DSquare(targetX, targetZ, circleCenterX, circleCenterZ);
        if(distanceSquare <= circleRadius * circleRadius)
            return true;

        return false;
    }
}