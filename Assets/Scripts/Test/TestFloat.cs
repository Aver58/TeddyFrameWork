using System;
using UnityEngine;

public class TestFloat : MonoBehaviour {
    public void Start() {
        // double d = 169580.95872d;
        // var f = (float)d;
        //
        // Debug.LogError($"{Math.Round(d, 3):F3}");
        //
        // Debug.LogError($"{Convert.ToSingle(d):F3}");
        // Debug.LogError($"{f:F3}");
        // Debug.LogError($"{f*1000:F3}");
        // Debug.LogError($"{(float)Math.Round(d, 3):F3}");
        //
        //
        // Debug.LogError($"{float.MinValue}");
        // Debug.LogError($"{float.MaxValue}");

        double d = 1.0e10; // 超出 float 范围的值
        d = 83459338;
        if (d >= float.MinValue && d <= float.MaxValue)
        {
            float f = (float)d;
            Debug.LogError($"{f:F2}");
        }
        else
        {
            Debug.LogError("超出 float 范围");
        }
    }
}