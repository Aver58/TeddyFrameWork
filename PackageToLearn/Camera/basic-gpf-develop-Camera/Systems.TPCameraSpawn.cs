using Framework.Core;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Framework.GPF {
    [DisableAutoCreation]
    public partial class TPCameraSpawnSystem : SystemBase {
        private EntityQuery requestQuery;
        private EntityQuery entityPrefabQuery;

        protected override void OnCreate() {
            requestQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] {
                    ComponentType.ReadOnly<SpawnEntityPrefabRequest>(),
                    ComponentType.ReadOnly<TPCameraSpawnInfo>()
                },
                Options = EntityQueryOptions.FilterWriteGroup
            });

            entityPrefabQuery = GetEntityQuery(new EntityQueryDesc {
                All = new ComponentType[] {
                    typeof(Prefab),
                    typeof(EntityPrefabSharedSource)
                },
                Options = EntityQueryOptions.FilterWriteGroup
            });

            RequireForUpdate(requestQuery);
            RequireForUpdate(entityPrefabQuery);
        }

        protected override void OnUpdate() {
            EntityManager entityManager = EntityManager;

            Entities
                .WithStructuralChanges()
                .WithEntityQueryOptions(EntityQueryOptions.FilterWriteGroup)
                .ForEach((Entity entity, in SpawnEntityPrefabRequest request, in TPCameraSpawnInfo spawnInfo) => {
                    entityPrefabQuery.SetSharedComponentFilter(new EntityPrefabSharedSource {
                        url = request.url
                    });
                    Entity entityPrefab = entityPrefabQuery.GetSingletonEntity();
                    entityPrefabQuery.ResetFilter();
                    Entity cameraEnt = EntityManager.Instantiate(entityPrefab);
                    EntityManager.AddComponent<LevelLifetimeTag>(cameraEnt);
                    TPCameraInfo cameraInfo = EntityManager.GetComponentData<TPCameraInfo>(cameraEnt);

                    // spawn pivot
                    Entity pivotEnt = EntityManager.CreateEntity(typeof(TPViewCtrlFollowState), typeof(TPViewCtrlRotateState), typeof(Translation), typeof(Rotation), typeof(LevelLifetimeTag));
                    EntityManager.SetComponentData(pivotEnt, new TPViewCtrlFollowState {
                        target = spawnInfo.target,
                        expectDistance = cameraInfo.distance
                    });

                    // init camera state
                    EntityManager.AddComponent<EntityPrefabInstanceTag>(cameraEnt);
                    EntityManager.SetComponentData(cameraEnt, new TPCameraState {
                        pitch = cameraInfo.pitch,
                        yaw = cameraInfo.yaw,
                        roll = cameraInfo.roll,
                        distance = cameraInfo.distance,
                        pivotEnt = pivotEnt
                    });
                }).Run();

            EntityManager.DestroyEntity(requestQuery);
        }
    }
}
