using System;
using System.Collections.Generic;

[System.Serializable]
public class TestFileConfig : BaseConfig {
    public string id;
    public float[] playerSpeeds;
    public int[] maxScores;
    public string[] gameTitles;

    public override void Parse(string[] values, string[] headers) {
        for (int i = 0; i < headers.Length; i++) {
            var header = headers[i].Replace("\r", "");
            switch (header) {
                case "id":
                    id = values[i];
                    break;
                case "playerSpeeds":
                    playerSpeeds = Array.ConvertAll(values[i].Split(';'), float.Parse);
                    break;
                case "maxScores":
                    maxScores = Array.ConvertAll(values[i].Split(';'), int.Parse);
                    break;
                case "gameTitles":
                    gameTitles = values[i].Split(';');
                    break;
            }
        }
    }

    private static Dictionary<string, TestFileConfig> cachedConfigs;
    public static TestFileConfig Get(string key) {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<TestFileConfig>("TestFile.csv");
        }

        TestFileConfig config = null;
        if (cachedConfigs != null) {
            cachedConfigs.TryGetValue(key, out config);
        }

        return config;
    }

    public static List<string> GetKeys() {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<TestFileConfig>("TestFile.csv");
        }

        return cachedConfigs != null ? new List<string>(cachedConfigs.Keys) : new List<string>();
    }
}
