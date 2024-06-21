using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;



public class TestRandomCurve : MonoBehaviour {
    // [SerializeField] private BookGroup bookGroup;
    [SerializeField] private Transform target; // 要追踪的目标
    [SerializeField] private float speed = 5f; // 导弹速度
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float controlSphereRadius;
    [SerializeField] private float directionOffset;

    private float percent;
    private Vector3 middlePoint;

    private void OnDrawGizmos() {
        // Gizmos.color = Color.green;
        // Gizmos.DrawWireSphere(controlPoint, 0.5f);
        // // 绘制控制点随机范围
        // Gizmos.color = Color.red;
        // Gizmos.DrawWireSphere(middlePoint, 0.5f);
        // Gizmos.color = Color.white;
        // Gizmos.DrawWireSphere(middlePoint, controlSphereRadius);
    }

    void Start()
    {
        // startPoint = transform.position;
        // endPoint = target.position;
        CalculateRandomControlPoint();
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // 计算导弹朝向目标的方向
        Vector3 direction = (target.position - transform.position).normalized;

        // 更新导弹位置
        // var dis = Vector3.Distance(endPoint, controlPoint) + Vector3.Distance(startPoint, controlPoint);
        // var percentSpeed = speed / dis;
        // percent += percentSpeed * Time.deltaTime;
        // var curveValue = curve.Evaluate(percent);
        // var pos = CalculateBezierPoint(curveValue);
        // transform.position = pos;

        // 朝向目标
        transform.LookAt(target);
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0,Vector3 p1, Vector3 p2) {
        if (t > 1) {
            t = 1;
        }

        var u = 1 - t;
        var tt = t * t;
        var uu = u * u;
        var p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
        return p;
    }

    void CalculateRandomControlPoint()
    {
        // 随机方向向量的长度，这可以影响随机点的距离
        // var direction = target.position - startPoint;
        // middlePoint = startPoint + direction * directionOffset;
        // // 计算随机控制点
        // controlPoint = middlePoint + Random.onUnitSphere * controlSphereRadius;
    }

    // Vector3 CalculateBezierPoint(float t, Vector3 p0,Vector3 p1, Vector3 p2)
    // {
    //     if (t > 1) {
    //         t = 1;
    //     }
    //
    //     float u = 1 - t;
    //     float tt = t * t;
    //     float uu = u * u;
    //     float uuu = uu * u;
    //     float ttt = tt * t;
    //
    //     Vector3 p = uuu * p0; // (1-t)^3 * P0
    //     p += 3 * uu * t * p1; // 3 * (1-t)^2 * t * P1
    //     p += 3 * u * tt * p1; // 3 * (1-t) * t^2 * P2
    //     p += ttt * p2; // t^3 * P3
    //
    //     return p;
    // }


    // [SerializeField] private Transform[] points;
    //
    // /// <summary>
    // /// 一阶贝塞尔
    // /// </summary>
    // /// <param name="t">时间参数，范围0~1</param>
    // /// <returns></returns>
    //
    // public Vector3 lineBezier(float t)
    // {
    //     Vector3 a = points[0].position;
    //     Vector3 b = points[1].position;
    //     return a + (b - a) * t;
    // }
    //
    // /// <summary>
    // /// 二阶贝塞尔
    // /// </summary>
    // /// <param name="t">时间参数，范围0~1</param>
    // /// <returns></returns>
    // public Vector3 quardaticBezier(float t, Vector3 p0,Vector3 p1, Vector3 p2)
    // {
    //     if (t > 1) {
    //         t = 1;
    //     }
    //
    //     Vector3 a = p0;
    //     Vector3 b = p1;
    //     Vector3 c = p2;
    //
    //     Vector3 aa = a + (b - a) * t;
    //     Vector3 bb = b + (c - b) * t;
    //     return aa + (bb - aa) * t;
    // }
    //
    // /// <summary>
    // /// 三阶贝塞尔
    // /// </summary>
    // /// <param name="t">时间参数，范围0~1</param>
    // /// <returns></returns>
    // public Vector3 cubicBezier(float t)
    // {
    //     Vector3 a = points[0].position;
    //     Vector3 b = points[1].position;
    //     Vector3 c = points[2].position;
    //     Vector3 d = points[3].position;
    //
    //     Vector3 aa = a + (b - a) * t;
    //     Vector3 bb = b + (c - b) * t;
    //     Vector3 cc = c + (d - c) * t;
    //
    //     Vector3 aaa = aa + (bb - aa) * t;
    //     Vector3 bbb = bb + (cc - bb) * t;
    //     return aaa + (bbb - aaa) * t;
    // }
    //
    // public static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2) {
    //     if (t > 1) {
    //         t = 1;
    //     }
    //
    //     var u = 1 - t;
    //     var tt = t * t;
    //     var uu = u * u;
    //     var p = uu * p0;
    //     p += 2 * u * t * p1;
    //     p += tt * p2;
    //     return p;
    // }
}