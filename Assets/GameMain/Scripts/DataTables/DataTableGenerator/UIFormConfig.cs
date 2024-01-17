//--------------------------------------------------------
//    [Author]:               Sausage
//    [  Date ]:             2024年1月14日
//--------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System;
using UnityEngine;

public partial class UIFormConfig {

    public readonly int Id;
	public readonly string Remark;
	public readonly string AssetName;
	public readonly string UIGroupName;
	public readonly bool AllowMultiInstance;
	public readonly bool PauseCoveredUIForm;

    

    public UIFormConfig(string input) {
        try {
            var tables = input.Split('\t');
            int.TryParse(tables[0],out Id); 
			Remark = tables[1];
			AssetName = tables[2];
			UIGroupName = tables[3];
			var AllowMultiInstanceTemp = 0;
			int.TryParse(tables[4],out AllowMultiInstanceTemp); 
			AllowMultiInstance=AllowMultiInstanceTemp!=0;
			var PauseCoveredUIFormTemp = 0;
			int.TryParse(tables[5],out PauseCoveredUIFormTemp); 
			PauseCoveredUIForm=PauseCoveredUIFormTemp!=0;
        } catch (Exception ex) {
            Debug.LogError(ex);
        }
    }

    static Dictionary<string, UIFormConfig> configs = null;
    public static UIFormConfig Get(string id) {
        if (!Inited) {
            Init(true);
        }

        if (string.IsNullOrEmpty(id)) {
            return null;
        }

        if (configs.ContainsKey(id)) {
            return configs[id];
        }

        UIFormConfig config = null;
        if (rawDatas.ContainsKey(id)) {
            config = configs[id] = new UIFormConfig(rawDatas[id]);
            rawDatas.Remove(id);
        }

        if (config == null) {
            Debug.LogFormat("获取配置失败 UIFormConfig id:{0}", id);
        }

        return config;
    }

    public static UIFormConfig Get(int id) {
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
        var path = AssetUtility.GetDataTableAsset("UIFormConfig", false);
        var lines = File.ReadAllLines(path);
        configs = new Dictionary<string, UIFormConfig>();

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