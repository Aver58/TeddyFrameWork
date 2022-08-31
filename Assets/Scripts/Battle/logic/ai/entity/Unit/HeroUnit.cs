#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Entity.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/26 13:46:22
=====================================================
*/
#endregion

using Aver3;

// 有行为树
public class HeroUnit : BattleUnit
{
    public HeroUnit(BattleCamp camp, BattleProperty property) : base(camp, property){}

    protected override BTAction GetBehaviorTree()
    {
        return BehaviorTreeFactory.instance.GetMobaBehaviorTree();
    }

    protected override BTAction GetDecisionTree()
    {
        //BehaviorTreeFactory.GetDecisionTree()
        // 玩家没有决策树，决策都是输入触发，比如移动、释放技能
        return null;
    }
}