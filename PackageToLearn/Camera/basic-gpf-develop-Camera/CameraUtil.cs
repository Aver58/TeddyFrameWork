using Unity.Burst;
using Unity.Entities;

namespace Framework.GPF {
    [BurstCompile]
    public static class CameraUtil {
        [BurstCompile]
        public static void RequestCamera(in EntityManager entityManager, in Entity cameraEnt, int priority, float fadeDuration) {
            entityManager.AddComponentData(entityManager.CreateEntity(), new CameraRequest {
                cameraEnt = cameraEnt,
                priority = priority,
                fadeDuration = fadeDuration
            });
        }

        [BurstCompile]
        public static void RequestPersistentCamera(in EntityManager entityManager, in Entity cameraEnt, bool active) {
            entityManager.AddComponentData(entityManager.CreateEntity(), new PersistentCameraRequest {
                cameraEnt = cameraEnt,
                isActive = active
            });
        }
    }
}