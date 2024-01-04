using Framework.Core;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

using UnityEngine;

namespace Framework.GPF {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateBefore(typeof(ClientPostAnimationSystemGroup))]
    public partial struct CameraSystem : ISystem {
        private Entity cameraStackEnt;
        private ComponentLookup<Translation> translationLookup;
        private ComponentLookup<Rotation> rotationLookup;
        private ComponentLookup<FovState> fovStateLookup;
        private ComponentLookup<CommonCameraInfo> cameraInfoLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            translationLookup = state.GetComponentLookup<Translation>();
            rotationLookup = state.GetComponentLookup<Rotation>();
            fovStateLookup = state.GetComponentLookup<FovState>();
            cameraInfoLookup = state.GetComponentLookup<CommonCameraInfo>();

            int typeCount = 0;
            EntityManager entityManager = state.EntityManager;

            NativeArray<ComponentType> types = new NativeArray<ComponentType>(7, Allocator.Temp);
            types[typeCount++] = ComponentType.ReadWrite<Translation>();
            types[typeCount++] = ComponentType.ReadWrite<Rotation>();
            types[typeCount++] = ComponentType.ReadWrite<FovState>();
            types[typeCount++] = ComponentType.ReadWrite<LocalToWorld>();
            types[typeCount++] = ComponentType.ReadWrite<PostSyncTR_Dots2Mono>();
            types[typeCount++] = ComponentType.ReadWrite<PersistentCamera>();
            types[typeCount++] = ComponentType.ReadWrite<CameraStack>();

            ArchetypeUtil.CreateEntity(in entityManager, in types, out cameraStackEnt);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            translationLookup.Update(ref state);
            rotationLookup.Update(ref state);
            fovStateLookup.Update(ref state);
            cameraInfoLookup.Update(ref state);

            using NativeReference<CameraRequest> highestPriorityRequest = new NativeReference<CameraRequest>(Allocator.TempJob);
            DynamicBuffer<PersistentCamera> persistentCameras = state.EntityManager.GetBuffer<PersistentCamera>(cameraStackEnt);
            DynamicBuffer<CameraStack> cameraStack = state.EntityManager.GetBuffer<CameraStack>(cameraStackEnt);
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

            // 处理 PerFrame Camera 请求
            state.Dependency = new GetHighestPriorityCameraJob {
                highestPriorityRequest = highestPriorityRequest
            }.Schedule(state.Dependency);

            // 处理 Persistent Camera 请求
            state.Dependency = new ProcessPersistentCameraRequestJob {
                persistentCameras = persistentCameras,
                cameraInfoLookup = cameraInfoLookup
            }.Schedule(state.Dependency);

            new DestroyCameraRequestJob {
                ecb = ecb
            }.Schedule(state.Dependency).Complete();

            // 判断是否进行 Blend
            int highestPriority = highestPriorityRequest.Value.priority;
            int persistentHighestPriority = persistentCameras.Length > 0 ? cameraInfoLookup[persistentCameras[^1].cameraEnt].priority : 0;

            Entity requestEnt = highestPriority >= persistentHighestPriority ? highestPriorityRequest.Value.cameraEnt : persistentCameras[^1].cameraEnt;
            Entity prevTopEnt = cameraStack.Length > 0 ? cameraStack[^1].cameraEnt : Entity.Null;

            float fadeDuration = highestPriority >= persistentHighestPriority ? highestPriorityRequest.Value.fadeDuration : cameraInfoLookup[requestEnt].fadeDuration;

            if (requestEnt != prevTopEnt && requestEnt != Entity.Null) {
                cameraStack.Add(new CameraStack {
                    cameraEnt = requestEnt,
                    fadeRatio = cameraStack.Length == 0 ? 1 : 0,
                    fadeDuration = fadeDuration
                });
            }

            if (cameraStack.Length == 0) {
                return;
            }

            // 更新 ratio
            float dt = SystemAPI.Time.DeltaTime;

            for (int i = cameraStack.Length - 1; i >= 0; i--) {
                CameraStack cameraStackInfo = cameraStack[i];

                cameraStackInfo.fadeRatio += dt / cameraStackInfo.fadeDuration;
                cameraStackInfo.fadeRatio = math.min(cameraStackInfo.fadeRatio, 1);
                cameraStack[i] = cameraStackInfo;

                // 当某一层 Camera Ratio 为 1，移除底层所有 Camera
                if (cameraStackInfo.fadeRatio == 1 && i > 0) {
                    cameraStack.RemoveRange(0, i);
                    break;
                }
            }

            // 计算最终值
            Entity cameraEnt = cameraStack[0].cameraEnt;
            var finalTranslation = translationLookup[cameraEnt].Value;
            var finalRotation = rotationLookup[cameraEnt].Value;
            float finalFov = fovStateLookup[cameraEnt].value;

            for (int i = 1; i < cameraStack.Length; i++) {
                CameraStack cameraStackInfo = cameraStack[i];
                cameraEnt = cameraStackInfo.cameraEnt;

                float3 targetTranslation = translationLookup[cameraEnt].Value;
                quaternion targetRotation = rotationLookup[cameraEnt].Value;
                float targetFov = fovStateLookup[cameraEnt].value;

                finalTranslation = math.lerp(finalTranslation, targetTranslation, cameraStackInfo.fadeRatio);
                finalRotation = math.slerp(finalRotation, targetRotation, cameraStackInfo.fadeRatio);
                finalFov = math.lerp(finalFov, targetFov, cameraStackInfo.fadeRatio);
            }

            translationLookup[cameraStackEnt] = new Translation { Value = finalTranslation };
            rotationLookup[cameraStackEnt] = new Rotation { Value = finalRotation };
            fovStateLookup[cameraStackEnt] = new FovState { value = finalFov };
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        partial struct GetHighestPriorityCameraJob : IJobEntity {
            public NativeReference<CameraRequest> highestPriorityRequest;

            [BurstCompile]
            public void Execute(in CameraRequest request) {
                if (request.priority > highestPriorityRequest.Value.priority) {
                    highestPriorityRequest.Value = request;
                }
            }
        }

        [BurstCompile]
        partial struct ProcessPersistentCameraRequestJob : IJobEntity {
            public ComponentLookup<CommonCameraInfo> cameraInfoLookup;
            public DynamicBuffer<PersistentCamera> persistentCameras;

            [BurstCompile]
            public void Execute(in PersistentCameraRequest request) {
                if (request.isActive) {
                    int i = 0;
                    int requestPriority = cameraInfoLookup[request.cameraEnt].priority;

                    for (; i < persistentCameras.Length; i++) {
                        if (requestPriority < cameraInfoLookup[persistentCameras[i].cameraEnt].priority) {
                            break;
                        }
                    }

                    persistentCameras.Insert(i, new PersistentCamera {
                        cameraEnt = request.cameraEnt
                    });
                } else {
                    for (int i = 0; i < persistentCameras.Length; i++) {
                        if (request.cameraEnt == persistentCameras[i].cameraEnt) {
                            persistentCameras.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
        }

        [BurstCompile]
        [WithAny(typeof(CameraRequest), typeof(PersistentCameraRequest))]
        partial struct DestroyCameraRequestJob : IJobEntity {
            public EntityCommandBuffer ecb;

            public void Execute(Entity entity) {
                ecb.DestroyEntity(entity);
            }
        }
    }

    [DisableAutoCreation]
    [RequireMatchingQueriesForUpdate]
    public partial class SyncCameraSystem : SystemBase {
        protected override void OnUpdate() {
            Entities.WithoutBurst().ForEach((Entity entity, Camera camera, in FovState fovState) => {
                if (camera != null && fovState.value > 0) {
                    camera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(fovState.value, camera.aspect);
                }
            }).Run();
        }
    }
}
