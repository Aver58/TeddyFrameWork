#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleLogic.cs
 Author:      Zeng Zhiwei
 Time:        2020/5/26 9:31:18
=====================================================
*/
#endregion

using UnityEngine;

public class BattleLogic : Singleton<BattleLogic>
{
    public int logicFrame = 0;
    private int m_UniqueID = 0;
    private BattleEntityManager m_EntityMgr;

    private BattleEntityManager.AIEntityUpdater m_DecisionUpdater;
    private BattleEntityManager.AIEntityUpdater m_RequestUpdater;
    private BattleEntityManager.AIEntityUpdater m_BehaviorUpdater;

    public BattleLogic()
    {
        TableConfig config = new TableConfig();
        config.LoadTableConfig();

        m_EntityMgr = BattleEntityManager.instance;

        m_DecisionUpdater = delegate (BattleEntity entity, float gameTime, float deltaTime)
        {
            return entity.UpdateDecision(gameTime, deltaTime);
        };
        m_RequestUpdater = delegate (BattleEntity entity, float gameTime, float deltaTime)
        {
            return entity.UpdateRequest(gameTime, deltaTime);
        };
        m_BehaviorUpdater = delegate (BattleEntity entity, float gameTime, float deltaTime)
        {
            return entity.UpdateBehavior(gameTime, deltaTime);
        };
    }

    public void Init()
    {
        //AddEnemy();
        AddPlayer();
    }

    private int GetUniqueID()
    {
        m_UniqueID += 1;
        return m_UniqueID;
    }

    private void AddPlayer()
    {
        BattleProperty property = new BattleProperty(npcPropertyTable.Instance.GetTableItem(101));
        property.Level = 1;
        BattleEntity entity = new BattleEntity(GetUniqueID(), BattleCamp.FRIENDLY, property);
        entity.enemyCamp = BattleCamp.ENEMY;

        m_EntityMgr.AddEntity(entity);
    }

    private void AddEnemy()
    {
        BattleProperty property = new BattleProperty(npcPropertyTable.Instance.GetTableItem(102));
        property.Level = 1;
        HeroEntity guardEntity = new HeroEntity(GetUniqueID(), BattleCamp.ENEMY, property);
        guardEntity.enemyCamp = BattleCamp.FRIENDLY;
        m_EntityMgr.AddEntity(guardEntity);
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        float gameTime = Time.realtimeSinceStartup;
        m_EntityMgr.UpdateAbility(gameTime, deltaTime);
        //Update Decision
        m_EntityMgr.IteratorDo(m_DecisionUpdater, gameTime, deltaTime);
        //Update Request
        m_EntityMgr.IteratorDo(m_RequestUpdater, gameTime, deltaTime);
        //Update Behaivor
        m_EntityMgr.IteratorDo(m_BehaviorUpdater, gameTime, deltaTime);

        logicFrame += 1;
    }
}