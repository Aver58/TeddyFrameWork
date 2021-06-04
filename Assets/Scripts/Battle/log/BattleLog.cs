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
        GameLog.Log(format, @params);
    }

    public static void LogError(string format,params object[] @params)
    {
        GameLog.LogError(string.Format(format, @params));
    }

    public static void LogNodeOnEnter(string type)
    {
        GameLog.Log($"【{type}】onEnter");
    }

    public static void LogNodeOnExit(string type)
    {
        GameLog.Log($"【{type}】onExit");
    }

    public static void LogRpgBattleAttacker(int logicFrame, BattleUnit caster, BattleUnit victim, string configName, float damage)
    {
        GameLog.Log("[逻辑帧:{0}]（{1}）使用了【{2}】对（{3}）造成伤害：{4}", logicFrame, caster.GetName(), configName, victim.GetName(), damage.ToString());
    }

    public static void LogRpgBattleHealer(int logicFrame, BattleUnit caster, BattleUnit target, string configName, float value)
    {
        GameLog.Log("[逻辑帧:{0}]（{1}）使用了【{2}】对（{3}）造成治疗值：{4}", logicFrame, caster.GetName(), configName, target.GetName(), value.ToString());
    }

    public static void LogTargetDodge(int logicFrame, BattleUnit caster, BattleUnit target, string configName)
    {
        GameLog.Log("[逻辑帧:{0}]（{1}）对（{2}）使用了【{3}】,被成功闪避！", logicFrame, caster.GetName() , target.GetName(), configName);
    }

}