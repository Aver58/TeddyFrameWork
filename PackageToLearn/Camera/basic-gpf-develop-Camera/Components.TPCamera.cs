using Framework.Core;

using Unity.Entities;
using Unity.Mathematics;

namespace Framework.GPF {
    public struct TPCameraInfo : IComponentData {
        public float minPitch;
        public float maxPitch;

        public float pitch;
        public float yaw;
        public float roll;

        public float distance;
        public float minDistance;
        public float maxDistance;

        public float pivotHeight;
        public float rotateScale;
        public float zoomScale;
        public float zoomDamping;

        public float sphereRadius;

        public float fov;
        public float2 offset;

        public uint belongTo;
        public uint collidesWith;
    }

    public struct TPCameraState : IComponentData {
        public float pitch;
        public float yaw;
        public float roll;

        public float fov;
        public float distance;
        public float2 offset;

        public bool isCollided;
        public float3 collidePosition;

        public Entity pivotEnt;
    }

    [WriteGroup(typeof(SpawnEntityPrefabRequest))]
    public struct TPCameraSpawnInfo : IComponentData {
        public Entity target; // Actor
    }
}