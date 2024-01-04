using Framework.GPF;

using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Simple.TPS {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateAfter(typeof(TPCameraSystem))]
    [UpdateBefore(typeof(CameraSystem))]
    public partial struct TPZoomCameraSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TPCameraState>();
            state.RequireForUpdate<TPZoomCameraState>();
            state.RequireForUpdate<CameraLocalCommand>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float dt = SystemAPI.Time.DeltaTime;

            EntityManager entityManager = state.EntityManager;
            Entity zoomCameraEnt = SystemAPI.GetSingletonEntity<TPZoomCameraState>();
            CameraLocalCommand cameraLocalCommand = SystemAPI.GetSingleton<CameraLocalCommand>();

            TPZoomCameraInfo zoomCameraInfo = entityManager.GetComponentData<TPZoomCameraInfo>(zoomCameraEnt);
            TPZoomCameraState zoomCameraState = entityManager.GetComponentData<TPZoomCameraState>(zoomCameraEnt);

            if (!state.EntityManager.Exists(zoomCameraState.tpCameraEnt)) {
                zoomCameraState.tpCameraEnt = SystemAPI.GetSingletonEntity<TPCameraState>();
            }

            Translation targetTranslation = entityManager.GetComponentData<Translation>(zoomCameraState.tpCameraEnt);
            Rotation targetRotation = entityManager.GetComponentData<Rotation>(zoomCameraState.tpCameraEnt);

            SystemAPI.SetComponent(zoomCameraEnt, targetTranslation);
            SystemAPI.SetComponent(zoomCameraEnt, targetRotation);

            zoomCameraState.prevAim = zoomCameraState.aim;
            zoomCameraState.aim = cameraLocalCommand.aim;

            if (zoomCameraState.aim) {
                zoomCameraState.adsTransitionRatio += dt / zoomCameraInfo.zoomInDuration;
            } else {
                zoomCameraState.adsTransitionRatio -= dt / zoomCameraInfo.zoomOutDuration;
            }

            zoomCameraState.adsTransitionRatio = math.clamp(zoomCameraState.adsTransitionRatio, 0, 1);
            entityManager.SetComponentData(zoomCameraEnt, zoomCameraState);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }
}