using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace StarForce.Editor {
    public class CollectSpriteAtlas {
        private const string DynamicAtlasPath = "Assets\\GameMain\\UI\\UIAtlas";
        private const string DynamicSpritePath = "Assets\\GameMain\\UI\\UISprites";
        private const string DynamicAtlasConfigPath = "Assets\\GameMain\\DataTables\\UIAtlasConfig.txt";
        private const string DynamicSpriteConfigPath = "Assets\\GameMain\\DataTables\\UISpriteConfig.txt";

        // 图集映射
        [MenuItem("Tools/CollectAtlasConfig")]
        public static void CollectAtlasConfig() {
            var collector = new AtlasCollector();
            var guids = AssetDatabase.FindAssets("t:spriteatlas", new string[] {
                DynamicAtlasPath
            });

            for (int i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var assetName = Path.GetFileNameWithoutExtension(path);
                collector.AddItem(i, assetName);
            }

            collector.GenerateConfig(DynamicAtlasConfigPath);
            AssetDatabase.Refresh();
        }

        // icon映射图集索引
        [MenuItem("Tools/CollectSpriteConfig")]
        public static void CollectSpriteConfig() {
            var collector = new SpriteCollector();
            var guids = AssetDatabase.FindAssets("t:sprite", new string[] {
                DynamicSpritePath
            });

            for (int i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var folderPath = Path.GetDirectoryName(path);
                string folderName = Path.GetFileName(folderPath);
                var assetName = Path.GetFileNameWithoutExtension(path);
                var atlasId = GetAtlasId(folderName);
                collector.AddItem(assetName, atlasId);
            }

            collector.GenerateConfig(DynamicSpriteConfigPath);
            AssetDatabase.Refresh();
        }

        private static Dictionary<string, int> atlasIdMap;
        private static int GetAtlasId(string name) {
            // 读取图集配置
            if (atlasIdMap == null) {
                atlasIdMap = new Dictionary<string, int>();
                if (!File.Exists(DynamicAtlasConfigPath)) {
                    Log.Error("没有找到指定文件：" + DynamicAtlasConfigPath);
                    return -1;
                }

                var lines = File.ReadAllLines(DynamicAtlasConfigPath);
                for (int i = 4; i < lines.Length; i++) {
                    var line = lines[i];
                    UIAtlasConfig config = new UIAtlasConfig();
                    config.ParseDataRow(line, null);
                    atlasIdMap.Add(config.AtlasName, config.Id);
                }
            }

            if (atlasIdMap.TryGetValue(name, out int id)) {
                return id;
            }

            Log.Error("UIAtlasConfig 没有找到指定图集：" + name);
            return -1;
        }
    }
}