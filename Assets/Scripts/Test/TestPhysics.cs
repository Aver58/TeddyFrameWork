using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestPhysics : MonoBehaviour {
    void Start() {


    }

    Action onDrawGizmos;
    private void OnDrawGizmos() {
        onDrawGizmos?.Invoke();
        onDrawGizmos = null;
    }

    RaycastHit[] hits = new RaycastHit[10];
    void Update() {
        Array.Clear(hits, 0, 10);
        Physics.SphereCastNonAlloc(transform.position, 0.5f, Vector3.down, hits,
            2, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits.Length; i++) {
            var hit = hits[i];
            if (hit.transform != null) {
                onDrawGizmos += () => {
                    Handles.Label(transform.position + new Vector3(0,1,0) , "hit" + hit.transform);
                    Handles.Label(transform.position + new Vector3(0,1,0) , "hit Normal" + hit.normal);

                    Debug.DrawLine(transform.position + hit.normal, transform.position + hit.normal *2,Color.green);
                };
                break;
            }
        }

    }
}
