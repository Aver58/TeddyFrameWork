using UnityEngine;

/// <summary>
/// LineRenderer控制器
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(BezierCurve))]
public class LineRendererCtrler : MonoBehaviour
{
    [SerializeField]
    int nodeCount = 20;

    [SerializeField]
    LineRenderer lineRenderer;

    [SerializeField]
    BezierCurve bezier;

    void Awake()
    {
        lineRenderer.positionCount = nodeCount + 1;
    }

    void Update()
    {
        // 更新LineRenderer的点
        for (int i = 0; i <= nodeCount; ++i)
        {
            Vector3 to = bezier.formula(i / (float)nodeCount);
            lineRenderer.SetPosition(i, to);
        }
    }
}
