#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleLogicDefine.cs
 Author:      Zeng Zhiwei
 Time:        2020/9/10 19:50:15
=====================================================
*/
#endregion

public enum HeroState
{
    IDLE,                       // 待机状态
    TURN,                       // 转身状态
    MOVE,                       // 移动状态
    DEAD,                       // 死亡状态
    REARY_CAST,                 // 技能准备释放状态
    CASTING,                    // 技能释放状态
    KNOCK_BACK,                 // 击退状态
    VICTORY,                    // 胜利状态
    REINCARNATING,              // 复活中状态
    WATCH,                      // 观战状态
}