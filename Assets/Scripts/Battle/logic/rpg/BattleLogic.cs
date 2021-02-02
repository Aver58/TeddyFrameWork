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
    private BattleUnitManager m_UnitMgr;

    private BattleUnitManager.AIEntityUpdater m_DecisionUpdater;
    private BattleUnitManager.AIEntityUpdater m_RequestUpdater;
    private BattleUnitManager.AIEntityUpdater m_BehaviorUpdater;

    public BattleLogic()
    {
        TableConfig config = new TableConfig();
        config.LoadTableConfig();

        m_UnitMgr = BattleUnitManager.instance;

        m_DecisionUpdater = delegate (BattleUnit entity, float gameTime, float deltaTime)
        {
            return entity.UpdateDecision(gameTime, deltaTime);
        };
        m_RequestUpdater = delegate (BattleUnit entity, float gameTime, float deltaTime)
        {
            return entity.UpdateRequest(gameTime, deltaTime);
        };
        m_BehaviorUpdater = delegate (BattleUnit entity, float gameTime, float deltaTime)
        {
            return entity.UpdateBehavior(gameTime, deltaTime);
        };
    }

    public void Init()
    {
        AddEnemy();
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
        BattleUnit entity = new BattleUnit(GetUniqueID(), BattleCamp.FRIENDLY, property);
        entity.enemyCamp = BattleCamp.ENEMY;

        m_UnitMgr.AddPlayer(entity);
    }

    private void AddEnemy()
    {
        BattleProperty property = new BattleProperty(npcPropertyTable.Instance.GetTableItem(102));
        property.Level = 1;
        HeroUnit guardEntity = new HeroUnit(GetUniqueID(), BattleCamp.ENEMY, property);
        guardEntity.enemyCamp = BattleCamp.FRIENDLY;
        m_UnitMgr.AddUnit(guardEntity);
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        float gameTime = Time.realtimeSinceStartup;
        m_UnitMgr.UpdateAbility(gameTime, deltaTime);
        //Update Decision
        m_UnitMgr.IteratorDo(m_DecisionUpdater, gameTime, deltaTime);
        //Update Request
        m_UnitMgr.IteratorDo(m_RequestUpdater, gameTime, deltaTime);
        //Update Behaivor
        m_UnitMgr.IteratorDo(m_BehaviorUpdater, gameTime, deltaTime);

        logicFrame += 1;
    }
}