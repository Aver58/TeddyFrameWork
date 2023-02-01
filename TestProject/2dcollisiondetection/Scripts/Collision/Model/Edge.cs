using Unity.Mathematics;
using UnityEngine;

namespace CustomPhysics.Collision.Model {
    public class Edge {
        public float3 a;
        public float3 b;
        public float3 normal;
        public float distance;
        public int index;
    }
}