using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace CustomPhysics.Collision.Model {
    [BurstCompile(CompileSynchronously = true)]
    public struct CollisionPair {
        public CollisionObject first;
        public CollisionObject second;
        public float3 penetrateVec;
    }
}