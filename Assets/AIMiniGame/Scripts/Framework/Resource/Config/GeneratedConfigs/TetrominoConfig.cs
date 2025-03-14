using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TetrominoConfig : BaseConfig {
    public string id;
    public int[,] shape;
    public Color color;

    public override void Parse(string[] values, string[] headers) {
        for (int i = 0; i < headers.Length; i++) {
            var header = headers[i].Replace("\r", "");
            switch (header) {
                case "id":
                    id = values[i];
                    break;
                case "shape":
                    var rows = values[i].Split('|');
                    shape = new int[4, 4];
                    for (int row = 0; row < rows.Length; row++) {
                        var cols = rows[row].Split(';');
                        for (int column = 0; column < cols.Length; column++) {
                            shape[row, column] = int.Parse(cols[column]);
                        }
                    }
                    break;
                case "color":
                    var colorStr = values[i].Replace("\r", "");
                    ColorUtility.TryParseHtmlString(colorStr, out color);
                    break;
            }
        }
    }

    private static Dictionary<string, TetrominoConfig> cachedConfigs;
    public static TetrominoConfig Get(string key) {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<TetrominoConfig>("Tetromino.csv");
        }

        TetrominoConfig config = null;
        if (cachedConfigs != null) {
            cachedConfigs.TryGetValue(key, out config);
        }

        return config;
    }

    public static List<string> GetKeys() {
        if (cachedConfigs == null) {
            cachedConfigs = ConfigManager.Instance.LoadConfig<TetrominoConfig>("Tetromino.csv");
        }

        return cachedConfigs != null ? new List<string>(cachedConfigs.Keys) : new List<string>();
    }
}
