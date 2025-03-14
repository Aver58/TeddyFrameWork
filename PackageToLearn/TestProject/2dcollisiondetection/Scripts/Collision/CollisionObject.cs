using System;
using System.Collections.Generic;
using CustomPhysics.Collision.Model;
using CustomPhysics.Collision.Shape;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Object = System.Object;

namespace CustomPhysics.Collision {
    [Flags]
    public enum CollisionFlags {
        Default = 1 << 0,
        StaticObject = 1 << 1,
        NoContactResponse = 1 << 2,
    }

    public struct CollisionShot {
        public CollisionObject target;
        public int count;
    }

    public interface ICollisionObject {
        public void InitCollisionObject();
        public ProjectionPoint GetProjectionPoint(AABBProjectionType projectionType);
        public void SetActive(bool value);
        public bool HasFlag(CollisionFlags flag);
        public void AddFlag(CollisionFlags flag);
        public void RemoveFlag(CollisionFlags flag);
        public float3 GetCurPosition();
        public float3 GetNextPosition();
        public float GetCurRotation();
        public void SetCurPos(float3 value);
        public void Translate(float3 diff);
        public void TranslateTo(float3 value);
        public void Rotate(float3 diff);
        public void RotateTo(float3 value);
        public void Scale(float diff);
        public void ScaleTo(float value);
        public float3 GetActiveVelocity();
        public float3 GetInputMoveVelocity();
        public void SetInputMoveVelocity(float3 value);
        public void AddExternalVelocity(float3 diff);
        public void SetExternalVelocity(float3 value);
        public void AddAcceleration(Acceleration accelerationInfo);
        public void RemoveAcceleration(Acceleration accelerationInfo);
    }

    public class CollisionObject : ICollisionObject{
        private static int publicId = 1;
        public int id;
        public CollisionShape shape;
        public bool isActive = true;
        public CollisionFlags flags = CollisionFlags.Default;
        public Object contextObject;
        public float3 position;
        public float3 nextPosition;
        public float3 rotation;
        public float scale = 1;
        public int level = 0;

        public List<Acceleration> accelerations;
        public float3 velocity;
        public float3 inputMoveVelocity;
        public float3 resolveVelocity;

        public Dictionary<int, CollisionShot> collisionShotsDic;
        public List<int> collisionShotList;
        public Action<CollisionObject> enterAction;
        public Action<CollisionObject> stayAction;
        public Action<CollisionObject> exitAction;

        // TODO: 添加一个脏标记

        public CollisionObject(CollisionShape shape, Object contextObject,
            float3 startPos, float startRotation = 0, int level = 0) {
            this.id = publicId++;
            this.shape = shape;
            this.position = startPos;
            this.nextPosition = startPos;
            this.rotation = new float3(0, startRotation, 0);
            this.contextObject = contextObject;
            this.level = level;
            accelerations = new List<Acceleration>();
            collisionShotsDic = new Dictionary<int, CollisionShot>();
            collisionShotList = new List<int>();
        }

        public static bool IsSameCollisionObject(CollisionObject obj1, CollisionObject obj2) {
            return obj1.id == obj2.id;
        }

        public void TryToCreateCollisionShot(CollisionObject target) {
            int targetId = target?.id ?? -1;
            int oriCount = -1;
            if (collisionShotsDic.TryGetValue(targetId, out CollisionShot shotInDic)) {
                oriCount = shotInDic.count;
            }

            collisionShotsDic[targetId] = new CollisionShot() {
                target = target,
                count = oriCount == -1 ? 2 : 1,
            };

            if (oriCount == -1) {
                collisionShotList.Add(targetId);
                enterAction?.Invoke(target);
            }
        }

        #region Interface

        public void InitCollisionObject() {
            shape.UpdateShape();

            int count = shape.localVertices.Length;
            float3 origin = (shape.aabb.upperBound + shape.aabb.lowerBound) / 2;
            for (int i = 0; i < count; i++) {
                shape.localVertices[i] -= origin;
            }

            shape.UpdateShape();
            shape.ApplyWorldVertices(position, rotation, scale);
        }

        public ProjectionPoint GetProjectionPoint(AABBProjectionType projectionType) {
            return new ProjectionPoint(this, projectionType);
        }

        public void SetActive(bool value) {
            this.isActive = value;
        }

        public bool HasFlag(CollisionFlags flag) {
            return flags.HasFlag(flag);
        }

        public void AddFlag(CollisionFlags flag) {
            this.flags |= flag;
        }

        public void RemoveFlag(CollisionFlags flag) {
            this.flags &= ~flag;
        }

        public float3 GetCurPosition() {
            return position;
        }

        public float3 GetNextPosition() {
            return nextPosition;
        }

        public float GetCurRotation() {
            return rotation.y;
        }

        public void SetCurPos(float3 value) {
            position = nextPosition = value;
        }

        public void Translate(float3 diff) {
            nextPosition += diff;
        }

        public void TranslateTo(float3 value) {
            nextPosition = value;
        }

        public void Rotate(float3 diff) {
            ApplyRotation(rotation + diff);
        }

        public void RotateTo(float3 value) {
            ApplyRotation(value);
        }

        public void Scale(float diff) {
            ApplyScale(scale + diff);
        }

        public void ScaleTo(float value) {
            ApplyScale(value);
        }

        public float3 GetActiveVelocity() {
            float3 result = velocity + inputMoveVelocity;
            for (int i = 0, count = accelerations.Count; i < count; i++) {
                result += accelerations[i].curVelocity;
            }

            return result;
        }

        public float3 GetInputMoveVelocity() {
            return inputMoveVelocity;
        }

        public void AddExternalVelocity(float3 diff) {
            this.velocity += diff;
        }

        public void AddAcceleration(Acceleration accelerationInfo) {
            accelerations.Add(accelerationInfo);
        }

        public void RemoveAcceleration(Acceleration accelerationInfo) {
            accelerations.Remove(accelerationInfo);
        }

        public void SetInputMoveVelocity(float3 value) {
            this.inputMoveVelocity = value;
        }

        public void SetExternalVelocity(float3 finalVelocity) {
            this.velocity = finalVelocity;
        }

        #endregion

        #region CollisionHandle

        public void ApplyPosition() {
            position = nextPosition;
            shape.ApplyWorldVertices(position, rotation, scale);
        }

        public void ApplyRotation(float3 newRotation) {
            this.rotation = newRotation;
        }

        public void ApplyScale(float newScale) {
            this.scale = newScale;
        }
        public void AddResolveVelocity(float3 diff) {
            if (math.distancesq(diff, float3.zero) > 0.00001f) {
                Debug.Log("?");
            }
            this.resolveVelocity += diff;
        }

        public void CleanVelocity() {
            this.resolveVelocity = this.inputMoveVelocity = float3.zero;
        }

        public float3 GetFarthestPointInDir(float3 dir) {
            float maxDis = float.MinValue;
            float3 farthestPoint = float3.zero;
            for (int i = 0, count = shape.vertices.Length; i < count; ++i) {
                float3 curPoint = shape.vertices[i];
                float dis = math.dot(curPoint, dir);
                if (dis > maxDis) {
                    maxDis = dis;
                    farthestPoint = curPoint;
                }
            }
            return farthestPoint;
        }

        #endregion
    }
}