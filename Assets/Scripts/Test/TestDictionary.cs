using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestDictionary : MonoBehaviour
{
    private Dictionary<int, int> map = new Dictionary<int, int>() {
        {1, 2},
        {2, 3},
        {3, 4}
    };
    // Start is called before the first frame update
    void Start() {
        var keys = map.Keys.ToArray();

        foreach (var VARIABLE in keys) {
            map.Remove(VARIABLE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
