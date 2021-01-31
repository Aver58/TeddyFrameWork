#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BattleLog.cs
 Author:      Zeng Zhiwei
 Time:        2020/6/22 8:58:55
=====================================================
*/
#endregion

public static class BattleLog
{
    public static void Log(string format, params object[] @params)
    {
        Debug.Log(string.Format(format, @params));
    }

    public static void LogError(string format,params object[] @params)
    {
        Debug.LogError(string.Format(format, @params));
    }

    public static void LogNodeOnEnter(string type)
    {
        Debug.Log($"【{type}】onEnter");
    }

    public static void LogNodeOnExit(string type)
    {
        Debug.Log($"【{type}】onExit");
    }

    public static void LogRpgBattleAttacker(int logicFrame, BattleUnit caster, BattleUnit victim, string configName, float damage)
    {
        Debug.Log("[逻辑帧:{0}]（{1}）使用了【{2}】对（{3}）造成伤害：{4}", logicFrame, caster.GetName(), configName, victim.GetName(), damage.ToString());
    }
}