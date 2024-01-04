using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestInt64 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        var data = new Hashtable();
        data["i"] = 1601203754936307712.ToString();
        var test1 = MiniJSON.jsonEncode(data);
        Debug.Log(test1);

        var test2 = JsonMapper.ToJson(data);
        Debug.Log(test2);
        
        var test3 = MiniJSON.jsonDecode(test2) as Hashtable;
        Debug.Log(test3["i"]);

        var info = "{\"customConfig\":[[1592350156353703936,2],[1,2]],\"isInfiniteBullet\":false}";
        var infoTable = JsonMapper.ToObject(info);
        // Debug.Log(infoTable);
        // System.Text.Encoding.UTF8.GetBytes

        var list = new List<string> { "许冰冰", "谢妤铮", "曾志伟" };
        var index = Random.Range(0, list.Count);
        Debug.Log(list[index]);
        list.RemoveAt(index);
        index = Random.Range(0, list.Count);
        Debug.Log(list[index]);
        list.RemoveAt(index);
        Debug.Log(list[0]);

        var hashTable = new Hashtable();
        hashTable["111"] = 111;
        hashTable["222"] = 222;
        var sb = new StringBuilder();
        foreach(string key in hashTable.Keys) {
            sb.Append($"{key}: {hashTable[key]}\r\n");
        }
        Debug.Log(sb.ToString());
        sb.Clear();

        // 反射获取 TestState 的 WeakCanUse 字段
        var weakCanUse = typeof(TestClass.TestState).GetField("WeakCanUse").GetValue(null);
        var weakCanUseInt = Int32.Parse(weakCanUse.ToString());
        Debug.LogError(weakCanUse);
        Debug.LogError(weakCanUseInt);
    }
}

public class TestClass {
    public static class TestState {
        public const byte WeakCanUse = 33;
    }
}