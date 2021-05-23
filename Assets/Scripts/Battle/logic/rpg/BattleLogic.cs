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
        AddPlayer();
    }

    private int GetUniqueID()
    {
        m_UniqueID += 1;
        return m_UniqueID;
    }

    private void AddPlayer()
    {
        BattleProperty property = new BattleProperty(NPCPropertyTable.Instance.GetTableItem(101));
        BattleUnit entity = new BattleUnit(GetUniqueID(), BattleCamp.FRIENDLY, property);
        entity.enemyCamp = BattleCamp.ENEMY;

        m_UnitMgr.AddPlayer(entity);
    }

    #region API

    /// <summary>
    /// 有行为树，不攻击的玩偶
    /// 人偶也要有行为树，要可以受击、冰冻、眩晕、中毒、灼烧等状态
    /// </summary>
    public BattleUnit AddOneDummyUnit()
    {
        var property = new BattleProperty(NPCPropertyTable.Instance.GetTableItem(102));
        var unit = new BattleUnit(GetUniqueID(), BattleCamp.ENEMY, property);
        unit.enemyCamp = BattleCamp.FRIENDLY;
        m_UnitMgr.AddUnit(unit);
        return unit;
    }

    /// <summary>
    /// 有行为树，会攻击，会寻路的AI
    /// </summary>
    public BattleUnit AddOneEnemyUnit()
    {
        BattleProperty property = new BattleProperty(NPCPropertyTable.Instance.GetTableItem(102));
        HeroUnit unit = new HeroUnit(GetUniqueID(), BattleCamp.ENEMY, property);
        unit.enemyCamp = BattleCamp.FRIENDLY;
        m_UnitMgr.AddUnit(unit);
        return unit;
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

    #endregion

}