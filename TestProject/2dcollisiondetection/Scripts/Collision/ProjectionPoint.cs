using System;
using CustomPhysics.Collision.Model;
using CustomPhysics.Collision.Shape;

namespace CustomPhysics.Collision {
    public class ProjectionPoint {
        public CollisionObject collisionObject;
        public AABBProjectionType projectionType;
        public float value {
            get {
                // TODO: 添加一个脏标记
                UpdateProjectionPoint();
                return _value;
            }
        }
        private float _value;

        public ProjectionPoint(CollisionObject collisionObject, AABBProjectionType projectionType) {
            this.collisionObject = collisionObject;
            this.projectionType = projectionType;
        }

        private void UpdateProjectionPoint() {
            AABB aabb = collisionObject.shape.aabb;

            switch (projectionType) {
                case AABBProjectionType.HorizontalStart:
                    _value = aabb.lowerBound.x;
                    break;
                case AABBProjectionType.HorizontalEnd:
                    _value = aabb.upperBound.x;
                    break;
                case AABBProjectionType.VerticalStart:
                    _value = aabb.lowerBound.z;
                    break;
                case AABBProjectionType.VerticalEnd:
                    _value = aabb.upperBound.z;
                    break;
                default:
                    throw new Exception("获得AABB投影点时传入了不存在的投影类型");
            }
        }
    }
}