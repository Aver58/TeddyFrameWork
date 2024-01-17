//--------------------------------------------------------
//    [Author]:               Sausage
//    [  Date ]:             2024年1月13日
//--------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System;
using UnityEngine;

public partial class UIAtlasConfig {

    public readonly int Id;
	public readonly string AtlasName;

    

    public UIAtlasConfig(string input) {
        try {
            var tables = input.Split('\t');
            int.TryParse(tables[0],out Id); 
			AtlasName = tables[1];
        } catch (Exception ex) {
            Debug.LogError(ex);
        }
    }

    static Dictionary<string, UIAtlasConfig> configs = null;
    public static UIAtlasConfig Get(string id) {
        if (!Inited) {
            Init(true);
        }

        if (string.IsNullOrEmpty(id)) {
            return null;
        }

        if (configs.ContainsKey(id)) {
            return configs[id];
        }

        UIAtlasConfig config = null;
        if (rawDatas.ContainsKey(id)) {
            config = configs[id] = new UIAtlasConfig(rawDatas[id]);
            rawDatas.Remove(id);
        }

        if (config == null) {
            Debug.LogFormat("获取配置失败 UIAtlasConfig id:{0}", id);
        }

        return config;
    }

    public static UIAtlasConfig Get(int id) {
        return Get(id.ToString());
    }

    public static bool Has(string id) {
        if (!Inited) {
            Init(true);
        }

        return configs.ContainsKey(id) || rawDatas.ContainsKey(id);
    }

    public static List<string> GetKeys() {
        if (!Inited) {
            Init(true);
        }

        var keys = new List<string>();
        keys.AddRange(configs.Keys);
        keys.AddRange(rawDatas.Keys);
        return keys;
    }

    public static bool Inited { get; private set; }
    protected static Dictionary<string, string> rawDatas = null;
    public static void Init(bool sync = false) {
        Inited = false;
        var path = AssetUtility.GetDataTableAsset("UIAtlasConfig", false);
        var lines = File.ReadAllLines(path);
        configs = new Dictionary<string, UIAtlasConfig>();

        if (sync) {
            rawDatas = new Dictionary<string, string>(lines.Length - 3);
            for (var i = 3; i < lines.Length; i++) {
                var line = lines[i];
                var index = line.IndexOf("\t");
                var id = line.Substring(0, index);

                rawDatas.Add(id, line);
            }
            Inited = true;
        } else {
            ThreadPool.QueueUserWorkItem((object @object) => {
                rawDatas = new Dictionary<string, string>(lines.Length - 3);
                for (var i = 3; i < lines.Length; i++) {
                    var line = lines[i];
                    var index = line.IndexOf("\t");
                    var id = line.Substring(0, index);

                    rawDatas.Add(id, line);
                }

                Inited = true;
            });
        }
    }

}