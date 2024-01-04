using UnityEngine;

public class TestSnapping : MonoBehaviour {
    public Vector3 targetPosition;
    public Collider targetCollider;
    public Collider myCollider;
    public float snapDistance = 1;

    void Update() {
        // transform.position = targetPosition;
        // 面吸附
        Vector3 myClosestPoint = myCollider.ClosestPoint(targetCollider.transform.position);
        Vector3 targetClosestPoint = targetCollider.ClosestPoint(myClosestPoint);
        Vector3 offset = targetClosestPoint - myClosestPoint;
        if (offset.magnitude < snapDistance) {
            transform.position += offset;
        }
        // 怎么在这基础上加上边缘对齐呢？
    }
}
