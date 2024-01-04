using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class TestToJson : MonoBehaviour {
    private void Start() {
        Dictionary<int, List<Vector3>> data = new Dictionary<int, List<Vector3>>();
        data.Add(1, new List<Vector3>());
        data[1].Add(Vector3.zero);
        data[1].Add(Vector3.one);
        data[1].Add(Vector3.forward);
        var jsonString = JsonMapper.ToJson(data);
        Debug.Log($"{jsonString}");
        // var jsonDataHashTable =  JsonMapper.ToObject<Dictionary<string, string>>(jsonString);
        // foreach (string key in jsonDataHashTable.Keys) {
        //     var value = jsonDataHashTable[key];
        //     Debug.Log($"{key}: {value}");
        // }
    }
}