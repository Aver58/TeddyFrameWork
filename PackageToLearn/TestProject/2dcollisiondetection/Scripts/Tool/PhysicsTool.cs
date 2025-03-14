using System.Collections.Generic;
using AOT;
using CustomPhysics.Collision;
using CustomPhysics.Collision.Model;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace CustomPhysics.Tool {
    [BurstCompile(CompileSynchronously = true)]
    public static class PhysicsTool {
        #region Old

        /// <summary>
        /// 检查点是否在一个多边形中
        /// </summary>
        /// <param name="vertices">点集，顺序要求能够从头到尾连接闭环</param>
        /// <param name="point">要检查的点</param>
        /// <returns></returns>
        public static bool IsPointInPolygon(List<Vector3> vertices, Vector3 point) {
            if (vertices.Count <= 2) {
                return false;
            }

            // PNPoly, 射线法
            bool flag = false;
            for (int i = 0, count = vertices.Count, j = count - 1; i < count; j = i++) {
                Vector3 edgeFrom = vertices[i];
                Vector3 edgeTo = vertices[j];
                // 被测点是否在边上
                if (IsPointOnSegment(edgeFrom, edgeTo, point)) {
                    return true;
                }

                // 等价于min(edgeFrom.y, edgeTo.y) < point.y <= max(edgeFrom.y, edgeTo.y)
                // 排除了不会相交的边，同时排除了edgeFrom.y == edgeTo.y的情况
                bool verticalInRange = edgeFrom.y > point.y != edgeTo.y > point.y;
                if (verticalInRange) {
                    float edgeSlope = (edgeTo.x - edgeFrom.x) / (edgeTo.y - edgeFrom.y);
                    float xOfPointOnEdge = edgeFrom.x + edgeSlope * (point.y - edgeFrom.y);
                    bool isPointLeftToEdge = point.x - xOfPointOnEdge < 0;
                    // 被测点是否在测试边的左侧（假想中的射线向右发射）
                    if(isPointLeftToEdge) {
                        flag = !flag;
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// 点是否在线段上
        /// </summary>
        /// <param name="lineStart">线段起点</param>
        /// <param name="lineEnd">线段终点</param>
        /// <param name="point">检查点</param>
        /// <returns></returns>
        public static bool IsPointOnSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point) {
            // collinear && in range
            return Vector3.Cross(lineStart - point, lineEnd - point) == Vector3.zero &&
                   Vector3.Dot(lineStart - point, lineEnd - point) <= 0;
        }

        #endregion

        public delegate void GetClosestPointToOriginDelegate(
            in float3 a, in float3 b, out float3 result);
        public delegate void GetPerpendicularToOriginDelegate(
            in float3 a, in float3 b, out float3 result);
        public delegate void CreateEdgeDelegate(
            in float3 a, in float3 b, out float distance, out float3 normal);
        public delegate void CheckCircleCollidedDelegate(
            in float3 fstPos, in float fstRadius, in float fstScale,
            in float3 sndPos, in float sndRadius, in float sndScale,
            out bool isCollided, out float3 penetrateVec);

        /// <summary>
        /// 检查点是否在三角形内
        /// </summary>
        /// <param name="points">点集</param>
        /// <param name="point">要检查的点</param>
        /// <returns></returns>
        public static bool IsPointInTriangle(List<float3> points, float3 point) {
            float3 v0 = points[2] - points[0];
            float3 v1 = points[1] - points[0];
            float3 v2 = point - points[0];

            float dot00 = math.dot(v0, v0);
            float dot01 = math.dot(v0, v1);
            float dot02 = math.dot(v0, v2);
            float dot11 = math.dot(v1, v1);
            float dot12 = math.dot(v1, v2);
            float denominator = 1 / (dot00 * dot11 - dot01 * dot01);

            // condition: u >= 0 && v >= 0 && u + v <= 1
            float u = (dot11 * dot02 - dot01 * dot12) * denominator;
            if (u < 0 || u > 1) {
                return false;
            }
            float v = (dot00 * dot12 - dot01 * dot02) * denominator;
            if (v < 0 || v > 1) {
                return false;
            }
            return u + v <= 1;
        }

        [BurstCompile(CompileSynchronously = true)]
        [MonoPInvokeCallback(typeof(GetPerpendicularToOriginDelegate))]
        public static void GetPerpendicularToOrigin(in float3 a, in float3 b, out float3 result) {
            float3 ab = b - a;
            float3 ao = -a;

            float sqrLength = math.distancesq(ab, float3.zero);
            if (sqrLength < float.Epsilon) {
                result = float3.zero;
                return;
            }

            float projection = math.dot(ab, ao) / sqrLength;

            // return a + ab * projection;
            result = new float3(a.x + projection * ab.x, a.y + projection * ab.y,
                a.z + projection * ab.z);
        }

        [BurstCompile(CompileSynchronously = true)]
        [MonoPInvokeCallback(typeof(GetClosestPointToOriginDelegate))]
        public static void GetClosestPointToOrigin(in float3 a, in float3 b, out float3 result) {
            float3 ab = b - a;
            float3 ao = -a;

            float sqrLength = math.distancesq(ab, float3.zero);

            // ab点重合了
            if(sqrLength < float.Epsilon) {
                result = a;
                return;
            }

            float projection = math.dot(ab, ao) / sqrLength;
            if (projection < 0) {
                result = a;
            }
            else if (projection > 1.0f) {
                result = b;
            }
            else {
                result = new float3(a.x + projection * ab.x, a.y + projection * ab.y,
                    a.z + projection * ab.z);
            }
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(CreateEdgeDelegate))]
        public static void CreateEdge(in float3 a, in float3 b, out float distance, out float3 normal) {
            GetPerpendicularToOrigin(a, b, out float3 result);
            normal = result;
            float lengthSq = math.distancesq(result, float3.zero);
            // 单位化边
            if (lengthSq > float.Epsilon) {
                distance = math.sqrt(lengthSq);
                normal *= 1.0f / distance;
            }
            else {
                // 向量垂直定则
                float3 v = a - b;
                v = math.normalizesafe(v);
                distance = 0;
                normal = new float3(v.z, 0, -v.x);
            }
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(CheckCircleCollidedDelegate))]
        public static void CheckCircleCollided(
            in float3 fstPos, in float fstRadius, in float fstScale,
            in float3 sndPos, in float sndRadius, in float sndScale,
            out bool isCollided, out float3 penetrateVec) {
            float fst = fstRadius * fstScale;
            float snd = sndRadius * sndScale;
            float radiusDis = fst + snd;
            isCollided = math.distance(fstPos, sndPos) - radiusDis <= 0;
            if (isCollided) {
                float3 oriVec = sndPos - fstPos;
                float oriDis = math.distance(oriVec, float3.zero);
                if (oriDis < 0.00001f) {
                    float separation = math.max(fst, snd);
                    oriVec = separation * math.normalizesafe(new float3(
                        fstRadius * fstRadius % 7, 0, sndRadius * sndScale % 17));
                } else {
                    oriVec = math.normalizesafe(oriVec);
                }
                penetrateVec = (radiusDis - oriDis) * oriVec;
            } else {
                penetrateVec = float3.zero;
            }
        }
    }
}