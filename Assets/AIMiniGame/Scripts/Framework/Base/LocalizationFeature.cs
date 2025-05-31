using UnityEngine;

public static class LocalizationFeature {
    public static SystemLanguage CurrentLanguage;
    private const string LanguageKey = "CurrentLanguage";

    public static void Init() {
        InitCurrentLanguage();
    }

    private static void InitCurrentLanguage() {
        // 获取缓存，没有的话，初始化设备语言
        if (PlayerPrefs.HasKey(LanguageKey)) {
            CurrentLanguage = (SystemLanguage)PlayerPrefs.GetInt(LanguageKey);
        } else {
            CurrentLanguage = Application.systemLanguage;
            SetCurrentLanguage(CurrentLanguage);
        }
    }

    public static void SetCurrentLanguage(SystemLanguage language) {
        PlayerPrefs.SetInt(LanguageKey, (int)language);
    }

    // string字符串拓展
    public static string Translate(this string key) {
        var config = LanguageConfig.Get(key);
        if (config != null) {
            return config.value;
        }
        return key;
    }

    public static string GetLanguageName(SystemLanguage systemLanguage) {
        switch (systemLanguage) {
            case SystemLanguage.Chinese:
                return "中文";
            case SystemLanguage.English:
                return "English";
        }

        return null;
    }
}
