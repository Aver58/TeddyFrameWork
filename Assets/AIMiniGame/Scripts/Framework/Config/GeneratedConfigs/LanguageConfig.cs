using System.Collections.Generic;

[System.Serializable]
public class LanguageConfig : BaseConfig {
    public string id;
    public string value;

    public override void Parse(string[] values, string[] headers) {
        for (int i = 0; i < headers.Length; i++) {
            var header = headers[i].Replace("\r", "");
            switch (header) {
                case "id":
                    id = values[i];
                    break;
                case "value":
                    value = values[i];
                    break;
            }
        }
    }

    private static Dictionary<string, LanguageConfig> cachedConfigs;
    public static LanguageConfig Get(string key) {
        if (cachedConfigs == null) {
            // 相对位置
            var relativePath = $"Localization/{LocalizationFeature.CurrentLanguage}/Language.csv";
            cachedConfigs = ConfigManager.Instance.LoadConfig<LanguageConfig>(relativePath);
        }

        LanguageConfig config = null;
        if (cachedConfigs != null) {
            cachedConfigs.TryGetValue(key, out config);
        }

        return config;
    }
}
