#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

public class TestPhysics : MonoBehaviour {
    public CharacterController controller;
    Action onDrawGizmos;
    private void OnDrawGizmos() {
        onDrawGizmos?.Invoke();
        onDrawGizmos = null;
    }

    RaycastHit[] hits = new RaycastHit[10];
    Collider[] colliders = new Collider[10];
    void Update() {
        Array.Clear(hits, 0, 10);
        Array.Clear(colliders, 0, 10);
        // Physics.SphereCastNonAlloc(transform.position, 0.5f, Vector3.down, hits, 2, LayerMask.GetMask("Default"));
        // for (int i = 0; i < hits.Length; i++) {
        //     var hit = hits[i];
        //     if (hit.transform != null) {
        //         onDrawGizmos += () => {
        //             Handles.Label(transform.position + new Vector3(0,1,0) , "hit" + hit.transform);
        //             Handles.Label(transform.position + new Vector3(0,1,0) , "hit Normal" + hit.normal);
        //
        //             Debug.DrawLine(transform.position + hit.normal, transform.position + hit.normal *2,Color.green);
        //         };
        //         break;
        //     }
        // }

        if (controller != null) {
            controller.enableOverlapRecovery = true;
        }

        Physics.OverlapBoxNonAlloc(transform.position, Vector3.one/2, colliders, transform.rotation);
        VisualizeOverlapBox(transform.position, Vector3.one/2, transform.rotation, Color.red);
        for (int i = 0; i < colliders.Length; i++) {
            var hit = colliders[i];
            if (hit != null) {
                onDrawGizmos += () => {
                    Handles.Label(transform.position + new Vector3(0,1,0) , "hit: " + hit.transform);
                    // Handles.Label(transform.position + new Vector3(0,1,0) , "hit Normal" + hit.normal);

                    // Debug.DrawLine(transform.position + hit.normal, transform.position + hit.normal *2,Color.green);
                };
                break;
            }
        }
    }

    public static void VisualizeOverlapBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, Color color)
    {
        // 计算盒子的8个顶点
        Vector3[] corners = new Vector3[8];

        corners[0] = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z);
        corners[1] = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, -halfExtents.z);
        corners[2] = center + orientation * new Vector3(halfExtents.x, -halfExtents.y, halfExtents.z);
        corners[3] = center + orientation * new Vector3(-halfExtents.x, -halfExtents.y, halfExtents.z);

        corners[4] = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z);
        corners[5] = center + orientation * new Vector3(halfExtents.x, halfExtents.y, -halfExtents.z);
        corners[6] = center + orientation * new Vector3(halfExtents.x, halfExtents.y, halfExtents.z);
        corners[7] = center + orientation * new Vector3(-halfExtents.x, halfExtents.y, halfExtents.z);

        // 绘制盒子的12条边
        Debug.DrawLine(corners[0], corners[1], color);
        Debug.DrawLine(corners[1], corners[2], color);
        Debug.DrawLine(corners[2], corners[3], color);
        Debug.DrawLine(corners[3], corners[0], color);

        Debug.DrawLine(corners[4], corners[5], color);
        Debug.DrawLine(corners[5], corners[6], color);
        Debug.DrawLine(corners[6], corners[7], color);
        Debug.DrawLine(corners[7], corners[4], color);

        Debug.DrawLine(corners[0], corners[4], color);
        Debug.DrawLine(corners[1], corners[5], color);
        Debug.DrawLine(corners[2], corners[6], color);
        Debug.DrawLine(corners[3], corners[7], color);
    }

    public void OnCollisionEnter(Collision other) {
        // todo 为啥不进呢？
        Debug.LogError($"{gameObject.name} OnCollisionEnter {other}");
    }
}

#endif
