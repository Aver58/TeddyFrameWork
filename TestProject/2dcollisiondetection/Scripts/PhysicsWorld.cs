using System;
using System.Collections.Generic;
using System.Linq;
using CustomPhysics.Collision;
using CustomPhysics.Collision.Model;
using CustomPhysics.Collision.Shape;
using CustomPhysics.Test;
using CustomPhysics.Tool;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using CollisionFlags = CustomPhysics.Collision.CollisionFlags;
using Random = UnityEngine.Random;

namespace CustomPhysics {
    public interface IPhysicsWorld {
        public void Init();
        public void Tick(float timeTickSpan, CollisionObject independentTarget = null);
        public bool AddCollisionObject(CollisionObject collisionObject);
        public bool RemoveCollisionObject(CollisionObject collisionObject);
    }

    public class PhysicsWorld : IPhysicsWorld {
        public static float epsilon = 0.00001f;
        public int tickFrame = 0;
        public int borderRadius = 15;
        public int maxIterCount = 10;
        public List<CollisionObject> collisionList;
        public List<CollisionPair> collisionPairs;

        private List<ProjectionPoint> broadScanList;
        private Dictionary<int, ProjectionPoint> broadStartPoints;
        private List<ProjectionPoint> verAABBProjList;
        private List<float3> simplexList = new List<float3>();

        public static PhysicsTool.GetClosestPointToOriginDelegate closestCalc;
        public static PhysicsTool.GetPerpendicularToOriginDelegate perpCalc;
        public static PhysicsTool.CreateEdgeDelegate createEdgeCalc;
        public static PhysicsTool.CheckCircleCollidedDelegate checkCircleCalc;

        #region Collision

        private void CollisionDetection(float timeSpan, CollisionObject independentTarget) {
            Profiler.BeginSample("[ViE] BroadPhase");
            BroadPhase();
            Profiler.EndSample();
            Profiler.BeginSample("[ViE] NarrowPhase");
            NarrowPhase(independentTarget);
            Profiler.EndSample();
        }

        private void BroadPhase() {
            collisionPairs.Clear();

            SweepAndPrune();
            // DynamicBVH();
        }

        private void SweepAndPrune() {
            int projCount = verAABBProjList.Count;
            if (projCount <= 0) {
                return;
            }

            // 升序排序
            if (projCount >= 2) {
                for (int i = 1; i < projCount; i++) {
                    var proj = verAABBProjList[i];
                    for (int j = i - 1; j >= 0; j--) {
                        if (verAABBProjList[j].value > proj.value) {
                            verAABBProjList[j + 1] = verAABBProjList[j];
                            verAABBProjList[j] = proj;
                        } else {
                            break;
                        }
                    }
                }
            }

            broadScanList.Clear();
            // (collisionId, ProjectionPoint)
            broadStartPoints.Clear();

            // 在竖直方向上检查
            broadScanList.Add(verAABBProjList[0]);
            broadStartPoints.Add(verAABBProjList[0].collisionObject.id, verAABBProjList[0]);
            for (int i = 1, count = verAABBProjList.Count; i < count; i++) {
                CollisionObject horProjObj = verAABBProjList[i].collisionObject;

                if (verAABBProjList[i].projectionType == AABBProjectionType.VerticalEnd) {
                    ProjectionPoint startPoint = broadStartPoints[horProjObj.id];
                    broadStartPoints.Remove(horProjObj.id);
                    broadScanList.Remove(startPoint);
                } else if (verAABBProjList[i].projectionType == AABBProjectionType.VerticalStart) {
                    for (int j = 0, scanCount = broadScanList.Count; j < scanCount; j++) {
                        CollisionObject scanObj = broadScanList[j].collisionObject;

                        CollisionPair pair;
                        int jLevel = scanObj.level;
                        int iLevel = horProjObj.level;

                        // 让碰撞对内碰撞体的顺序规律一致
                        if (jLevel == iLevel) {
                            int iIndex = collisionList.IndexOf(horProjObj);
                            int jIndex = collisionList.IndexOf(scanObj);
                            pair = new CollisionPair() {
                                first = iIndex < jIndex ? horProjObj : scanObj,
                                second = iIndex > jIndex ? horProjObj : scanObj,
                            };
                        } else {
                            pair = new CollisionPair() {
                                first = jLevel > iLevel ? horProjObj : scanObj,
                                second = jLevel < iLevel ? horProjObj : scanObj,
                            };
                        }

                        collisionPairs.Add(pair);
                    }

                    broadScanList.Add(verAABBProjList[i]);
                    broadStartPoints.Add(horProjObj.id, verAABBProjList[i]);
                }
            }

            // for (int i = 0, count = collisionPairs.Count; i < count; i++) {
                // CollisionPair pair = collisionPairs[i];
                // Debug.Log($"{tickFrame} {pair.first.id}与{pair.second.id}粗检测碰撞");
            // }
        }

        private void DynamicBVH() {
        }

        private void NarrowPhase(CollisionObject independentTarget) {
            for (int i = collisionPairs.Count - 1; i >= 0; i--) {
                CollisionPair pair = collisionPairs[i];
                CollisionObject fst = pair.first;
                CollisionObject snd = pair.second;

                if (independentTarget != null &&
                    !(CollisionObject.IsSameCollisionObject(pair.first, independentTarget) ||
                      CollisionObject.IsSameCollisionObject(pair.second, independentTarget))) {
                    continue;
                }

                bool isCollided = false;
                float3 penetrateVec = float3.zero;
                if (!fst.isActive || !snd.isActive) {
                    isCollided = false;
                } else {
                    simplexList.Clear();

                    bool isAllCircle = fst.shape.shapeType == ShapeType.Circle &&
                            snd.shape.shapeType == ShapeType.Circle;

                    if (isAllCircle) {
                        // 圆碰撞对的计算简单且常用，单独处理以加速
                        checkCircleCalc(fst.position, ((Circle) fst.shape).radius, fst.scale,
                            snd.position, ((Circle) snd.shape).radius, snd.scale,
                            out isCollided, out penetrateVec);
                    } else {
                        isCollided = GJK(fst, snd, simplexList);
                        if (isCollided && !fst.flags.HasFlag(CollisionFlags.NoContactResponse) &&
                            !snd.flags.HasFlag(CollisionFlags.NoContactResponse)) {
                            penetrateVec = EPA(fst, snd, simplexList);
                        }
                    }
                }

                if(!isCollided){
                    collisionPairs.RemoveAt(i);
                } else {
                    collisionPairs[i] = new CollisionPair() {
                        first = pair.first,
                        second = pair.second,
                        penetrateVec = penetrateVec,
                    };
                    // Debug.Log($"{tickFrame} {fst.id}与{snd.id}窄检测碰撞，穿透向量为{pair.penetrateVec}，长度为{pair.penetrateVec.magnitude}");
                }
            }
        }

        private bool GJK(CollisionObject fst, CollisionObject snd, List<float3> simplex) {
            bool isCollision = false;
            float3 supDir = float3.zero;

            supDir = FindFirstDirection(fst, snd);
            simplex.Add(Support(supDir, fst, snd));
            simplex.Add(Support(-supDir, fst, snd));

            float3 fstVertex = simplex[0];
            float3 sndVertex = simplex[1];

            closestCalc(fstVertex, sndVertex, out float3 result);
            supDir = -result;
            for (int i = 0; i < maxIterCount; ++i) {
                if (math.distancesq(supDir, float3.zero) < epsilon) {
                    isCollision = true;
                    break;
                }

                float3 p = Support(supDir, fst, snd);
                float3 supSubFst = p - fstVertex;
                float3 supSubSnd = p - sndVertex;
                if (math.distancesq(supSubFst, float3.zero) < epsilon ||
                    math.distancesq(supSubSnd, float3.zero) < epsilon) {
                    isCollision = false;
                    break;
                }

                simplex.Add(p);

                if (PhysicsTool.IsPointInTriangle(simplex, float3.zero)) {
                    isCollision = true;
                    break;
                }

                supDir = FindNextDirection(simplex);
            }

            return isCollision;
        }

        private float3 EPA(CollisionObject fst, CollisionObject snd, List<float3> simplex) {
            if (simplex.Count > 2) {
                FindNextDirection(simplex);
            }

            SimplexEdge simplexEdge = PhysicsCachePool.GetSimplexEdgeFromPool();
            simplexEdge.InitEdges(simplex);
            Edge curEpaEdge = null;
            float dis = 0;

            for (int i = 0; i < maxIterCount; ++i) {
                Edge e = simplexEdge.FindClosestEdge();
                curEpaEdge = e;

                float3 point = Support(e.normal, fst, snd);
                float distance = math.dot(point, e.normal);
                if (distance - e.distance < epsilon) {
                    dis = distance;
                    break;
                }

                simplexEdge.InsertEdgePoint(e, point);
            }

            PhysicsCachePool.RecycleSimplexEdge(simplexEdge);

            return dis * curEpaEdge.normal;
        }

        public float3 FindFirstDirection(CollisionObject a, CollisionObject b) {
            float3 dir = a.shape.vertices[0] - b.shape.vertices[0];
            if (math.distancesq(dir, float3.zero) < epsilon) {
                dir = a.shape.vertices[1] - b.shape.vertices[0];
            }
            return dir;
        }

        private float3 FindNextDirection(List<float3> simplex) {
            int pointCount = simplex.Count;

            if (pointCount == 2) {
                closestCalc(simplex[0], simplex[1], out float3 result);
                return -result;
            } else if (pointCount == 3) {
                closestCalc(simplex[2], simplex[0], out float3 resultCA);
                float3 crossOnCA = resultCA;
                closestCalc(simplex[2], simplex[1], out float3 resultCB);
                float3 crossOnCB = resultCB;

                if (math.distancesq(crossOnCA, float3.zero) <
                    math.distancesq(crossOnCB, float3.zero)) {
                    simplex.RemoveAt(1);
                    return -crossOnCA;
                } else {
                    simplex.RemoveAt(0);
                    return -crossOnCB;
                }
            } else {
                Debug.Log("[ViE] 单纯形有错误的边数");
                return float3.zero;
            }
        }

        private float3 Support(float3 dir, CollisionObject fst, CollisionObject snd) {
            float3 a = fst.GetFarthestPointInDir(dir);
            float3 b = snd.GetFarthestPointInDir(-dir);

            float3 supPoint = a - b;

            return supPoint;
        }

        #endregion

        private void ApplyAcceleration(float timeSpan, CollisionObject independentTarget) {
            if (independentTarget != null) {
                return;
            }

            for (int i = 0, count = collisionList.Count; i < count; i++) {
                CollisionObject curCo = collisionList[i];

                // 应用加速度
                for (int j = curCo.accelerations.Count - 1; j >= 0; j--) {
                    Acceleration curA = curCo.accelerations[j];
                    if (curA.isEnded) {
                        curCo.accelerations.RemoveAt(j);
                        continue;
                    }
                    curA.Tick(timeSpan);
                }
            }
        }

        private void Resolve(float timeSpan, CollisionObject independentTarget) {
            for (int i = 0, count = collisionPairs.Count; i < count; i++) {
                CollisionPair pair = collisionPairs[i];

                if (independentTarget != null &&
                    !(CollisionObject.IsSameCollisionObject(pair.first, independentTarget) ||
                      CollisionObject.IsSameCollisionObject(pair.second, independentTarget))) {
                    continue;
                }

                if (pair.first.HasFlag(CollisionFlags.NoContactResponse) ||
                    pair.second.HasFlag(CollisionFlags.NoContactResponse)) {
                    continue;
                }

                float3 fstActiveVelocity = pair.first.GetActiveVelocity();
                float3 sndActiveVelocity = pair.second.GetActiveVelocity();
                float depth = math.distance(pair.penetrateVec, float3.zero);
                float coefficient = 0.7f;
                float tolerance = 0.01f;
                float rate = coefficient * Mathf.Max(0, depth - tolerance);
                float3 penetrateDir = math.normalizesafe(pair.penetrateVec);
                float3 resolveVec = rate * penetrateDir;

                if (pair.first.level > pair.second.level) {
                    pair.second.AddResolveVelocity(resolveVec);
                    float externalRate = math.dot(sndActiveVelocity, penetrateDir) * timeSpan;
                    if (externalRate > epsilon) {
                        pair.second.AddResolveVelocity(externalRate * penetrateDir);
                    }
                } else if(pair.first.level < pair.second.level) {
                    pair.first.AddResolveVelocity(-resolveVec);
                    float externalRate = math.dot(fstActiveVelocity, -penetrateDir) * timeSpan;
                    if (externalRate > epsilon) {
                        pair.first.AddResolveVelocity(-externalRate * penetrateDir);
                    }
                } else {
                    float fstExternalRate = math.dot(fstActiveVelocity, penetrateDir) * timeSpan;
                    float sndExternalRate = math.dot(sndActiveVelocity, -penetrateDir) * timeSpan;

                    bool fstMoving = math.abs(fstExternalRate) > epsilon;
                    bool sndMoving = math.abs(sndExternalRate) > epsilon;
                    if (fstMoving != sndMoving) {
                        if (fstMoving) {
                            pair.first.AddResolveVelocity(-resolveVec);
                            if (fstExternalRate > epsilon) {
                                pair.first.AddResolveVelocity(-fstExternalRate * penetrateDir);
                            }
                        }
                        if (sndMoving) {
                            pair.second.AddResolveVelocity(resolveVec);
                            if (sndExternalRate > epsilon) {
                                pair.second.AddResolveVelocity(sndExternalRate * penetrateDir);
                            }
                        }
                    } else {
                        pair.first.AddResolveVelocity(-resolveVec / 2f);
                        pair.second.AddResolveVelocity(resolveVec / 2f);

                        pair.first.AddResolveVelocity(-fstExternalRate * penetrateDir);
                        pair.second.AddResolveVelocity(sndExternalRate * penetrateDir);
                    }
                }
            }
        }

        private void ApplyVelocity(float timeSpan, CollisionObject independentTarget) {
            // 最后一步
            if (independentTarget == null) {
                for (int i = 0, count = collisionList.Count; i < count; i++) {
                    CollisionObject co = collisionList[i];
                    ApplyVelocityOnAObject(co, timeSpan);
                }
            } else {
                ApplyVelocityOnAObject(independentTarget, timeSpan);
            }
        }

        private void ApplyVelocityOnAObject(CollisionObject co, float timeSpan) {
            if (math.distance(co.GetActiveVelocity(), float3.zero) > 0) {
                Debug.Log("?");
            }

            float3 resultantVelocity = co.GetActiveVelocity() * timeSpan + co.resolveVelocity;
            co.Translate(resultantVelocity);

            // 处理空气墙
            if (!co.flags.HasFlag(CollisionFlags.StaticObject)) {
                float curDis = math.distance(co.position, float3.zero);
                float nextDis = math.distance(co.nextPosition, float3.zero);
                bool curCoInRange = curDis <= borderRadius;
                bool nextCoInRange = nextDis < borderRadius;
                if (curCoInRange && !nextCoInRange) {
                    co.nextPosition = (borderRadius - 0.01f) * math.normalizesafe(co.nextPosition);
                    co.TryToCreateCollisionShot(null);
                }
            }

            co.ApplyPosition();

            co.CleanVelocity();
        }

        private void ExternalPairHandle() {
            for (int i = 0, count = collisionPairs.Count; i < count; i++) {
                CollisionPair pair = collisionPairs[i];
                CollisionObject fst = pair.first;
                CollisionObject snd = pair.second;
                fst.TryToCreateCollisionShot(snd);
                snd.TryToCreateCollisionShot(fst);
            }

            for (int i = collisionList.Count - 1; i >= 0; i--) {
                CollisionObject co = collisionList[i];
                for (int j = co.collisionShotList.Count - 1; j >= 0; j--) {
                    int id = co.collisionShotList[j];
                    CollisionShot curShot = co.collisionShotsDic[id];
                    int curCount = curShot.count;
                    if (curCount < 0) {
                        co.collisionShotList.Remove(id);
                        co.collisionShotsDic.Remove(id);
                        co.exitAction?.Invoke(curShot.target);
                    } else {
                        co.collisionShotsDic[id] = new CollisionShot() {
                            target = curShot.target,
                            count = curCount - 1,
                        };

                        if (curCount == 1) {
                            co.stayAction?.Invoke(curShot.target);
                        }
                    }
                }
            }
        }

        #region Interface

        public void Init() {
            collisionList = new List<CollisionObject>();
            collisionPairs = new List<CollisionPair>();

            broadScanList = new List<ProjectionPoint>();
            broadStartPoints = new Dictionary<int, ProjectionPoint>();
            verAABBProjList = new List<ProjectionPoint>();

            closestCalc = BurstCompiler
                .CompileFunctionPointer<PhysicsTool.GetClosestPointToOriginDelegate>(
                    PhysicsTool.GetClosestPointToOrigin).Invoke;
            perpCalc = BurstCompiler
                .CompileFunctionPointer<PhysicsTool.GetPerpendicularToOriginDelegate>(
                    PhysicsTool.GetPerpendicularToOrigin).Invoke;
            createEdgeCalc = BurstCompiler
                .CompileFunctionPointer<PhysicsTool.CreateEdgeDelegate>(
                    PhysicsTool.CreateEdge).Invoke;
            checkCircleCalc = BurstCompiler
                .CompileFunctionPointer<PhysicsTool.CheckCircleCollidedDelegate>(
                    PhysicsTool.CheckCircleCollided).Invoke;
        }

        public void Tick(float timeSpan, CollisionObject independentTarget = null) {
            tickFrame++;

            CollisionDetection(timeSpan, independentTarget);
            ApplyAcceleration(timeSpan, independentTarget);
            Resolve(timeSpan, independentTarget);
            ApplyVelocity(timeSpan, independentTarget);
            ExternalPairHandle();
        }

        public void Destroy() {
            collisionList.Clear();
            collisionList = null;
            collisionPairs.Clear();
            collisionPairs = null;
            broadScanList.Clear();
            broadScanList = null;
            verAABBProjList.Clear();
            verAABBProjList = null;
        }

        public bool AddCollisionObject(CollisionObject collisionObject) {
            collisionObject.InitCollisionObject();
            collisionList.Add(collisionObject);

            verAABBProjList.Add(
                collisionObject.GetProjectionPoint(AABBProjectionType.VerticalStart));
            verAABBProjList.Add(
                collisionObject.GetProjectionPoint(AABBProjectionType.VerticalEnd));
            return true;
        }

        public bool RemoveCollisionObject(CollisionObject collisionObject) {
            verAABBProjList.Remove(
                collisionObject.GetProjectionPoint(AABBProjectionType.VerticalStart));
            verAABBProjList.Remove(
                collisionObject.GetProjectionPoint(AABBProjectionType.VerticalEnd));
            collisionList.Remove(collisionObject);
            return true;
        }

        #endregion
    }
}