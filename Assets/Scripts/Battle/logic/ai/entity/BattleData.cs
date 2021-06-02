#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleData.cs
 Author:      Zeng Zhiwei
 Time:        2021/6/1 17:35:35
=====================================================
*/
#endregion

using Aver3;

public class BattleData : BTData
{
    public float deltaTime { get; set; }
    public float gameTime { get; set; }
    public BehaviorRequest request { get; set; }
    public BattleUnit owner { get; set; }

    public BattleData(BattleUnit battleEntity)
    {
        owner = battleEntity;
    }
}