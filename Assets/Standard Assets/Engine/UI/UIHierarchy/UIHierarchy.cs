#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UIHierarchy.cs
 Author:      Zeng Zhiwei
 Time:        2019/11/19 10:30:33
=====================================================
*/
#endregion

using UnityEngine;
using System.Collections.Generic;

public class UIHierarchy : MonoBehaviour
{
    [System.Serializable]
    public class ItemInfo
    {
        public string name;
        public Object item;

        public ItemInfo() { }
        public ItemInfo(string _name, Object _item) { name = _name; item = _item; }
    }

    [System.Serializable]
    public class EffectItemInfo
    {
        public string name;
        public Object item;
        public Object parent;
        public EffectItemInfo() { }
        public EffectItemInfo(string _name, Object _item, Object _parent)
        {
            name = _name;
            item = _item;
            parent = _parent;
        }
    }

    // 控件
    public List<ItemInfo> widgets;
    public void SetWidgets(List<ItemInfo> data)
    {
        if (data.Count == 0) return;
        if (widgets == null)
        {
            widgets = new List<ItemInfo>();
        }

        widgets.Clear();
        widgets.AddRange(data);
    }

    // 特效控件
    public List<EffectItemInfo> effects;

    public void SetEffects(List<EffectItemInfo> data)
    {
        if (data.Count == 0) return;
        if (effects == null)
        {
            effects = new List<EffectItemInfo>();
        }
        effects.Clear();
        effects.AddRange(data);
    }

    // 外部引用
    [SerializeField]
    public List<ItemInfo> externals;
}

