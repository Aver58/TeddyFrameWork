using System.Collections;
using LitJson;
using UnityEngine;

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
        Debug.Log(infoTable);
        // System.Text.Encoding.UTF8.GetBytes
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
