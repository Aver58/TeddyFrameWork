#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleWorkingData.cs
 Author:      Zeng Zhiwei
 Time:        2020\5\18 星期一 23:44:57
=====================================================
*/
#endregion

using TsiU;

public class BattleWorkingData : TBTWorkingData
{
    public AIBehaviorRequest request { get; set; }

    public BattleUnit owner { get; set; }

    public BattleWorkingData(BattleUnit battleEntity)
    {
        owner = battleEntity;
    }
}