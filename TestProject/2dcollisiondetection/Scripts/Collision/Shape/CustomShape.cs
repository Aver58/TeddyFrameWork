using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace CustomPhysics.Collision.Shape {
    public class CustomShape : CollisionShape {
        public CustomShape(float3[] localVertices) : base(ShapeType.Custom, localVertices) {
        }

        protected override void GetBound(out float3 lowerBound, out float3 upperBound) {
            float minX = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxZ = float.MinValue;

            for (int i = 0, count = vertices.Length; i < count; i++) {
                if (vertices[i].x < minX) {
                    minX = vertices[i].x;
                }
                if (vertices[i].z < minZ) {
                    minZ = vertices[i].z;
                }
                if (vertices[i].x > maxX) {
                    maxX = vertices[i].x;
                }
                if (vertices[i].z > maxZ) {
                    maxZ = vertices[i].z;
                }
            }

            lowerBound = new float3(minX, 0, minZ);
            upperBound = new float3(maxX, 0, maxZ);
        }
    }
}