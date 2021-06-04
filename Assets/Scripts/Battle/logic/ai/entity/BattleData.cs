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
    public float deltaTime;
    public float gameTime;
    public BattleUnit owner;
    public BehaviorRequest request;

    public BattleData(BattleUnit battleEntity)
    {
        owner = battleEntity;
    }
}