using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Book {
    [Tooltip("开始坐标")] public Vector3 StartPoint;
    [Tooltip("结束坐标")] public Vector3 EndPoint;
    [NonSerialized][Tooltip("控制点坐标")] public Vector3 ControlPoint;

    [Tooltip("开始旋转")] public Quaternion StartRotation;
    [Tooltip("结束旋转")] public Quaternion EndRotation;
}

[CreateAssetMenu(menuName = "TestSerializable/MagicLevelBook")]
public class MagicLevelBookSO : ScriptableObject {
    [Tooltip("整组书实例化坐标")] public Vector3 StartPosition;
    [Tooltip("整组书实例化旋转")] public Quaternion StartRotation;
    [Tooltip("飞行导弹速度")] public float FlySpeed;
    [Tooltip("飞行曲线控制")] public AnimationCurve AnimationCurve;
    [Tooltip("控制点在方向上的偏移")] public float ControlPointDirectionOffset;
    [Tooltip("控制点的随机球形半价")] public float ControlPointSphereRadius;

    [Tooltip("书本")]public List<Book> books;
}