using System;
using System.Collections.Generic;

[System.Serializable]
public class UIViewDefineConfig : BaseConfig {
    public string key;
    public string name;
    public string assetName;
    public int uILayer;

    public override void Parse(string[] values, string[] headers) {
        for (int i = 0; i < headers.Length; i++) {
            var header = headers[i].Replace("\r", "");
            switch (header) {
                case "Key":
                    key = values[i];
                    break;
                case "Name":
                    name = values[i];
                    break;
                case "AssetName":
                    assetName = values[i];
                    break;
                case "UILayer":
                    uILayer = int.Parse(values[i]);
                    break;
            }
        }
    }

    private static Dictionary<string, UIViewDefineConfig> cachedConfigs;
    public static UIViewDefineConfig Get(string key) {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<UIViewDefineConfig>("UIViewDefine.csv");
        }

        UIViewDefineConfig config = null;
        if (cachedConfigs != null) {
            cachedConfigs.TryGetValue(key, out config);
        }

        return config;
    }

    public static List<string> GetKeys() {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<UIViewDefineConfig>("UIViewDefine.csv");
        }

        return cachedConfigs != null ? new List<string>(cachedConfigs.Keys) : new List<string>();
    }
}
