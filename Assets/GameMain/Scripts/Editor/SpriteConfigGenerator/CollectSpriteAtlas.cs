using System;
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
            atlasIdMap = null;
        }

        private static Dictionary<string, int> atlasIdMap;

        private static int GetAtlasId(string name) {
            if (atlasIdMap == null) {
                atlasIdMap = new Dictionary<string, int>();
                if (!File.Exists(DynamicAtlasConfigPath)) {
                    Log.Error("没有找到指定文件：" + DynamicAtlasConfigPath);
                    return -1;
                }

                var keys = UIAtlasConfig.GetKeys();
                for (int i = 0; i < keys.Count; i++) {
                    var config = UIAtlasConfig.Get(keys[i]);
                    ;
                    atlasIdMap.Add(config.AtlasName, config.Id);
                }
            }

            if (atlasIdMap.TryGetValue(name, out int id)) {
                return id;
            }

            Log.Error("UIAtlasConfig 没有找到指定图集：" + name);
            return -1;
        }

        private static string tableRoot = Application.dataPath + "/ToBundle/Config/Txt";

        // private static void CollectPrefabsForEffect(string root, string txtName) {
        //     Dictionary<string, string> visualForDeviceDic = new Dictionary<string, string>();
        //     var contents = new List<List<string>>();
        //     var filePath = tableRoot + "/{0}/" + txtName + ".txt";
        //     try {
        //         var guids = AssetDatabase.FindAssets("t:prefab", new string[] {
        //             root
        //         });
        //         var total = guids.Length;
        //         var index = 0;
        //         var tableIndex = 0;
        //         var localizationEffectDict = new Dictionary<string, List<string>>();
        //         foreach (var guid in guids) {
        //             var path = AssetDatabase.GUIDToAssetPath(guid);
        //             var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //             var suffix = Suffix(asset.name);
        //             var original = "";
        //             var visualForDevice = 0.ToString();
        //             if (suffix.Equals("_Low")) {
        //                 original = asset.name.Replace("_Low", "");
        //                 visualForDevice = 1.ToString();
        //             }
        //
        //             if (suffix.Equals("_Mid")) {
        //                 original = asset.name.Replace("_Mid", "");
        //                 visualForDevice = 2.ToString();
        //             }
        //
        //             if (!visualForDeviceDic.ContainsKey(original)) {
        //                 visualForDeviceDic.Add(original, visualForDevice);
        //             }
        //             else {
        //                 visualForDeviceDic[original] = visualForDevice;
        //             }
        //         }
        //
        //         foreach (var guid in guids) {
        //             EditorUtility.DisplayProgressBar("Collect", string.Format("{0}/{1}", ++index, total),
        //                 (float)index / total);
        //             var path = AssetDatabase.GUIDToAssetPath(guid);
        //             var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        //             string suffix = Suffix(asset.name);
        //             if (suffix.Equals("_Low") || suffix.Equals("_Mid")) {
        //                 continue;
        //             }
        //
        //             tableIndex++;
        //             var values = new List<string>();
        //             contents.Add(values);
        //             values.Add(asset.name);
        //             path = path.Replace(root + "/", "");
        //             var lastSeparatorIndex = path.LastIndexOf("/");
        //             if (lastSeparatorIndex != -1) {
        //                 path = path.Remove(lastSeparatorIndex);
        //                 values.Add(path);
        //             }
        //             else {
        //                 values.Add("");
        //             }
        //
        //             values.Add(visualForDeviceDic.ContainsKey(asset.name) ? visualForDeviceDic[asset.name] : "0");
        //         }
        //
        //         var lines = new string[3 + contents.Count];
        //         lines[0] = "string\tstring\tint";
        //         lines[1] = "id\tpath\tvisualForDevice";
        //         lines[2] = "唯一标识\t相对路径\t视效分级";
        //         var isCN = sign == "CN";
        //         for (var i = 0; i < contents.Count; i++) {
        //             var assetName = contents[i][0];
        //             if (!isCN && localizationEffectDict.ContainsKey(assetName)) {
        //                 var newContents = contents[i].ToArray();
        //                 if (sign != "CH") {
        //                     if (localizationEffectDict[assetName].Contains(sign)) {
        //                         newContents[1] = contents[i][1].Replace("CN", sign);
        //                     }
        //                     else if (localizationEffectDict[assetName].Contains("OT")) {
        //                         newContents[1] = contents[i][1].Replace("CN", "OT");
        //                     }
        //                 }
        //
        //                 lines[3 + i] = string.Join("\t", newContents);
        //             }
        //             else {
        //                 lines[3 + i] = string.Join("\t", contents[i].ToArray());
        //             }
        //         }
        //
        //         var path = string.Format(filePath, sign);
        //         var directory = Path.GetDirectoryName(path);
        //         if (!Directory.Exists(directory)) {
        //             Directory.CreateDirectory(directory);
        //         }
        //
        //         File.WriteAllLines(path, lines);
        //     }
        //     catch (Exception e) {
        //         Debug.LogError(e);
        //     }
        //     finally {
        //         EditorUtility.ClearProgressBar();
        //         AssetDatabase.Refresh();
        //     }
        // }

        private static string Suffix(string name) {
            if (name.Length > 4) {
                return name.Substring(name.Length - 4);
            }
            else {
                return "";
            }
        }
    }
}