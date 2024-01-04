using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Framework.GPF {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateBefore(typeof(CameraSystem))]
    public partial struct TPCameraSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TPCameraState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float3 position;
            quaternion rotation;
            Entity cameraEnt = SystemAPI.GetSingletonEntity<TPCameraState>();
            TPCameraState cameraState = SystemAPI.GetComponent<TPCameraState>(cameraEnt);
            Translation pivotTrans = SystemAPI.GetComponent<Translation>(cameraState.pivotEnt);
            CameraShakeState shakeState = state.EntityManager.GetComponentData<CameraShakeState>(cameraEnt);
            float3 rotationAngle = new float3(cameraState.pitch, cameraState.yaw, cameraState.roll) + shakeState.rotation;

            if (cameraState.isCollided) {
                position = cameraState.collidePosition + shakeState.translation;
                rotation = quaternion.Euler(rotationAngle);
            } else {
                float4x4 pivotToWorld = new float4x4(quaternion.Euler(rotationAngle.x, rotationAngle.y, 0), pivotTrans.Value);
                float4x4 cameraToWorld = math.mul(pivotToWorld, float4x4.Translate(new float3(cameraState.offset.x, cameraState.offset.y, -cameraState.distance)));
                cameraToWorld = math.mul(cameraToWorld, float4x4.Euler(0, 0, rotationAngle.z));

                position = cameraToWorld.c3.xyz + shakeState.translation;
                rotation = new quaternion(cameraToWorld);
            }

            SystemAPI.SetComponent(cameraEnt, new Translation { Value = position });
            SystemAPI.SetComponent(cameraEnt, new Rotation { Value = rotation });
            SystemAPI.SetComponent(cameraEnt, new FovState { value = cameraState.fov });
        }

        public void OnDestroy(ref SystemState state) { }
    }
}