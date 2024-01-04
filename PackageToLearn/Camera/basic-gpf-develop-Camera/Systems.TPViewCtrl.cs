using Framework.Core;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;

namespace Framework.GPF {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateBefore(typeof(TPCameraSystem))]
    [UpdateAfter(typeof(GhostComponentSerializerCollectionSystemGroup))]
    public partial struct TPViewCtrlSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<TPCameraState>();
            state.RequireForUpdate<LocalPlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            float dt = SystemAPI.Time.DeltaTime;

            // 网络回滚时 dt 为负数
            if (dt <= 0) {
                return;
            }

            Entity cameraEnt = SystemAPI.GetSingletonEntity<TPCameraState>();
            Entity playerEnt = SystemAPI.GetSingletonEntity<LocalPlayerTag>();
            TPCameraInfo cameraInfo = SystemAPI.GetComponent<TPCameraInfo>(cameraEnt);
            TPCameraState cameraState = SystemAPI.GetComponent<TPCameraState>(cameraEnt);
            BasicPlayerLocalCommand basicPlayerLocalCommand = SystemAPI.GetComponent<BasicPlayerLocalCommand>(playerEnt);

            Entity pivotEnt = cameraState.pivotEnt;
            TPViewCtrlFollowState viewCtrlFollowState = SystemAPI.GetComponent<TPViewCtrlFollowState>(pivotEnt);
            TPViewCtrlRotateState viewCtrlRotateState = SystemAPI.GetComponent<TPViewCtrlRotateState>(pivotEnt);

            Entity actorEnt = viewCtrlFollowState.target;

            float3 pivotPos;
            float2 minOffset = float.MaxValue;

            // reload 会导致一段时间的 player 销毁
            if (state.EntityManager.Exists(actorEnt)) {
                // 更新 Pivot
                Translation actorTrans = SystemAPI.GetComponent<Translation>(actorEnt);
                // Pivot Rotation 用于 DebugCameraSystem
                Rotation actorRot = SystemAPI.GetComponent<Rotation>(actorEnt);
                ActorLocomotionState actorLocomotionState = SystemAPI.GetComponent<ActorLocomotionState>(actorEnt);

                minOffset.x = actorLocomotionState.hullRadius - cameraInfo.sphereRadius;
                minOffset.y = actorLocomotionState.hullHeight / 2 - cameraInfo.sphereRadius - cameraInfo.pivotHeight;

                pivotPos = actorTrans.Value + new float3(0f, actorLocomotionState.hullHeight / 2f, 0f);
                pivotPos.y += cameraInfo.pivotHeight;

                SystemAPI.SetComponent(pivotEnt, new Translation {
                    Value = pivotPos
                });
                SystemAPI.SetComponent(pivotEnt, actorRot);
            } else {
                pivotPos = SystemAPI.GetComponent<Translation>(pivotEnt).Value;
            }

            // 更新基础 Rotate
            float pitch = viewCtrlRotateState.baseEuler.x - basicPlayerLocalCommand.lookDelta.y * cameraInfo.rotateScale;
            float yaw = viewCtrlRotateState.baseEuler.y + basicPlayerLocalCommand.lookDelta.x * cameraInfo.rotateScale;
            float prevYaw = yaw;

            pitch = math.clamp(pitch, cameraInfo.minPitch, cameraInfo.maxPitch);
            yaw = yaw % (2f * math.PI);

            if (yaw < 0f) {
                yaw += 2f * math.PI;
            }

            viewCtrlRotateState.baseEuler.x = pitch;
            viewCtrlRotateState.baseEuler.y = yaw;

            float expectDistance = viewCtrlFollowState.expectDistance - cameraInfo.zoomScale * basicPlayerLocalCommand.zoom;
            expectDistance = math.clamp(expectDistance, cameraInfo.minDistance, cameraInfo.maxDistance);
            viewCtrlFollowState.expectDistance = expectDistance;

            float2 expectOffset = viewCtrlFollowState.expectOffset.Equals(float2.zero) ? cameraInfo.offset : viewCtrlFollowState.expectOffset;
            float expectFov = viewCtrlFollowState.expectFov == 0 ? cameraInfo.fov : viewCtrlFollowState.expectFov;

            // 当目标值归一化时，当前值也需要进行归一化。
            float3 prevCameraRot = new float3(cameraState.pitch, cameraState.yaw + (yaw - prevYaw), cameraState.roll);
            float3 expectRot = viewCtrlRotateState.baseEuler + viewCtrlRotateState.additiveEuler;
            float3 finalRot = mathex.exp_damp(prevCameraRot, expectRot, cameraInfo.zoomDamping, dt);
            float2 finalOffset = mathex.exp_damp(cameraState.offset, expectOffset, cameraInfo.zoomDamping, dt);

            minOffset = math.min(minOffset, finalOffset.xy);

            quaternion cameraRot = quaternion.Euler(new float3(finalRot.x, finalRot.y, 0));
            float3 raycastOffset = math.rotate(cameraRot, new float3(minOffset.x, 0, 0)) + new float3(0, minOffset.y, 0);
            float3 expectPosOffset = math.rotate(cameraRot, math.back() * expectDistance + new float3(finalOffset.x, finalOffset.y, 0));
            float raycastDistance = math.length(expectPosOffset - raycastOffset);
            float3 direction = math.normalize(expectPosOffset - raycastOffset);

            CollisionFilter filter = new CollisionFilter {
                BelongsTo = cameraInfo.belongTo,
                CollidesWith = cameraInfo.collidesWith
            };

            // TODO 遗弃的方案
            // float3 direction = math.rotate(quaternion.Euler(cameraState.pitch, cameraState.yaw, 0), math.back());
            //
            // if (SystemAPI.GetSingleton<PhysicsWorldSingleton>().SphereCast(origin, cameraInfo.sphereRadius, direction, expectDistance, out ColliderCastHit hitInfo, filter)) {
            //     expectDistance = hitInfo.Fraction * expectDistance;
            //     expectOffset *= math.lerp(1, hitInfo.Fraction, cameraInfo.offsetFraction);
            // }

            cameraState.isCollided = SystemAPI.GetSingleton<PhysicsWorldSingleton>().SphereCast(pivotPos + raycastOffset, cameraInfo.sphereRadius, direction, raycastDistance, out ColliderCastHit hitInfo, filter);

            if (cameraState.isCollided) {
                cameraState.collidePosition = pivotPos + raycastOffset + raycastDistance * hitInfo.Fraction * direction;
            }

            cameraState.pitch = finalRot.x;
            cameraState.yaw = finalRot.y;
            cameraState.roll = finalRot.z;
            cameraState.offset = finalOffset;
            cameraState.distance = mathex.exp_damp(cameraState.distance, expectDistance, cameraInfo.zoomDamping, dt);
            cameraState.fov = mathex.exp_damp(cameraState.fov, expectFov, cameraInfo.zoomDamping, dt);

            SystemAPI.SetComponent(cameraEnt, cameraState);
            SystemAPI.SetComponent(pivotEnt, viewCtrlRotateState);
            SystemAPI.SetComponent(pivotEnt, viewCtrlFollowState);
        }

        public void OnDestroy(ref SystemState state) { }
    }
}