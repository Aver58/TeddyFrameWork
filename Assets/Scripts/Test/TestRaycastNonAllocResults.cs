using UnityEngine;

public class TestRaycastNonAllocResults : MonoBehaviour
{
    private RaycastHit[] hits = new RaycastHit[10];

    void Start() {
        Physics.RaycastNonAlloc(Vector3.zero, Vector3.forward, hits, 10);
        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].collider != null) {
                Debug.LogError(hits[i].collider);
            }
        }
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Physics.RaycastNonAlloc(Vector3.zero, Vector3.forward, hits, 10);
            for (int i = 0; i < hits.Length; i++) {
                if (hits[i].collider != null) {
                    Debug.LogError(hits[i].collider);
                }
            }
        }
    }
}
