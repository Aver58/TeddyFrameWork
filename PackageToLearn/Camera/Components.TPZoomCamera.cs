using Unity.Entities;

namespace Simple.TPS {
    public struct TPZoomCameraInfo : IComponentData {
        public float zoomInDuration;
        public float zoomOutDuration;
    }

    public struct TPZoomCameraState : IComponentData {
        public Entity tpCameraEnt;
        public bool prevAim;
        public bool aim;
        public float adsTransitionRatio;
    }
}