using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigManager {
    private static ConfigManager instance;
    private static string classDirectoryPath = Application.dataPath + @"\AIMiniGame\Scripts\Framework\Resource\Config\GeneratedConfigs\";
    private static string configDirectoryPath = Application.dataPath + @"\AIMiniGame\ToBundle\Config\";

    public static ConfigManager Instance {
        get {
            if (instance == null) {
                instance = new ConfigManager();
            }
            return instance;
        }
    }

    public Dictionary<string, T> LoadConfig<T>(string fileName) where T : BaseConfig, new() {
        string filePath = GetRelativePath(fileName);
        if (!File.Exists(filePath)) {
            Debug.LogError($"Config file not found: {filePath}");
            return null;
        }

        string csvContent = File.ReadAllText(filePath);
        var lines = csvContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 4) {
            Debug.LogError($"Config file has no data: {filePath}");
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
        // todo bundle
        return configDirectoryPath + fileName;
    }
}
