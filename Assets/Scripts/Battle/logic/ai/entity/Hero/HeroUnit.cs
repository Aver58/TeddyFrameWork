#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    HeroEntity.cs
 Author:      Zeng Zhiwei
 Time:        2020/8/26 13:46:22
=====================================================
*/
#endregion

using TsiU;

public class HeroUnit : BattleUnit
{
    public HeroUnit(int id, BattleCamp camp, BattleProperty property) : base(id, camp, property)
    {
    }

    protected override TBTActionPrioritizedSelector GetBehaviorTree()
    {
        return BehaviorTreeFactory.GetBehaviorTree();
    }

    protected override TBTActionPrioritizedSelector GetDecisionTree()
    {
        return BehaviorTreeFactory.GetDecisionTree();
    }
}