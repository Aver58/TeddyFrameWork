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

public class HeroEntity : BattleEntity
{
    public HeroEntity(int id, BattleCamp camp, BattleProperty property) : base(id, camp, property)
    {
    }

    protected override TBTAction GetBehaviorTree()
    {
        return BehaviorTreeFactory.GetBehaviorTree();
    }

    protected override TBTAction GetDecisionTree()
    {
        return BehaviorTreeFactory.GetDecisionTree();
    }
}