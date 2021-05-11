#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UIMirror.cs
 Author:      Zeng Zhiwei
 Time:        2021/3/26 10:58:46
=====================================================
*/
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;
//https://zhuanlan.zhihu.com/p/25995971

[AddComponentMenu("UI/Effects/UIMirror", 20)]
[RequireComponent(typeof(Graphic))]
public class UIMirror : BaseMeshEffect
{
    public enum MirrorType
    {
        /// <summary>
        /// 水平
        /// </summary>
        Horizontal,

        /// <summary>
        /// 垂直
        /// </summary>
        Vertical,

        /// <summary>
        /// 四分之一
        /// 相当于水平，然后再垂直
        /// </summary>
        Quarter,
    }

    /// <summary>
    /// 镜像类型
    /// </summary>
    [SerializeField]
    private MirrorType m_MirrorType = MirrorType.Horizontal;
    public MirrorType mirrorType
    {
        get { return m_MirrorType; }
        set
        {
            if(m_MirrorType != value)
            {
                m_MirrorType = value;
                if(graphic != null)
                {
                    graphic.SetVerticesDirty();
                }
            }
        }
    }

    [NonSerialized]
    private RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get { return m_RectTransform ?? (m_RectTransform = GetComponent<RectTransform>()); }
    }

    /// <summary>
    /// 设置原始尺寸
    /// </summary>
    public void SetNativeSize()
    {
        if(graphic != null && graphic is Image)
        {
            Sprite overrideSprite = (graphic as Image).overrideSprite;

            if(overrideSprite != null)
            {
                float w = overrideSprite.rect.width / (graphic as Image).pixelsPerUnit;
                float h = overrideSprite.rect.height / (graphic as Image).pixelsPerUnit;
                rectTransform.anchorMax = rectTransform.anchorMin;

                switch(m_MirrorType)
                {
                    case MirrorType.Horizontal:
                        rectTransform.sizeDelta = new Vector2(w * 2, h);
                        break;
                    case MirrorType.Vertical:
                        rectTransform.sizeDelta = new Vector2(w, h * 2);
                        break;
                    case MirrorType.Quarter:
                        rectTransform.sizeDelta = new Vector2(w * 2, h * 2);
                        break;
                }

                graphic.SetVerticesDirty();
            }
        }
    }

    private void ApplyMirror(List<UIVertex> verts)
    {
        int count = verts.Count;

        //调用相应的镜像处理函数
        if(graphic is Image)
        {
            Image.Type type = (graphic as Image).type;

            switch(type)
            {
                case Image.Type.Simple:
                    DrawSimple(verts, count);
                    break;
                case Image.Type.Sliced:

                    break;
                case Image.Type.Tiled:

                    break;
                case Image.Type.Filled:

                    break;
            }
        }
        else
        {
            DrawSimple(verts, count);
        }
    }

    public void DrawSimple(List<UIVertex> output, int count)
    {
        //获取当前Graphic矩形绘制范围
        Rect rect = graphic.GetPixelAdjustedRect();

        SimpleScale(rect, output, count);

        switch(m_MirrorType)
        {
            case MirrorType.Horizontal:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, true);
                break;
            case MirrorType.Vertical:
                ExtendCapacity(output, count);
                MirrorVerts(rect, output, count, false);
                break;
            case MirrorType.Quarter:
                ExtendCapacity(output, count * 3);
                MirrorVerts(rect, output, count, true);
                MirrorVerts(rect, output, count * 2, false);
                break;
        }
    }

    //List扩容
    protected void ExtendCapacity(List<UIVertex> verts, int addCount)
    {
        var neededCapacity = verts.Count + addCount;
        if(verts.Capacity < neededCapacity)
            verts.Capacity = neededCapacity;
    }

    //将原始顶点进行缩放
    protected void SimpleScale(Rect rect, List<UIVertex> verts, int count)
    {
        for(int i = 0; i < count; i++)
        {
            UIVertex vertex = verts[i];
            Vector3 position = vertex.position;

            //原始顶点的缩放了，把所有顶点往左边挤，
            //其实就是顶点横坐标相对于绘制区最左边宽度减半。
            if(m_MirrorType == MirrorType.Horizontal || m_MirrorType == MirrorType.Quarter)
            {
                position.x = (position.x + rect.x) * 0.5f;
            }

            if(m_MirrorType == MirrorType.Vertical || m_MirrorType == MirrorType.Quarter)
            {
                position.y = (position.y + rect.y) * 0.5f;
            }

            vertex.position = position;

            verts[i] = vertex;
        }
    }

    //把顶点复制一份，以rect.center为对称轴，进行翻转，就可以把右边的部分绘制出来了。
    protected void MirrorVerts(Rect rect, List<UIVertex> verts, int count, bool isHorizontal = true)
    {
        // 倒序去添加点，这样才能绘制正面的顶点
        for(int i = count - 1; i >= 0; i--)
        {
            UIVertex vertex = verts[i];
            Vector3 position = vertex.position;

            if(isHorizontal)
            {
                position.x = rect.center.x * 2 - position.x;
            }
            else
            {
                position.y = rect.center.y * 2 - position.y;
            }

            vertex.position = position;

            verts.Add(vertex);
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if(!IsActive())
            return;

        //这里引用了一个数组对象池的类：ListPool。
        var output = ListPool<UIVertex>.Get();
        //获取所有的顶点数据
        vh.GetUIVertexStream(output);

        ApplyMirror(output);

        vh.Clear();
        //把顶点数据写入
        vh.AddUIVertexTriangleStream(output);
        ListPool<UIVertex>.Release(output);
    }
}