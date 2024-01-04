using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Framework.GPF {
    [BurstCompile]
    [DisableAutoCreation]
    [UpdateBefore(typeof(ExplosionShakeSystem))]
    public partial struct ExplosionSourceSystem : ISystem {
        private EntityArchetype shakeTaskArchetype;
        private EntityQuery destroySourceQuery;
        private ComponentLookup<DestroyExplosionSource> destroySourceLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            NativeArray<ComponentType> types = new NativeArray<ComponentType>(2, Allocator.Temp);
            types[0] = ComponentType.ReadWrite<ExplosionShakeTask>();
            types[1] = ComponentType.ReadWrite<StopExplosionShakeTask>();
            shakeTaskArchetype = state.EntityManager.CreateArchetype(types);

            destroySourceLookup = state.GetComponentLookup<DestroyExplosionSource>();
            destroySourceQuery = state.GetEntityQuery(ComponentType.ReadWrite<DestroyExplosionSource>());

            state.RequireForUpdate<LocalActorTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            destroySourceLookup.Update(ref state);

            float dt = SystemAPI.Time.DeltaTime;
            Entity actorEnt = SystemAPI.GetSingletonEntity<LocalActorTag>();
            Translation actorTrans = state.EntityManager.GetComponentData<Translation>(actorEnt);
            var buffer = new EntityCommandBuffer(Allocator.TempJob);

            state.Dependency = new ApplyExplosionSourceJob {
                dt = dt,
                actorPos = actorTrans.Value,
                commandBuffer = buffer,
                shakeTaskArchetype = shakeTaskArchetype,
                destroySourceLookup = destroySourceLookup
            }.Schedule(state.Dependency);
            state.Dependency.Complete();

            buffer.Playback(state.EntityManager);
            state.EntityManager.DestroyEntity(destroySourceQuery);
        }

        public void OnDestroy(ref SystemState state) { }

        [BurstCompile]
        [WithNone(typeof(DestroyExplosionSource))]
        public partial struct ApplyExplosionSourceJob : IJobEntity {
            public float dt;
            public float3 actorPos;
            public EntityArchetype shakeTaskArchetype;
            public EntityCommandBuffer commandBuffer;
            public ComponentLookup<DestroyExplosionSource> destroySourceLookup;

            [BurstCompile]
            void Execute(Entity entity, ref ExplosionSourceState sourceState, in ExplosionSourceDesc sourceDesc, in Translation translation) {
                sourceState.elapsedTime += dt;

                if (sourceState.elapsedTime < sourceDesc.loopTime) {
                    return;
                }

                float amplitude = CalculateShakeAmplitude(translation.Value, actorPos, sourceDesc.amplitude, sourceDesc.radius);

                if (amplitude > 0f) {
                    Entity shakeEnt = commandBuffer.CreateEntity(shakeTaskArchetype);
                    commandBuffer.SetComponentEnabled<StopExplosionShakeTask>(shakeEnt, false);
                    commandBuffer.SetComponent(shakeEnt, new ExplosionShakeTask {
                        amplitude = amplitude,
                        duration = sourceDesc.duration,
                        frequency = sourceDesc.frequency
                    });
                }

                if (sourceDesc.loop) {
                    sourceState.elapsedTime = 0f;
                } else {
                    destroySourceLookup.SetComponentEnabled(entity, true);
                }
            }
        }

        // 根据角色位置计算对应角色的实际振幅
        public static float CalculateShakeAmplitude(float3 origin, float3 chrPos, float maxAmplitude, float radius) {
            if (radius <= 0f) {
                return maxAmplitude;
            }

            float amplitude = 0f;
            float distance = math.distance(origin, chrPos);

            if (distance <= radius) {
                float fraction = 1f - (distance / radius);
                amplitude = maxAmplitude * fraction;
            }

            return amplitude;
        }
    }

}