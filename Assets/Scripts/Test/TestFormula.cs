using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Random = System.Random;
using Vector3 = UnityEngine.Vector3;

public enum GameMode {
    SportsPartyMode = 1,
    SportsPartyMode_GunFightMode = 2,
    SportsPartyMode_PartyMode = 3,
    SportsPartyMode_ScuffleMode = 4,
    SportsPartyMode_DefuseMode = 5,
}

public struct TeamRoleInfoData {
    public long PlayerId;
    private int schange; // 积分
    public int Schange {
        get => schange;
        set { schange = value; }
    }
}

public class TestFormula : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        var deadNum = 0;
        var killRoleNum = 0;
        var assistsKillNum = 0;
        var uprearRoleNum = 0;
        var damage = 0;
        var teamNum = 1;
        var teamRank = 1;

        var performScoreFloat = GetPerformScore(GameMode.SportsPartyMode_DefuseMode, killRoleNum, assistsKillNum, deadNum, uprearRoleNum, damage);
        var resultScoreFloat = GetResultScore(GameMode.SportsPartyMode_DefuseMode, teamNum, teamRank);

        var factor = 0.05f;
        int performScore = Mathf.RoundToInt(performScoreFloat * factor);
        int resultScore = Mathf.RoundToInt(resultScoreFloat * factor);
        var total = performScore + resultScore;
        Debug.LogError($"factor {factor} performScore {performScore} resultScore {resultScore} total {total}");

        // float result = Mathf.Log((1 + 1) / (3 + 0 + 1), 15);
        // Debug.Log(result);
        //
        // int.TryParse("", out var lost);
        // Debug.LogError(lost);
        // Debug.LogError(Math.Clamp(total, 3, 100));
        //
        // var content = "{\n" +
        //               "\t\"1_2_0\": 1," +
        //               "\t\"1816734938670567438_2\": -12," +
        //               "\t\"23_2_1\": \"1816734938670567446\"\n}";
        // var keyMapDataTable = (Hashtable)MiniJSON.jsonDecode(content);

        // 使用 BigInteger 处理大数字
        // if (keyMapDataTable["23_2_1"] is double)
        // {
        //     BigInteger value = new BigInteger((double)keyMapDataTable["23_2_1"]);
        //     Debug.LogError(value);  // 正确显示为大整数
        // }
        // else
        // {
        //     Debug.LogError(keyMapDataTable["23_2_1"]);
        // }

        // Debug.LogError(keyMapDataTable["23_2_1"]);

        // var teamRoleInfoData = Get();
        // teamRoleInfoData.Schange = 3;
        // Debug.LogError(teamRoleInfoData.Schange);
        // var item = RPC(teamRoleInfoData);
        // Debug.LogError(item.Schange);

        var probabilityConfig = new Dictionary<string, double>
        {
            { "A", 0.5 },
            { "B", 0.3 },
            { "C", 0.2 }
        };

        // 初始化计数器
        var counts = new Dictionary<string, int>
        {
            { "A", 0 },
            { "B", 0 },
            { "C", 0 }
        };

        // 测试随机生成
        for (int i = 0; i < 20; i++)
        {
            string result = GetRandomObject(probabilityConfig);
            counts[result]++;
        }

        Debug.LogError($"A: {counts["A"] / 20f} B: {counts["B"] / 20f} C: {counts["C"] / 20f}");

        // var seconds = FormatTommss3(5000);
        // Debug.LogError(seconds);

        Vector3 originalVector = new Vector3(1.23456f, 7.89101f, 3.45678f);
        Vector3 roundedVector = RoundVector3(originalVector, 2);

        Debug.Log("Original Vector: " + originalVector);
        Debug.Log("Rounded Vector: " + roundedVector);
        Debug.Log("Rounded Vector: " + JsonUtility.ToJson(roundedVector));

        // Debug.Log(int.Parse("01"));
        // Debug.Log(Time.realtimeSinceStartup);
        Debug.Log(Math.Ceiling(2.4f));
    }

    void Update() {
        // Debug.Log(Time.realtimeSinceStartup);
    }

    public static Vector3 RoundVector3(Vector3 vector, int decimalPlaces)
    {
        vector.x = (float)System.Math.Round(vector.x, decimalPlaces);
        vector.y = (float)System.Math.Round(vector.y, decimalPlaces);
        vector.z = (float)System.Math.Round(vector.z, decimalPlaces);
        return vector;
    }

    public static string FormatTommss3(double  totalSeconds) {
        int minutes = (int)(totalSeconds / 60);
        int seconds = (int)(totalSeconds % 60);
        int milliseconds = (int)((totalSeconds - Math.Floor(totalSeconds)) * 1000);
        if (minutes >= 100) {
            return "99:99.999";
        }

        return $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
    }

    private static Random random = new Random();

    public static string GetRandomObject(Dictionary<string, double> probabilityConfig) {
        // 计算总和，确保所有概率相加为1
        double totalProbability = 0;
        foreach (var kvp in probabilityConfig) {
            totalProbability += kvp.Value;
        }

        // 生成一个0到总和之间的随机数
        double randomValue = random.NextDouble() * totalProbability;

        double cumulativeProbability = 0.0;

        // 根据概率选择对象
        foreach (var kvp in probabilityConfig) {
            cumulativeProbability += kvp.Value;
            if (randomValue <= cumulativeProbability) {
                return kvp.Key;
            }
        }

        // 如果没有匹配（不太可能发生），返回默认值或抛出异常
        throw new InvalidOperationException("Random selection failed due to probability configuration.");
    }

    public TeamRoleInfoData Get() {
        var teamRoleInfoData = new TeamRoleInfoData {
            PlayerId = 1,
            Schange = 2
        };

        return teamRoleInfoData;
    }

    public TeamRoleInfoData RPC(TeamRoleInfoData teamRoleInfoData) {
        Debug.LogError(teamRoleInfoData.Schange);
        teamRoleInfoData.Schange = 4;
        return teamRoleInfoData;
    }

    private static float GetPerformScore(GameMode gameMode, int killRoleNum, int assistsKillNum, int deadNum, int uprearRoleNum, float damage) {
        switch (gameMode) {
            case GameMode.SportsPartyMode_DefuseMode:
                var scoreTimes = 0;
                // Rank=-330*LOG((0.5*死亡+1)/(0.25*击杀+0.25*助攻+1),10)+LOG(爆破模式得分次数+1,2)*60
                return -330 * Mathf.Log((0.5f * deadNum + 1f) / (0.25f * killRoleNum + 0.25f * assistsKillNum + 1f), 10) + Mathf.Log(scoreTimes + 1f, 2f) * 60f;
            case GameMode.SportsPartyMode_GunFightMode:
                // 综合表现 Rank=-200*LOG((死亡+1)/(击杀+助攻+1),15)+(总伤害/300-1)*18
                return -200 * Mathf.Log((0.2f * deadNum + 1f) / (killRoleNum + assistsKillNum + 1f), 15) + (damage / 300 - 1) * 18;
            case GameMode.SportsPartyMode:
                // 综合表现 Rank=-330*LOG((0.2*死亡+1)/(0.25*击杀+0.25*助攻+1),10)+LOG(扶人+1,2)*20
                return -330 * Mathf.Log((0.2f * deadNum + 1f) / (0.25f * killRoleNum + 0.25f * assistsKillNum + 1f), 10) + Mathf.Log(uprearRoleNum + 1, 2) * 20;
            case GameMode.SportsPartyMode_PartyMode:
                // 综合表现 Rank=-105*LOG((死亡+1)/(0.5*击杀+0.25*助攻+1),10)
                return -105 * Mathf.Log((deadNum + 1f) / (0.5f * killRoleNum + 0.25f * assistsKillNum + 1f), 10);
            case GameMode.SportsPartyMode_ScuffleMode:
                // 综合表现 Rank=-112*LOG((死亡+1)/(0.5*击杀+0.5*助攻+1),10)
                return -112 * Mathf.Log((deadNum + 1f) / (0.5f * killRoleNum + 0.5f * assistsKillNum + 1f), 10);
        }

        return 0;
    }

    private static float GetResultScore(GameMode gameMode, long teamNum, int teamRank) {
        switch (gameMode) {
            case GameMode.SportsPartyMode_GunFightMode:
                // Rank=LOG(((队伍人数+1)/队伍排名),2)*25
                return Mathf.Log((teamNum + 1) / teamRank, 2) * 25f;
            case GameMode.SportsPartyMode:
                // Rank=LOG(((队伍人数+1)/队伍排名),2)*32.5
                return Mathf.Log((teamNum + 1) / teamRank, 2) * 32.5f;
            case GameMode.SportsPartyMode_PartyMode:
                // Rank=Log(((队伍人数+1)/队伍排名),2)*23.8
                return Mathf.Log((teamNum + 1) / teamRank, 2) * 23.8f;
            case GameMode.SportsPartyMode_ScuffleMode:
                // Rank=LOG(((队伍人数+1)/队伍排名),2)*27
                return Mathf.Log((teamNum + 1) / teamRank, 2) * 27f;
        }

        return 0;
    }
}