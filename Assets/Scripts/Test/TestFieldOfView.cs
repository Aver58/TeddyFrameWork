using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Interface that needs to be implemented by any object that gets affected by the Field of View of the player.
/// </summary>
public interface IHideable {
    void OnFOVEnter();
    void OnFOVLeave();
}

public class Hideable : MonoBehaviour, IHideable {
    private MeshRenderer render;

    private void Awake() {
        OnFOVLeave();
    }

    public void OnFOVEnter() {
        if (render == null)
            render = GetComponent<MeshRenderer>();
        render.enabled = true;
    }

    public void OnFOVLeave() {
        if (render == null)
            render = GetComponent<MeshRenderer>();
        render.enabled = false;
    }
}

[RequireComponent(typeof(MeshFilter))]
public class TestFieldOfView : MonoBehaviour {
    [Header("视野设置")]
    [Tooltip("玩家可以看到的半径或最大距离")] public float viewRadius = 50f;
    [Range(0, 360), Tooltip("视野角度")] public float viewAngle = 90f;

    [Header("边缘解析设置")] [Tooltip("边缘分解算法的迭代（更高=更精确但也更昂贵）")]
    public int edgeResolveIterations = 1;
    public float edgeDstThreshold;

    [Header("常规设置")]
    [Range(0, 1), Tooltip("视场更新之间的延迟,隔多少秒设置一次扫描物体的隐藏显示")]
    public float delayBetweenFOVUpdates = 0.2f;

    [Header("层级设置")]
    [Tooltip("进入/离开视野时受到影响的物体。它们必须实现iHidable接口")] public LayerMask targetMask;
    [Tooltip("阻挡视野的对象")] public LayerMask obstacleMask;

    [Header("可视化设置")]
    [Tooltip("视野可视化吗？")] public bool visualizeFieldOfView = true;
    [Tooltip("影响重新计算视场时射出的射线数量。光线投射计数=视角*网格分辨率")]
    public float meshResolution = 1;

    [Header("周边视野设置")]
    [Tooltip("玩家是否有周边视野?")] public bool hasPeripheralVision = false;
    [Tooltip("玩家用其周边视觉所能看到的最大半径距离.")] public float viewRadiusPeripheralVision = 10f;
    [Tooltip("影响重新计算玩家周边视野时投射出的射线数量。价值越高，成本就越高！光线投射计数")] public int meshResolutionPeripheralVision = 10;

    private MeshFilter viewMeshFilter;
    private Mesh viewMesh;

    //变量在DrawFieldOfView方法中使用（在这里存储效率更高-GC.collect…）
    private List<Vector3> viewPoints = new List<Vector3>();

    private void Start() {
        viewMeshFilter = GetComponent<MeshFilter>();
        viewMesh = new Mesh { name = "View Mesh" };
        viewMeshFilter.mesh = viewMesh;
    }

    void OnEnable() {
        // StartCoroutine("FindTargetsWithDelay", delayBetweenFOVUpdates);
    }

    private void LateUpdate() {
        if (visualizeFieldOfView) {
            viewMeshFilter.mesh = viewMesh;
            DrawFieldOfView();
        } else {
            viewMeshFilter.mesh = null;
        }
    }

    private readonly List<int> triangles = new List<int>();
    private readonly List<Vector3> vertices = new List<Vector3>();

    /// <summary>
    /// 画出视野
    /// </summary>
    private void DrawFieldOfView() {
        viewPoints.Clear();
        var oldViewCast = new ViewCastInfo();
        for (int i = 0; i <= Mathf.RoundToInt(viewAngle * meshResolution); i++) {
            var newViewCast = ViewCast(transform.eulerAngles.y - viewAngle / 2 + (viewAngle / Mathf.RoundToInt(viewAngle * meshResolution)) * i, viewRadius);
            if (i > 0) {
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDstThreshold)) {
                    var edge = FindEdge(oldViewCast, newViewCast, viewRadius);
                    if (edge.pointA != Vector3.zero) {
                        viewPoints.Add(edge.pointA);
                    }

                    if (edge.pointB != Vector3.zero) {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        /* 计算周边视野 */
        if (hasPeripheralVision && viewAngle < 360) {
            //把较短的光线投射到周围，以确保他总是能从各个方向看一点东西
            for (int i = 0; i < meshResolutionPeripheralVision + 1; i++) {
                ViewCastInfo newViewCast = ViewCast(transform.eulerAngles.y + viewAngle / 2 + i * (360 - viewAngle) / meshResolutionPeripheralVision, viewRadiusPeripheralVision);
                if (i > 0) {
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDstThreshold)) {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast, viewRadiusPeripheralVision);
                        if (edge.pointA != Vector3.zero) {
                            viewPoints.Add(edge.pointA);
                        }

                        if (edge.pointB != Vector3.zero) {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }
        }

        /* 画出网格 */
        int vertexCount = viewPoints.Count + 1;
        vertices.Clear();
        triangles.Clear();
        vertices.Add(Vector3.zero);
        for (int i = 0; i < vertexCount - 1; i++) {
            vertices.Add(transform.InverseTransformPoint(viewPoints[i]));
            if (i < vertexCount - 2) {
                triangles.Add(0);
                triangles.Add(i + 1);
                triangles.Add(i + 2);
            }
        }

        viewMesh.Clear();
        viewMesh.SetVertices(vertices);
        viewMesh.SetTriangles(triangles, 0);
        viewMesh.RecalculateNormals();
    }

    /// <summary>
    /// 以给定的角度投射光线，结果返回ViewCastInfo结构。
    /// </summary>
    /// <param name="globalAngle">每条射线的角度</param>
    /// <returns></returns>
    ViewCastInfo ViewCast(float globalAngle, float viewRadius) {
        var dir = DirFromAngle(globalAngle, true);
        if (Physics.Raycast(transform.position, dir, out RaycastHit hit, viewRadius, obstacleMask)) {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        } else {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    /// <summary>
    /// 找到碰撞体的边缘
    /// </summary>
    /// <param name="minViewCast"></param>
    /// <param name="maxViewCast"></param>
    /// <returns></returns>
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float viewRadius) {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++) {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle, viewRadius);
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.point;
            } else {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    /// <summary>
    /// 每1秒运行一次FindVisibleTargets方法
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator FindTargetsWithDelay(float delay) {
        while (true) {
            FindVisibleTargets();
            yield return new WaitForSeconds(delay);
        }
    }

    Collider[] targetsInViewRadius = new Collider[10];

    /// <summary>
    /// 查找所有可见目标并将其添加到“可见目标”列表中.
    /// </summary>
    void FindVisibleTargets() {
        int length = Physics.OverlapSphereNonAlloc(transform.position, viewRadius, targetsInViewRadius, targetMask);
        //在执行下一次FixedUpdate之前，对碰撞体的改动不会立即同步到物理场景
        // Physics.autoSyncTransforms = false;

        /* check normal field of view */
        for (int i = 0; i < length; i++) {
            Transform target = targetsInViewRadius[i].transform;
            bool isInFOV = false;
            //检查是否应该隐藏
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2) {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) {
                    isInFOV = true;
                }
            } else if (hasPeripheralVision) {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                // 这里我们必须检查到目标的距离，因为周围的视野可能有不同于正常视野的半径
                if (dstToTarget < viewRadiusPeripheralVision && !Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) {
                    isInFOV = true;
                }
            }

            //apply effect to IHideable
            IHideable hideable;
            //target.TryGetComponent(out hideable);............................................
            hideable = target.GetComponent<IHideable>();
            if (hideable != null) {
                if (isInFOV) {
                    hideable.OnFOVEnter();
                } else {
                    hideable.OnFOVLeave();
                }
            }
        }

        // Physics.autoSyncTransforms = true;
    }

    /// <summary>
    /// 将角度转换为方向矢量.
    /// </summary>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    public Vector3 DirFromAngle(float angleInDegrees, bool IsAngleGlobal) {
        if (!IsAngleGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}


/// <summary>
/// 用于存储有关视图光线投射的信息的结构体
/// </summary>
public struct ViewCastInfo {
    public bool hit;
    public Vector3 point;
    public float distance;
    public float angle;

    public ViewCastInfo(bool hit, Vector3 point, float distance, float angle) {
        this.hit = hit;
        this.point = point;
        this.distance = distance;
        this.angle = angle;
    }
}

/// <summary>
/// 保存边缘信息的结构体
/// </summary>
public struct EdgeInfo {
    public Vector3 pointA;
    public Vector3 pointB;

    public EdgeInfo(Vector3 pointA, Vector3 pointB) {
        this.pointA = pointA;
        this.pointB = pointB;
    }
}