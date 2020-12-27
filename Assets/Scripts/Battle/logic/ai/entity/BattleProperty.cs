#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleProperty.cs
 Author:      Zeng Zhiwei
 Time:        2020/3/5 14:08:40
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class BattleProperty
{
    #region 客户端数据
    public int id { get; set; }
    public int curHP { get; set; }
    public int curEnergy{ get; set; }
    public int Level { get; set; }

    #endregion

    private NPCPropertyItem m_BaseValue;

    public BattleProperty(NPCPropertyItem item)
    {
        m_BaseValue = item;

        curHP = GetMaxHP();
        curEnergy = 0;
    }

    // 承受伤害
    public void UpdateHP(float damage)
    {
        curHP += Mathf.CeilToInt(damage);

        curHP = Mathf.Clamp(curHP, 0, GetMaxHP());
        GameMsg.instance.SendMessage(GameMsgDef.BattleEntity_HP_Updated, new HeorHPUpdateEventArgs(id, curHP,GetMaxHP()));
    }

    public void UpdateEnergy(int energy)
    {
        curEnergy += energy;
    }

    #region Config

    public int GetID()
    {
        return m_BaseValue.id;
    }

    public List<int> GetSkillList()
    {
        return m_BaseValue.skillList;
    }

    public int GetMaxHP()
    {
        return m_BaseValue.maxHp;
    }

    public int GetMaxMP()
    {
        return m_BaseValue.maxMp;
    }

    public int GetPhysic()
    {
        return m_BaseValue.physic;
    }

    public int GetMagic()
    {
        return m_BaseValue.magic;
    }

    public int GetAttackRange()
    {
        return m_BaseValue.attackRange;
    }

    public float GetMaxRadius()
    {
        return m_BaseValue.maxRadius;
    }

    public void GetStartPoint(out float x, out float y)
    {
        x = m_BaseValue.startPoint[0];
        y = m_BaseValue.startPoint[1];
    }

    public Vector3 GetStartPoint()
    {
        return new Vector3(m_BaseValue.startPoint[0], 0, m_BaseValue.startPoint[1]);
    }

    public int GetViewRange()
    {
        return m_BaseValue.viewRange;
    }

    public string GetModelPath()
    {
        return m_BaseValue.modelPath;
    }

    public int GetMoveSpeed()
    {
        return m_BaseValue.moveSpeed;
    }

    public int GetTurnSpeed()
    {
        return m_BaseValue.turnSpeed;
    }

    #endregion

    public object GetValue(string propertyName)
    {
        return m_BaseValue.GetType().GetProperty(propertyName).GetValue(this); //todo test
    }
}