using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace CustomPhysics.Collision.Model {
    public enum AABBProjectionType {
        HorizontalStart,
        HorizontalEnd,
        VerticalStart,
        VerticalEnd,
    }

    [BurstCompile(CompileSynchronously = true)]
    public struct AABB {
        public float3 lowerBound;
        public float3 upperBound;

        public AABB(float3 lowerBound, float3 upperBound) {
            this.lowerBound = lowerBound;
            this.upperBound = upperBound;
        }
    }
}