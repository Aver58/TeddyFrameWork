#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    DrawArea.cs
 Author:      Zeng Zhiwei
 Time:        2020\5\31 星期日 16:01:28
=====================================================
*/
#endregion

using UnityEngine;

public class DebugController: MonoBehaviour
{
    //划线的数量
    private int lineCount = 32;
    //划线使用的材质球
    public Material lineMaterial;
    private bool _isDrawAttackArea = false;
    private bool _isDrawMoveArea = false;
    private bool _isDrawViewArea = false;
    private float _attackRange;
    private float _viewRange;
    private Vector3 _moveCenter;
    private float _moveRange;
    private Color moveAreaColor = new Color(0, 1, 0, 0.1f);
    private Color viewAreaColor = new Color(0, 0, 1, 0.1f);

    void Awake()
    {
        CreateLineMaterial();
    }
    /// <summary>
    /// 创建一个材质球
    /// </summary>
    private void CreateLineMaterial()
    {
        //如果材质球不存在
        if(!lineMaterial)
        {
            //用代码的方式实例一个材质球
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            //设置参数
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //设置参数
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            //设置参数
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    /// <summary>
    /// 画圆面
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    private void DrawCircle(Vector3 center, float radius,Color color = default(Color))
    {
        for(int i = 0; i < lineCount; ++i)
        {
            float a = i / (float)lineCount;
            float lastAngle = ((i - 1) / (float)lineCount) * Mathf.PI * 2;
            float angle = a * Mathf.PI * 2;
            // 设置颜色
            //GL.Color(new Color(a, 1 - a, 0, 0.8F));
            GL.Color(color);
            GL.Vertex(center);
            GL.Vertex3(Mathf.Cos(lastAngle) * radius, 0, Mathf.Sin(lastAngle) * radius);
            GL.Vertex3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
        }
    }

    // 攻击区域
    public void DrawAttackArea(float radius)
    {
        _attackRange = radius;
        _isDrawAttackArea = true;
    }

    // 可见区域
    public void DrawViewArea(float radius)
    {
        _viewRange = radius;
        _isDrawViewArea = true;
    }

    // 活动区域
    public void DrawMoveArea(Vector3 center, float radius)
    {
        _moveCenter = center;
        _moveRange = radius;
        _isDrawMoveArea = true;
    }

    /// <summary>
    /// 使用GL画线的回调
    /// </summary>
    public void OnRenderObject()
    {
        //激活第一个着色器通过（在本例中，我们知道它是唯一的通过）
        lineMaterial.SetPass(0);
        //渲染入栈  在Push——Pop之间写GL代码
        GL.PushMatrix();
        //矩阵相乘，将物体坐标转化为世界坐标
        GL.MultMatrix(transform.localToWorldMatrix);
        // 开始画线  在Begin——End之间写画线方式
        //GL.LINES 画线
        GL.Begin(GL.TRIANGLES);

        if(_isDrawAttackArea)
        {
            DrawCircle(Vector3.zero, _attackRange,Color.red);
        }

        if(_isDrawMoveArea)
        {
            DrawCircle(_moveCenter, _moveRange, moveAreaColor);
        }

        if(_isDrawViewArea)
        {
            DrawCircle(Vector3.zero, _viewRange, viewAreaColor);
        }

        GL.End();
        //渲染出栈
        GL.PopMatrix();
    }
}