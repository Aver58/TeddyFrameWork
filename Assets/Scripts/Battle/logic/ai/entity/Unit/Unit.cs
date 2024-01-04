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
public abstract class Unit
{
    private float m_PosX;
    private float m_PosY;
    private float m_PosZ;
    private float m_ForwardX;
    private float m_ForwardY;
    private float m_ForwardZ;
    private Vector2 m_Forward2D;
    private Vector3 m_Forward3D;
    private Vector2 m_Position2D;
    private Vector3 m_Position3D;

    public int id;

    public virtual void OnEnter(){ }
    public virtual void OnExit(){ }
    public virtual void Update(float deltaTime,int logicFrame) { }

    public Unit()
    {
        int id = BattleUnitManager.Instance.GetUniqueID();
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

    public Vector3 Get3DForward()
    {
        m_Forward3D.Set(m_ForwardX, m_ForwardY, m_ForwardZ);
        return m_Forward3D;
    }

    public Vector3 Get3DPosition()
    {
        m_Position3D.Set(m_PosX, m_PosY, m_PosZ);
        return m_Position3D;
    }

    #endregion

    #region set

    public void Set3DPosition(Vector3 pos)
    {
        Set3DPosition(pos.x, pos.y, pos.z);
    }

    public void Set3DPosition(float posX, float posY, float posZ)
    {
        m_PosX = posX;
        m_PosY = posY;
        m_PosZ = posZ;
    }

    public void Set2DForward(Vector2 forward)
    {
        Set2DForward(forward.x, forward.y);
    }

    public void Set3DForward(Vector3 pos)
    {
        Set3DForward(pos.x, pos.y, pos.z);
    }

    public void Set2DForward(float posX, float posZ)
    {
        m_ForwardX = posX;
        m_ForwardZ = posZ;
    }

    public void Set3DForward(float posX, float posY, float posZ)
    {
        if(posX == 0f && posY == 0f && posZ == 0f)
        {
            GameLog.Log("Set3DForward is zero!");
            return;
        }
        m_ForwardX = posX;
        m_ForwardY = posY;
        m_ForwardZ = posZ;
    }

    #endregion
}

