using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnTrigger : MonoBehaviour {
    public GameObject go;
    public float second;

    void Start() {
        go.GetComponent<TestOnTriggerGO>().action += collider1 => {
            var time = TimeSpan.FromSeconds(second);
            var formattedTime = $@"{time:mm\:ss\.fff}";
            Debug.LogError(formattedTime);
        };

        Vector3 originalVector = new Vector3(1.12345f, 2.67891f, 3.45678f);
        Vector3 truncatedVector = TruncateVector3(originalVector, 3);

        Debug.Log("Original Vector: " + originalVector);
        Debug.Log("Truncated Vector: " + truncatedVector.x);
        Debug.Log("Truncated Vector: " + truncatedVector.y);
        Debug.Log("Truncated Vector: " + truncatedVector.z);
    }

    Vector3 TruncateVector3(Vector3 vector, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        vector.x = Mathf.Round(vector.x * multiplier) / multiplier;
        vector.y = Mathf.Round(vector.y * multiplier) / multiplier;
        vector.z = Mathf.Round(vector.z * multiplier) / multiplier;
        return vector;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
