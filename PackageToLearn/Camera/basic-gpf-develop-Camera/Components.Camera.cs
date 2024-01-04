using Unity.Collections;
using Unity.Entities;

namespace Framework.GPF {
    public struct FovState : IComponentData {
        public float value;
    }

    public struct CameraSource : IComponentData {
        public FixedString512Bytes url;
    }

    public struct DebugCameraInfo : IComponentData {
        public FixedString64Bytes name;
    }

    public struct CommonCameraInfo : IComponentData {
        public int priority;
        public float fadeDuration;
    }

    public struct PersistentCamera : IBufferElementData {
        public Entity cameraEnt;
    }

    public struct CameraStack : IBufferElementData {
        public Entity cameraEnt;
        public float fadeRatio;
        public float fadeDuration;
    }

    public struct CameraRequest : IComponentData {
        public Entity cameraEnt;
        public int priority;
        public float fadeDuration;
    }

    public struct PersistentCameraRequest : IComponentData {
        public Entity cameraEnt;
        public bool isActive;
    }
}
