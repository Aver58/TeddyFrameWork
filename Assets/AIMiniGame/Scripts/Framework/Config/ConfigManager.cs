using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ConfigManager {
    private static ConfigManager instance;
    private static string classDirectoryPath = Application.dataPath + @"\AIMiniGame\Scripts\Framework\Resource\Config\GeneratedConfigs\";
    private static string configDirectoryPath = "Assets/AIMiniGame/ToBundle/Config/";

    public static ConfigManager Instance {
        get {
            if (instance == null) {
                instance = new ConfigManager();
            }
            return instance;
        }
    }

    public Dictionary<string, T> LoadConfig<T>(string fileName) where T : BaseConfig, new() {
        string fileFullPath = GetRelativePath(fileName);
        // if (!Addressables.ResourceLocators.Any(locator => locator.Locate(fileFullPath, typeof(TextAsset), out _))) {
        //     Debug.LogError($"The file '{fileFullPath}' is not found in Addressables.");
        //     return null;
        // }

        //File.ReadAllText(filePath); 改成 Addressables 同步加载，会阻塞进程
        string csvContent = Addressables.LoadAssetAsync<TextAsset>(fileFullPath).WaitForCompletion().text;
        var lines = csvContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 4) {
            Debug.LogError($"Config file has no data: {fileFullPath}");
            return null;
        }

        var headers = lines[0].Split(',');
        Dictionary<string, T> configDict = new Dictionary<string, T>();

        for (int i = 3; i < lines.Length; i++) {
            var values = lines[i].Split(',');
            T config = new T();
            config.Parse(values, headers);
            configDict[values[0]] = config;
        }

        return configDict;
    }

    private string GetRelativePath(string fileName) {
        return Path.Combine(configDirectoryPath, fileName);
    }
}
