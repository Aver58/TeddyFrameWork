using System;
using System.Collections.Generic;

[System.Serializable]
public class PetMapConfig : BaseConfig {
    public int id;
    public string[] petSprites;

    public override void Parse(string[] values, string[] headers) {
        for (int i = 0; i < headers.Length; i++) {
            var header = headers[i].Replace("\r", "");
            switch (header) {
                case "id":
                    id = int.Parse(values[i]);
                    break;
                case "petSprites":
                    petSprites = values[i].Split(';');
                    break;
            }
        }
    }

    private static Dictionary<string, PetMapConfig> cachedConfigs;
    public static PetMapConfig Get(string key) {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<PetMapConfig>("PetMap.csv");
        }

        PetMapConfig config = null;
        if (cachedConfigs != null) {
            cachedConfigs.TryGetValue(key, out config);
        }

        return config;
    }

    public static List<string> GetKeys() {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<PetMapConfig>("PetMap.csv");
        }

        return cachedConfigs != null ? new List<string>(cachedConfigs.Keys) : new List<string>();
    }
}
