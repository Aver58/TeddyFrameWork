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

public class BattleEntityManager : Singleton<BattleEntityManager>
{
    //--------------------------------------------------------------------------------------
    public delegate int AIEntityUpdater(BattleUnit entity, float gameTime, float deltaTime);
    //--------------------------------------------------------------------------------------

    private List<BattleUnit> m_Entites;
    private List<BattleUnit> m_Temps;

    public BattleEntityManager()
    {
        m_Entites = new List<BattleUnit>();
        m_Temps = new List<BattleUnit>();
    }

    public void IteratorDo(AIEntityUpdater updater, float gameTime, float deltaTime)
    {
        foreach(BattleUnit e in m_Entites)
        {
            updater(e, gameTime, deltaTime);
        }
    }

    public void UpdateAbility(float gameTime, float deltaTime)
    {
        foreach(BattleUnit entity in m_Entites)
        {
            if(!entity.IsDead())
            {
                entity.UpdateAbility(gameTime, deltaTime);
            }
        }
    }

    public void AddEntity(BattleUnit e)
    {
        m_Entites.Add(e);
    }

    public List<BattleUnit> GetEntities(BattleCamp battleCamp)
    {
        m_Temps = new List<BattleUnit>(0);
        foreach(BattleUnit entity in m_Entites)
        {
            if(entity.camp == battleCamp)
            {
                m_Temps.Add(entity);
            }
        }
        return m_Temps;
    }

    //todo 删除了
    public BattleUnit GetMyEntity()
    {
        foreach(BattleUnit entity in m_Entites)
        {
            if(entity.camp == BattleCamp.FRIENDLY)
            {
                return entity;
            }
        }
        return null;
    }
}