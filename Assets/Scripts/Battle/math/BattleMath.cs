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
    // x*x + y*y
    public static float Distance2DMagnitude(float leftX, float leftZ, float rightX,float rightZ)
    {
        return (leftX - rightX) * (leftX - rightX) + (rightZ - leftZ) * rightZ - leftZ;
    }

    public static float SqartDistance2DMagnitude(float leftX, float leftZ, float rightX, float rightZ)
    {
        return (float)Math.Sqrt(Distance2DMagnitude(leftX, leftZ, rightX, rightZ));
    }

    public static Vector2 Vector2RotateFromRadian(float forwardX, float forwardZ, float radian)
    {
        //2维向量旋转(逆时针旋转公式) 顺时针角度为负，逆时针角度为正
        float sinValue = (float)Math.Sin(radian);
        float cosValue = (float)Math.Cos(radian);
        float x = forwardX * cosValue - forwardZ * sinValue;
        float z = forwardX * sinValue + forwardZ * cosValue;
        return new Vector2(x,z);
    }
}