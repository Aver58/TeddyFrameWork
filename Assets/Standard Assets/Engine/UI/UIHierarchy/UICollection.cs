#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UICollection.cs
 Author:      Zeng Zhiwei
 Time:        2021\2\19 星期五 21:59:58
=====================================================
*/
#endregion

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UICollection组件集合
/// </summary>
public sealed class UICollection : MonoBehaviour
{
    [SerializeField]
    public List<Component> components = new List<Component>();
    public T GetComponent<T>(int index) where T : Object
    {
        return components[index] as T;
    }
}