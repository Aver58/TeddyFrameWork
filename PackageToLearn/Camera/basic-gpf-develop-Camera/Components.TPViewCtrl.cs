using Unity.Entities;
using Unity.Mathematics;

namespace Framework.GPF {
    public struct TPViewCtrlFollowState : IComponentData {
        public Entity target;
        public float expectDistance;
        public float expectFov;
        public float2 expectOffset;
    }

    public struct TPViewCtrlRotateState : IComponentData {
        public float3 baseEuler;
        public float3 additiveEuler;
    }
}
