#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleEntityManager.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/19 13:53:37
=====================================================
*/
#endregion

using System.Collections.Generic;

public class BattleUnitManager : MonoSingleton<BattleUnitManager>
{
    //--------------------------------------------------------------------------------------
    public delegate int AIEntityUpdater(BattleUnit entity, float gameTime, float deltaTime);
    //--------------------------------------------------------------------------------------

    private int m_UniqueID = 0;
    private List<BattleUnit> m_Units;
    private List<BattleUnit> m_Temps;
    public BattleUnit playerUnit;

    public BattleUnitManager()
    {
        m_Units = new List<BattleUnit>();
        m_Temps = new List<BattleUnit>();
    }

    public void IteratorDo(AIEntityUpdater updater, float gameTime, float deltaTime)
    {
        foreach(BattleUnit e in m_Units)
        {
            updater(e, gameTime, deltaTime);
        }
    }

    public void UpdateAbility(float gameTime, float deltaTime)
    {
        foreach(BattleUnit unit in m_Units)
        {
            if(!unit.IsDead())
            {
                unit.UpdateAbility(gameTime, deltaTime);
            }
        }
    }

    public int GetUniqueID()
    {
        m_UniqueID += 1;
        return m_UniqueID;
    }

    public void AddPlayer(BattleUnit e)
    {
        playerUnit = e;
        AddUnit(e);
    }

    public void AddUnit(BattleUnit e)
    {
        m_Units.Add(e);
    }

    public List<BattleUnit> GetEntities(BattleCamp battleCamp)
    {
        m_Temps = new List<BattleUnit>(0);
        foreach(BattleUnit entity in m_Units)
        {
            if(entity.camp == battleCamp)
                m_Temps.Add(entity);
        }
        return m_Temps;
    }
}