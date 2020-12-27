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

public enum HeroState{
    IDLE = 1,                       // 待机状态
    TURN = 2,                       // 转身状态
    MOVE = 3,                       // 移动状态
    DEAD = 4,                       // 死亡状态
    CASTING = 5,                    // 技能释放状态
    KNOCK_BACK = 6,                 // 击退状态
    VICTORY = 7,                    // 胜利状态
    REINCARNATING = 8,              // 复活中状态
    WATCH = 9,                      // 观战状态
}