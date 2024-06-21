using UnityEngine;

/// <summary>
/// 贝塞尔曲线
/// </summary>
[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour
{
    /// <summary>
    /// 控制点（包括起始点和终止点）
    /// </summary>
    [SerializeField]
    Transform[] points;

    /// <summary>
    /// 精确度
    /// </summary>
    [SerializeField]
    int accuracy = 20;

    void Update()
    {
        // 绘制贝塞尔曲线
        Vector3 prev_pos = points[0].position;
        for (int i = 0; i <= accuracy; ++i)
        {
            Vector3 to = formula(i / (float)accuracy);
            Debug.DrawLine(prev_pos, to);
            prev_pos = to;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        // 绘制控制点（包括起始点和终止点）
        for (int i = 0; i < points.Length; ++i)
        {
            if (i < points.Length - 1)
            {
                if (4 == points.Length && i == 1)
                {
                    continue;
                }
                Vector3 current = points[i].position;
                Vector3 next = points[i + 1].position;
                
                Gizmos.DrawLine(current, next);
            }
        }
    }

    /// <summary>
    /// 贝塞尔时间公式（二阶、三阶）
    /// </summary>
    /// <param name="t">时间参数，范围0~1</param>
    /// <returns></returns>
    public Vector3 formula(float t)
    {
        switch(points.Length)
        {
            case 3: return quardaticBezier(t);
            case 4: return cubicBezier(t);
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 一阶贝塞尔
    /// </summary>
    /// <param name="t">时间参数，范围0~1</param>
    /// <returns></returns>

    public Vector3 lineBezier(float t)
    {
        Vector3 a = points[0].position;
        Vector3 b = points[1].position;
        return a + (b - a) * t;
    }

    /// <summary>
    /// 二阶贝塞尔
    /// </summary>
    /// <param name="t">时间参数，范围0~1</param>
    /// <returns></returns>
    public Vector3 quardaticBezier(float t)
    {
        Vector3 a = points[0].position;
        Vector3 b = points[1].position;
        Vector3 c = points[2].position;

        Vector3 aa = a + (b - a) * t;
        Vector3 bb = b + (c - b) * t;
        return aa + (bb - aa) * t;
    }

    /// <summary>
    /// 三阶贝塞尔
    /// </summary>
    /// <param name="t">时间参数，范围0~1</param>
    /// <returns></returns>
    public Vector3 cubicBezier(float t)
    {
        Vector3 a = points[0].position;
        Vector3 b = points[1].position;
        Vector3 c = points[2].position;
        Vector3 d = points[3].position;

        Vector3 aa = a + (b - a) * t;
        Vector3 bb = b + (c - b) * t;
        Vector3 cc = c + (d - c) * t;

        Vector3 aaa = aa + (bb - aa) * t;
        Vector3 bbb = bb + (cc - bb) * t;
        return aaa + (bbb - aaa) * t;
    }
}