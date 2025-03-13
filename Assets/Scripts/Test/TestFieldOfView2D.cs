using System.Collections.Generic;
using UnityEngine;

namespace MyNamespace {
    /// <summary>
    /// 用于存储有关视图光线投射的信息的结构体
    /// </summary>
    public struct ViewCastInfo {
        public bool hit;
        public Vector2 point;
        public float distance;
        public float angle;

        public ViewCastInfo(bool hit, Vector2 point, float distance, float angle) {
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
        public Vector2 pointA;
        public Vector2 pointB;

        public EdgeInfo(Vector2 pointA, Vector2 pointB) {
            this.pointA = pointA;
            this.pointB = pointB;
        }
    }

    public class RoleFieldOfView : MonoBehaviour {
        [Header("视野设置")]
        [Tooltip("玩家可以看到的半径或最大距离")] public float viewRadius = 50f;

        [Range(0, 360), Tooltip("视野角度")] public float viewAngle = 90f;

        [Header("边缘解析设置")]
        [Tooltip("边缘分解算法的迭代（更高=更精确但也更昂贵）")] public int edgeResolveIterations = 1;

        public float edgeDstThreshold;

        [Header("常规设置")]
        [Range(0, 1), Tooltip("视场更新之间的延迟,隔多少秒设置一次扫描物体的隐藏显示")]
        public float delayBetweenFOVUpdates = 0.2f;

        [Header("层级设置")]
        [Tooltip("阻挡视野的对象")] public LayerMask obstacleMask;

        [Header("可视化设置")]
        [Tooltip("视野可视化吗？")] public bool ShowFieldOfView = true;

        [Tooltip("影响重新计算视场时射出的射线数量。光线投射计数=视角*网格分辨率")]
        public float meshResolution = 1;

        private Mesh viewMesh;
        private MeshFilter viewMeshFilter;
        private List<Vector2> viewPoints = new List<Vector2>();
        private readonly List<int> triangles = new List<int>();
        private readonly List<Vector3> vertices = new List<Vector3>();

        private void Start() {
            viewMeshFilter = GetComponent<MeshFilter>();
            viewMesh = new Mesh { name = "RoleFieldOfView" };
            viewMeshFilter.mesh = viewMesh;
        }

        private void OnDestroy() {
            ShowFieldOfView = false;
        }

        private void LateUpdate() {
            if (ShowFieldOfView) {
                viewMeshFilter.mesh = viewMesh;
                DrawFieldOfView();
            } else {
                viewMeshFilter.mesh = null;
            }
        }

        private void DrawFieldOfView() {
            viewPoints.Clear();
            var oldViewCast = new ViewCastInfo();
            for (int i = 0; i <= Mathf.RoundToInt(viewAngle * meshResolution); i++) {
                var newViewCast = ViewCast(transform.eulerAngles.z - viewAngle / 2 + (viewAngle / Mathf.RoundToInt(viewAngle * meshResolution)) * i, viewRadius);
                if (i > 0) {
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && Mathf.Abs(oldViewCast.distance - newViewCast.distance) > edgeDstThreshold)) {
                        var edge = FindEdge(oldViewCast, newViewCast, viewRadius);
                        if (edge.pointA != Vector2.zero) {
                            viewPoints.Add(edge.pointA);
                        }

                        if (edge.pointB != Vector2.zero) {
                            viewPoints.Add(edge.pointB);
                        }
                    }
                }

                viewPoints.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }

            // 网格绘制
            int vertexCount = viewPoints.Count + 1;
            vertices.Clear();
            triangles.Clear();
            vertices.Add(Vector3.zero);
            for (int i = 0; i < vertexCount - 1; i++) {
                vertices.Add((Vector3)transform.InverseTransformPoint(viewPoints[i]));
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

        // 以给定的角度投射光线，结果返回ViewCastInfo结构。
        private ViewCastInfo ViewCast(float globalAngle, float viewRadius) {
            var dir = DirFromAngle(globalAngle, true);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), dir, viewRadius, obstacleMask);
            if (hit) {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            } else {
                return new ViewCastInfo(false, (Vector2)transform.position + dir * viewRadius, viewRadius, globalAngle);
            }
        }

        // 将角度转换为方向矢量.
        private Vector2 DirFromAngle(float angleInDegrees, bool IsAngleGlobal) {
            if (!IsAngleGlobal) {
                angleInDegrees += transform.eulerAngles.z;
            }

            return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }

        // 找到碰撞体的边缘
        private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, float viewRadius) {
            float minAngle = minViewCast.angle;
            float maxAngle = maxViewCast.angle;
            Vector2 minPoint = Vector2.zero;
            Vector2 maxPoint = Vector2.zero;

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
    }
}