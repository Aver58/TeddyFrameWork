using Unity.Burst;
using Unity.Entities;
using Framework.GPF;

namespace Simple.TPS {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateAfter(typeof(TPZoomCameraSystem))]
    [UpdateBefore(typeof(CameraSystem))]
    public partial struct SimpleCameraRequestSystem : ISystem {
        private ComponentLookup<CommonCameraInfo> cameraInfoLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            cameraInfoLookup = state.GetComponentLookup<CommonCameraInfo>(true);
            state.RequireForUpdate<TPCameraState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            cameraInfoLookup.Update(ref state);
            Entity tpCameraEnt = SystemAPI.GetSingletonEntity<TPCameraState>();

            if (!SystemAPI.HasSingleton<TPZoomCameraState>()) {
                CameraUtil.RequestCamera(state.EntityManager, tpCameraEnt, cameraInfoLookup[tpCameraEnt].priority, cameraInfoLookup[tpCameraEnt].fadeDuration);
                return;
            }

            int priority;
            float fadeDuration;

            Entity zoomCameraEnt = SystemAPI.GetSingletonEntity<TPZoomCameraState>();
            TPZoomCameraInfo zoomCameraInfo = state.EntityManager.GetComponentData<TPZoomCameraInfo>(zoomCameraEnt);
            TPZoomCameraState zoomCameraState = state.EntityManager.GetComponentData<TPZoomCameraState>(zoomCameraEnt);

            // TODO 根据角色状态判断本帧的相机请求
            Entity cameraEnt = zoomCameraState.aim ? zoomCameraEnt : tpCameraEnt;

            if (zoomCameraState.prevAim && !zoomCameraState.aim) {
                priority = cameraInfoLookup[zoomCameraEnt].priority;
                fadeDuration = zoomCameraInfo.zoomOutDuration;
            } else {
                priority = cameraInfoLookup[cameraEnt].priority;
                fadeDuration = cameraInfoLookup[cameraEnt].fadeDuration;
            }

            CameraUtil.RequestCamera(state.EntityManager, cameraEnt, priority, fadeDuration);
        }

        public void OnDestroy(ref SystemState state) { }
    }
}
