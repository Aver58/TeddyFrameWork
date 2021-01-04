#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Unit.cs
 Author:      Zeng Zhiwei
 Time:        2020/3/5 10:22:48
=====================================================
*/
#endregion

using UnityEngine;

// 数据层
// 维护不依赖表现的真实位置坐标
public class Entity
{
    private float m_PosX;
    private float m_PosY;
    private float m_PosZ;
    private float m_ForwardX;
    private float m_ForwardY;
    private float m_ForwardZ;
    private Vector2 m_Forward2D;
    private Vector2 m_Position2D;
    private Vector3 m_Position3D;
    public int id { get; set; }

    public void OnEnter(){ }
    public void OnExit(){ }
    public void Update(float deltaTime,int logicFrame) { }

    public Entity(int id)
    {
        this.id = id;
    }

    #region get

    public void Get2DPosition(out float posX,out float posZ)
    {
        posX = m_PosX;
        posZ = m_PosZ;
    }

    public Vector2 Get2DPosition()
    {
        m_Position2D.Set(m_PosX, m_PosZ);
        return m_Position2D;
    }

    public Vector2 Get2DForward()
    {
        m_Forward2D.Set(m_ForwardX, m_ForwardZ);
        return m_Forward2D;
    }

    public Vector3 Get3DPosition()
    {
        m_Position3D.Set(m_PosX, m_PosY, m_PosZ);
        return m_Position3D;
    }

    #endregion

    #region set
    // todo 改成set都是设置3d，get可以取2d或者3d
    public void Set3DPosition(Vector3 pos)
    {
        m_PosX = pos.x;
        m_PosY = pos.y;
        m_PosZ = pos.z;
    }

    public void Set3DPosition(float posX, float posY, float posZ)
    {
        m_PosX = posX;
        m_PosY = posY;
        m_PosZ = posZ;
    }

    public void Set2DPosition(float posX, float posZ)
    {
        m_PosX = posX;
        m_PosZ = posZ;
    }

    public void Set3DForward(Vector3 pos)
    {
        m_ForwardX = pos.x;
        m_ForwardY = pos.y;
        m_ForwardZ = pos.z;
    }

    public void Set3DForward(float posX, float posY, float posZ)
    {
        m_ForwardX = posX;
        m_ForwardY = posY;
        m_ForwardZ = posZ;
    }

    public void Set2DForward(float posX, float posZ)
    {
        m_ForwardX = posX;
        m_ForwardZ = posZ;
    }

    public void Set2DForward(Vector2 forward)
    {
        m_ForwardX = forward.x;
        m_ForwardZ = forward.y;
    }
    #endregion

}

