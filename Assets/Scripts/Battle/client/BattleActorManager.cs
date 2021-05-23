#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleActorManager.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/14 22:01:55
=====================================================
*/
#endregion

using System.Collections.Generic;

public class BattleActorManager : Singleton<BattleActorManager>
{
    private Dictionary<int,HeroActor> m_ActorMap;
    private List<HeroActor> m_Temps;

    public BattleActorManager()
    {
        m_ActorMap = new Dictionary<int, HeroActor>();
        m_Temps = new List<HeroActor>();
    }

    public void AddActor(int hash,HeroActor e)
    {
        m_ActorMap.Add(hash,e);
    }

    public List<HeroActor> GetActors(List<BattleUnit> entities)
    {
        m_Temps = new List<HeroActor>();
        foreach(var entity in entities)
        {
            HeroActor actor;
            if(m_ActorMap.TryGetValue(entity.hash, out actor))
            {
                m_Temps.Add(actor);
            }
        }
        return m_Temps;
    }

    public List<HeroActor> GetEnemyActors()
    {
        m_Temps = new List<HeroActor>();
        foreach(var keyValuePair in m_ActorMap)
        {
            var actor = keyValuePair.Value;
            if(actor.battleUnit.camp == BattleCamp.ENEMY)
            {
                m_Temps.Add(actor);
            }
        }
        return m_Temps;
    }
}