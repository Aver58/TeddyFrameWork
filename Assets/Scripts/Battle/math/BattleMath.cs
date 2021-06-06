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
    public static float GetDistance2DSquare(float leftX, float leftZ, float rightX, float rightZ)
    {
        return (leftX - rightX) * (leftX - rightX) + (rightZ - leftZ) * (rightZ - leftZ);
    }

    // 距离
    public static float GetDistance2D(float leftX, float leftZ, float rightX, float rightZ)
    {
        return (float)Math.Sqrt(GetDistance2DSquare(leftX, leftZ, rightX, rightZ));
    }

    public static float GetDistance3DSquare(Vector3 left, Vector3 right)
    {
        return GetDistance3DSquare(left.x, left.y, left.z,right.x, right.y, right.z);
    }

    // 距离平方 x*x + y*y + z*z
    public static float GetDistance3DSquare(float leftX, float leftY, float leftZ, float rightX, float rightY, float rightZ)
    {
        return (leftX - rightX) * (leftX - rightX) + (leftY - rightY) * (leftY - rightY) + (rightZ - leftZ) * (rightZ - leftZ);
    }

    /// <summary>
    /// 绕y轴旋转矩阵
    /// 顺时针角度为负，逆时针角度为正
    /// </summary>
    public static void RotateByYAxis(float x, float y, float z, float radian,out float x1, ref float y1, out float z1)
    {
        float sinValue = (float)Math.Sin(radian);
        float cosValue = (float)Math.Cos(radian);
        x1 = x * cosValue - z * sinValue;
        z1 = x * sinValue + z * cosValue;
    }

}