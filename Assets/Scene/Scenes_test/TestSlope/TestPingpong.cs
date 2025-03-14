using UnityEngine;

public class TestPingpong : MonoBehaviour {
    private Vector3 pointA;
    private Vector3 targetPoint;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private float speed = 2f; // 移动速度
    [SerializeField] private float serverTime;

    void Start() {
        pointA = transform.position;
        var curPos = Vector3.zero;
        float length = Vector3.Distance(pointA, pointB);
        float totalTime = length / speed;
        var remainder = serverTime % (2 * totalTime);
        float timeProgress = remainder / totalTime;
        if (timeProgress <= 1) {
            // 正向
            curPos = Vector3.Lerp(pointA, pointB, timeProgress);
            targetPoint = pointB;
        } else {
            // 反向
            curPos = Vector3.Lerp(pointB, pointA, timeProgress - 1);
            targetPoint = pointA;
        }

        transform.position = curPos;
    }

    void Update() {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPoint) < 0.01f) {
            targetPoint = targetPoint == pointA? pointB : pointA; // 改变方向
        }
    }
}