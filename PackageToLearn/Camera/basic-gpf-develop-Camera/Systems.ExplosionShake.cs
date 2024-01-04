using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Framework.GPF {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateBefore(typeof(TPViewCtrlSystem))]
    public partial struct ExplosionShakeSystem : ISystem {
        private EntityQuery cameraShakeQuery;
        private EntityQuery stopTaskQuery;

        private NativeReference<Random> random;

        private ComponentLookup<CameraShakeDesc> shakeDescLookup;
        private ComponentLookup<CameraShakeState> shakeStateLookup;
        private ComponentLookup<StopExplosionShakeTask> stopTaskLookup;

        public void OnCreate(ref SystemState state) {
            random = new NativeReference<Random>(Allocator.Persistent);
            random.Value = new Random((uint)System.DateTime.Now.Ticks);

            shakeDescLookup = state.GetComponentLookup<CameraShakeDesc>(true);
            shakeStateLookup = state.GetComponentLookup<CameraShakeState>();
            stopTaskLookup = state.GetComponentLookup<StopExplosionShakeTask>();

            stopTaskQuery = state.GetEntityQuery(ComponentType.ReadWrite<StopExplosionShakeTask>());
            cameraShakeQuery = state.GetEntityQuery(ComponentType.ReadWrite<CameraShakeState>());

            NativeArray<EntityQuery> queries = new NativeArray<EntityQuery>(2, Allocator.Temp);
            queries[0] = cameraShakeQuery;
            queries[1] = stopTaskQuery;
            state.RequireAnyForUpdate(queries);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            shakeDescLookup.Update(ref state);
            shakeStateLookup.Update(ref state);
            stopTaskLookup.Update(ref state);

            float dt = SystemAPI.Time.DeltaTime;
            NativeReference<float3> explosionRotation = new NativeReference<float3>(Allocator.TempJob);
            NativeReference<float3> explosionTranslation = new NativeReference<float3>(Allocator.TempJob);

            state.Dependency = new ApplyExplosionShakeJob {
                dt = dt,
                random = random,
                stopTaskLookup = stopTaskLookup,
                explosionRotation = explosionRotation,
                explosionTranslation = explosionTranslation,
            }.Schedule(state.Dependency);

            NativeList<Entity> entities = cameraShakeQuery.ToEntityListAsync(state.WorldUpdateAllocator, state.Dependency, out JobHandle jobHandle);
            jobHandle.Complete();

            for (int i = 0; i < entities.Length; i++) {
                Entity entity = entities[i];
                CameraShakeState shakeState = shakeStateLookup[entity];
                CameraShakeDesc shakeDesc = shakeDescLookup[entity];

                shakeState.explosionRotation = explosionRotation.Value * shakeDesc.explosionViewRotateFraction;
                shakeState.explosionTranslation = explosionTranslation.Value * shakeDesc.explosionViewTranslateFraction;

                // TODO 后续有新的 Shake 需要拆出 CameraShakeSystem
                shakeState.rotation = shakeState.explosionRotation;
                shakeState.translation = shakeState.explosionTranslation;
                shakeStateLookup[entity] = shakeState;
            }

            state.EntityManager.DestroyEntity(stopTaskQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state) {
            random.Dispose();
        }

        [BurstCompile]
        public partial struct ApplyExplosionShakeJob : IJobEntity {
            public float dt;
            public NativeReference<Random> random;
            public NativeReference<float3> explosionRotation;
            public NativeReference<float3> explosionTranslation;
            public ComponentLookup<StopExplosionShakeTask> stopTaskLookup;

            [BurstCompile]
            void Execute(Entity entity, ref ExplosionShakeTask task) {
                if (task.duration < task.elapsedTime
                    || task.duration <= 0f
                    || task.frequency <= 0f
                    || task.amplitude <= 0f) {
                    stopTaskLookup.SetComponentEnabled(entity, true);
                    return;
                }

                if (task.elapsedTime >= task.nextShakeTime) {
                    task.nextShakeTime += (1f / task.frequency);
                    task.posOffset = random.Value.NextFloat3(-task.amplitude, task.amplitude);
                    task.rotOffset = random.Value.NextFloat3(-task.amplitude, task.amplitude);
                }

                // NOTES:
                // TTF2 中，每经过 1/shakeSource.frequency 时间，就会完成一次从 0 -> 1 | -1 -> 0 的正弦运动，即 sin(x) x -> (0, π) or x -> (π，2π)。
                // 而 source engine 的 x = elapsedTime * (shakeSource.frequency / fraction)，没法保证上述周期性现象。
                // 所以调整了 x 的算法为 x = (frequency / 2f) * elapsedTime * 2π，以保证以 ttf2 的周期运动一致
                float fraction = (task.duration - task.elapsedTime) / task.duration;

                fraction *= fraction;
                fraction *= math.sin(task.frequency * task.elapsedTime * math.PI);

                // pitch 只有正数的 shake
                float3 rotOffset = math.radians(task.rotOffset * fraction);
                rotOffset.x = math.abs(rotOffset.x);
                explosionRotation.Value += rotOffset;
                explosionTranslation.Value += task.posOffset * fraction;

                // amplitude -= amplitude * (dt / duration) * (1 / frequency)
                task.amplitude -= task.amplitude * dt / (task.duration * task.frequency);
                task.elapsedTime += dt;
            }
        }
    }
}