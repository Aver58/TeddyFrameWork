using Framework;
using Framework.GPF;

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Simple.TPS {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateBefore(typeof(CameraSystem))]
    public partial struct FixedCameraSystem : ISystem {
        private EntityQuery fixedCameraQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<FixedCameraState>();
            state.RequireForUpdate<LocalActorTag>();

            fixedCameraQuery = state.GetEntityQuery(ComponentType.ReadOnly<FixedCameraState>());
        }

        [ExcludeFromBurstCompatTesting("DebugDraw")]
        public void OnUpdate(ref SystemState state) {
            Entity actorEnt = SystemAPI.GetSingletonEntity<LocalActorTag>();
            float3 actorPos = state.EntityManager.GetComponentData<Translation>(actorEnt).Value;

            NativeArray<Entity> entities = fixedCameraQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entities.Length; i++) {
                Entity fixedCameraEnt = entities[i];
                FixedCameraInfo fixedCameraInfo = state.EntityManager.GetComponentData<FixedCameraInfo>(fixedCameraEnt);
                FixedCameraState fixedCameraState = state.EntityManager.GetComponentData<FixedCameraState>(fixedCameraEnt);

                float3 triggerPos = fixedCameraInfo.triggerPosition;
                bool active = math.distance(actorPos.xz, triggerPos.xz) <= fixedCameraInfo.radius;

                if (fixedCameraState.active != active) {
                    fixedCameraState.active = active;
                    state.EntityManager.SetComponentData(fixedCameraEnt, fixedCameraState);

                    CameraUtil.RequestPersistentCamera(state.EntityManager, fixedCameraEnt, fixedCameraState.active);
                }

                DebugDraw.DrawCylinder3D(triggerPos, quaternion.identity, 2, fixedCameraInfo.radius, color : Color.green, meshTypeSetting : MeshType.Mixed, isOverhead : true);
            }
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) { }
    }
}