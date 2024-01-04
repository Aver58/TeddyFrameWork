using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyBuildSystem.Features.Runtime.Bases.Buildings {
    public class BuildingPlacer : MonoBehaviour {
        public BuildingPart preview;

        private Vector3 sourcePosition;
        private Vector3 targetPosition;
        private Vector3 hitPosition;

        public enum BuildMode { NONE, PLACE, DESTROY }
        private BuildMode buildMode;
        public BuildMode GetBuildMode => buildMode;

        private int selectId { get {
            return 0;
        }}

        private RaycastHit[] hitResult = new RaycastHit[20];
        private RaycastHit[] hitResultPlane = new RaycastHit[1];

        private void Start() {
        }

        private Vector3[] GetVertex(Vector3[] vertices) {
            var uniqueVertices = new HashSet<Vector3>();
            for (int i = 0; i < vertices.Length; i++) {
                uniqueVertices.Add(vertices[i]);
            }

            return uniqueVertices.ToArray();
        }

        private void Update() {
            HandleBuildModes();
        }

        private void HandleBuildModes() {
            if (Input.GetKeyDown(KeyCode.E)) {
                ChangeBuildMode(BuildMode.PLACE);
            }

            if (Input.GetKeyDown(KeyCode.R)) {
                ChangeBuildMode(BuildMode.DESTROY);
            }

            
            if (GetBuildMode == BuildMode.PLACE) {
                HandlePlacingMode();
            }
        }

        private void HandlePlacingMode() {
            if (!HasPreview()) {
                CreatePreview(selectId);
            } else {
            //     if (FindClosestSocket()) {
            //         HandleSnapping();
            //     } else {
            HandleFree();
            //     }
            }
        }

        private float tolerance = 0.5f;
        private float rayDistance = 10;
        private float TOLERANCE = 0.01f;
        private void HandleFree() {
            // HandleUtility.FindNearestVertex(targetPosition);
            // 射线检测
            // 碰撞点
            // 遍历所有顶点(这个可以后续用配置，先获取mesh的顶点)
            //      遍历当前对象所有顶点，拿到最近顶点
            // 小于指定阈值，就进行吸附，强行位移偏移
            var ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastManager.RaycastNonAlloc(ray, hitResult, rayDistance, LayerMask.GetMask("Raycast")|
                                                                        LayerMask.GetMask("Default"));
            if (hitResult.Length > 0) {
                for (int i = 0; i < hitResult.Length; i++) {
                    var hit = hitResult[i];
                    if (hit.collider != null) {
                        // Debug.Log("Vertex Position: " + vertex);
                        hitPosition = hit.point;
                        var buildingPart = hit.transform.GetComponent<BuildingPart>();
                        if (buildingPart) {
                            var distanceSource = Mathf.Infinity;
                            // 找到预览对象，离碰撞点最近的顶点
                            foreach (var previewBuildSocket in preview.GetBuildingSockets()) {
                                var dis = Vector3.Distance(hit.point, previewBuildSocket.transform.position);
                                if (dis < distanceSource) {
                                    distanceSource = dis;
                                    sourcePosition = previewBuildSocket.transform.position;
                                }
                            }

                            // 变成 Unity 的顶点吸附功能实现
                            var distanceTarget = Mathf.Infinity;
                            var targetBuildingSockets = buildingPart.GetBuildingSockets();
                            foreach (var buildingSocket  in targetBuildingSockets) {
                                var dis = Vector3.Distance(hit.point, buildingSocket.transform.position);
                                if (dis < distanceTarget) {
                                    distanceTarget = dis;
                                    targetPosition = buildingSocket.transform.position;
                                }
                            }

                            var myCollider = preview.GetComponent<Collider>();
                            var targetCollider = hit.collider;
                            var myClosestPoint = myCollider.ClosestPoint(targetCollider.transform.position);
                            var targetClosestPoint = targetCollider.ClosestPoint(myClosestPoint);

                            
                            var offset = targetClosestPoint - myClosestPoint;
                            if (offset.magnitude < 0.5f) {
                                preview.transform.position += offset;
                            }

                            var newVector = new Vector3();
                            // hit 的2个轴移动
                            if (Math.Abs(myClosestPoint.x - preview.transform.position.x - myCollider.bounds.extents.x) < TOLERANCE) {
                                newVector.x = preview.transform.position.x;
                                newVector.y = hit.point.y;
                                newVector.z = hit.point.z;
                            } else if (Math.Abs(myClosestPoint.y - preview.transform.position.y - myCollider.bounds.extents.y) < TOLERANCE) {
                                newVector.x = hit.point.x;
                                newVector.y = preview.transform.position.y;
                                newVector.z = hit.point.z;
                            }else if (Math.Abs(myClosestPoint.z - preview.transform.position.z - myCollider.bounds.extents.z) < TOLERANCE) {
                                newVector.x = hit.point.x;
                                newVector.y = hit.point.y;
                                newVector.z = preview.transform.position.z;
                            }
                            preview.transform.position = newVector;

                            // var distanceOffset = Vector3.Distance(targetPosition, sourcePosition);
                            // if (distanceOffset <= 0.2f) {
                            //     var offset = targetPosition - sourcePosition;
                            //     preview.transform.position += offset;
                            // } else {
                            //     // 面吸附
                            //
                            //     // 先吸附上，然后开始坐标偏移，应该还要有一个父节点偏移
                            //     preview.transform.position = hit.point;
                            // }


                            // 改成点到面的距离，用投影去实现
                            // if (distance <= tolerance) {
                            //
                            // } else {
                            //     preview.transform.position = hit.point;
                            // }
                        } else {
                            preview.transform.position = hit.point;
                        }
                        break;
                    } else {
                        // 地板
                        // 没有击中才是这样处理
                        // 由射线方向算出射线最远距离
                        var maxDistancePosition = ray.origin + ray.direction * rayDistance;
                        // 然后向下打射线，着地处理
                        Physics.RaycastNonAlloc(maxDistancePosition, -Vector3.up, hitResult, Mathf.Infinity, LayerMask.GetMask("Default"));
                        for (int j = 0; j < hitResult.Length; j++) {
                            var groundHit = hitResult[j];
                            if (groundHit.collider != null) {
                                preview.transform.position = groundHit.point;
                            }
                        }
                    }
                }
            }
        }

        private void OnGUI() {
            GUILayout.Label("sourcePosition : " + sourcePosition);
            GUILayout.Label("targetPosition : " + targetPosition);
            GUILayout.Label("hitPosition : " + hitPosition);
        }

        private void OnDrawGizmos() {
            Gizmos.DrawSphere(targetPosition, 0.2f);
            Gizmos.DrawWireSphere(sourcePosition, 0.1f);
            Gizmos.DrawWireSphere(hitPosition, 0.2f);
        }

        #region Preview

        private bool HasPreview() {
            return preview != null;
        }

        private void CreatePreview(int id) {

        }

        private void CancelPreview() {

        }

        #endregion

        private void ChangeBuildMode(BuildMode mode, bool clearPreview = true) {
            if (clearPreview) {
                CancelPreview();
            }

            buildMode = mode;
            // OnChangedBuildModeEvent?.Invoke(mode);
        }
    }
}