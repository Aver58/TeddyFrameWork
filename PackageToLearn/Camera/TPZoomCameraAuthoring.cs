using System.Collections;
using System.Collections.Generic;
using Framework.GPF;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Simple.TPS {
    public class TPZoomCameraAuthoring : MonoBehaviour {
        public float fov;
        public float zoomInDuration;
        public float zoomOutDuration;
        public int priority;
    }

    class TPZoomCameraBaker : Baker<TPZoomCameraAuthoring> {
        public override void Bake(TPZoomCameraAuthoring authoring) {
            AddComponent<TPZoomCameraState>();
            AddComponent(new DebugCameraInfo {
                name = "Zoom Camera"
            });
            AddComponent(new TPZoomCameraInfo {
                zoomInDuration = authoring.zoomInDuration,
                zoomOutDuration = authoring.zoomOutDuration,
            });
            AddComponent(new CommonCameraInfo {
                fadeDuration = authoring.zoomInDuration,
                priority = authoring.priority
            });

            AddComponent<Translation>();
            AddComponent<Rotation>();
            AddComponent(new FovState { value = authoring.fov });
        }
    }
}
