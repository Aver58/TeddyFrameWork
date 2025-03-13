using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnTriggerEnter : MonoBehaviour {
    private void OnTriggerEnter(Collider other) {
        Debug.LogError($"OnTriggerEnter{gameObject.name}");
    }

    private void OnTriggerExit(Collider other) {
        Debug.LogError($"OnTriggerExit{gameObject.name}");
    }
}
