using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Simple.TPS {
    public struct FixedCameraInfo : IComponentData {
        public float3 triggerPosition;
        public float radius;
    }

    public struct FixedCameraState : IComponentData {
        public bool active;
    }
}