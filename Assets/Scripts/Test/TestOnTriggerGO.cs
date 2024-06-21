using System;
using UnityEngine;

public class TestOnTriggerGO : MonoBehaviour {
    public Action<Collider> action;

    private void OnTriggerEnter(Collider other) {
        Debug.LogError(other);

        action?.Invoke(other);
    }
}
