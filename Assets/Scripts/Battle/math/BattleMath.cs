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

public static class BattleMath {
    public static float Vector2Magnitude(float x, float y) {
        return (float)Math.Sqrt(x * x + y * y);
    }

    public static float Vector2SqrMagnitude(float x, float y) {
        return x * x + y * y;
    }

    // 距离平方 x*x + y*y
    public static float GetDistance2DSquare(float x1, float z1, float x2, float z2) {
        return (x1 - x2) * (x1 - x2) + (z2 - z1) * (z2 - z1);
    }

    // 距离
    public static float GetDistance2D(float x1, float z1, float x2, float z2) {
        return (float)Math.Sqrt(GetDistance2DSquare(x1, z1, x2, z2));
    }

    public static float GetDistance3DSquare(Vector3 left, Vector3 right) {
        return GetDistance3DSquare(left.x, left.y, left.z,right.x, right.y, right.z);
    }

    // 距离平方 x*x + y*y + z*z
    public static float GetDistance3DSquare(float x1, float y1, float z1, float x2, float y2, float z2) {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2) + (z2 - z1) * (z2 - z1);
    }

    /// <summary>
    /// 绕y轴旋转矩阵
    /// 顺时针角度为负，逆时针角度为正
    /// </summary>
    public static void RotateByYAxis2D(float x, float z, float radian, out float x1, out float z1) {
        float sinValue = (float)Math.Sin(radian);
        float cosValue = (float)Math.Cos(radian);
        x1 = x * cosValue - z * sinValue;
        z1 = x * sinValue + z * cosValue;
    }

    /// <summary>
    /// 点是否在圆内
    /// 判断点到圆心的距离是否小于半径
    /// </summary>
    public static bool IsPointInCircle(float targetX,float targetZ, float circleCenterX,float circleCenterZ,float circleRadius) {
        var distanceSquare = GetDistance2DSquare(targetX, targetZ, circleCenterX, circleCenterZ);
        return distanceSquare <= circleRadius * circleRadius;
    }

    // 点是否在AABB内,分离轴算法
    public static bool IsPointInRect(Vector2 rectCenter, Vector2 rectForward, float rectLength, float rectHeight, Vector2 pointCenter) {
        return false;
    }
    
    // 圆与矩形AABB 相交
    public static bool IsPointInRect(Vector2 rectCenter, Vector2 rectForward, float rectLength, float rectHeight, Vector2 pointCenter, float pointRadius) {
        if (pointRadius > 0) {
            
        }
        
        return IsPointInRect(rectCenter, rectForward, rectLength, rectHeight, pointCenter);
    }

    // 判断两个矩形是否相交
    // (x1,y1) (x2,y2)是第一个矩形左下和右上角的两个点
    // (x3,y3) (x4,y4)是第二个矩形左下角和右上角的两个点
    public static bool IsRectIntersect(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4) {
        return Math.Max(x1, x3) <= Math.Min(x2, x4) && Math.Max(y1, y3) <= Math.Min(y2, y4);
    }

    // AABB与圆是否相交
    // h = (rectLength/2, rectHeight/2)
    public static bool IsBoxCircleIntersect(Vector2 rectCenter, Vector2 h, Vector2 point, float radius) {
        // 第一步：转换到第一象限
        var v = point - rectCenter;
        v.x = Math.Abs(v.x);
        v.y = Math.Abs(v.y);
        // 第二步：求圆心到矩形的最短距离矢量
        var u = v - h;
        u.x = Math.Max(u.x, 0);
        u.y = Math.Max(u.y, 0);
        // 第三步：长度平方与半径平方比较
        return u.sqrMagnitude <= radius * radius;
    }
}