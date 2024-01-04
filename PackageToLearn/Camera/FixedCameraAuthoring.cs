using Framework.GPF;

using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Simple.TPS {
    public class FixedCameraAuthoring : MonoBehaviour {
        public float fov;

        public float fadeDuration;
        public int priority;
        public float radius;
        public float3 triggerPosition;
    }

    class FixedCameraBaker : Baker<FixedCameraAuthoring> {
        public override void Bake(FixedCameraAuthoring authoring) {
            AddComponent<FixedCameraState>();
            AddComponent(new DebugCameraInfo {
                name = "Fixed Camera"
            });
            AddComponent(new FixedCameraInfo {
                triggerPosition = authoring.triggerPosition,
                radius = authoring.radius
            });
            AddComponent(new CommonCameraInfo {
                fadeDuration = authoring.fadeDuration,
                priority = authoring.priority
            });

            AddComponent(new Translation { Value = authoring.transform.position });
            AddComponent(new Rotation { Value = authoring.transform.rotation });
            AddComponent(new FovState { value = authoring.fov });
        }
    }
}
