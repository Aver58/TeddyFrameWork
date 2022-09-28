using System;
using System.Collections.Generic;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : Singleton<ProjectileManager> {
        private static Dictionary<ProjectileType, Type> projectileClassMap = new Dictionary<ProjectileType, Type>() {
            { ProjectileType.Linear, typeof(LinearProjectile) },
        }; 
        private List<ProjectileEntity> projectiles;
        private Dictionary<ProjectileType, ObjectPool<ProjectileEntity>> projectileEntityPoolMap;
        private GameObjectPool projectileActorPool;

        public event Action OnCreateProjectile;
        
        public ProjectileManager() {
            projectiles = new List<ProjectileEntity>();
            projectileEntityPoolMap = new Dictionary<ProjectileType, ObjectPool<ProjectileEntity>>();
            projectileActorPool = new GameObjectPool(LoadModule.BULLET_PATH_PREFIX);
        }

        public void OnUpdate() {
            for (var i = 0; i < projectiles.Count; i++) {
                var projectile = projectiles[i];
                projectile.OnUpdate();
            }
        }

        private void ReleaseProjectile(ProjectileActor projectileActor) {
            // for (int i = 0; i < projectiles.Count; i++) {
            //     if (projectiles[i].Id == projectile.Id) {
            //         projectiles.RemoveAt(i);
            //         linearProjectilePool.Add(projectile);
            //         projectile.gameObject.SetActive(false);
            //     }
            // }
        }
        
        public ProjectileEntity GetProjectile(ProjectileType projectileType) {
            if (!projectileEntityPoolMap.ContainsKey(projectileType)) {
                projectileEntityPoolMap[projectileType] = new ObjectPool<ProjectileEntity>();
            }
            
            var projectileEntity = projectileEntityPoolMap[projectileType].Get();
            projectiles.Add(projectileEntity);
            
            return projectileEntity;
        }

        public void GetActorAsync(string effectName, Action<GameObject> callback) {
            projectileActorPool.GetAsync(effectName, callback);
        }

        public void CreateLinearProjectile(AbsEntity caster, string effectName, Vector3 sourcePosition, Vector3 sourceForward, float moveSpeed, float fixedDistance) {
            
        }
        
        // private void OnCreateProjectile(GameObject instance) {
        //     if (instance != null) {
        //         instance.transform.SetParent(UIModule.Instance.GetParentTransform(ViewType.MAIN));
        //         actor = instance.GetComponent<ProjectileActor>();
        //         if (actor != null) {
        //             var transform = actor.transform;
        //             transform.localPosition = LocalPosition;
        //             var angle = Vector3.SignedAngle(Vector3.zero, flyToward, Vector3.forward);
        //             transform.localRotation = new Quaternion(0, 0, angle, 1);
        //         }
        //     } else {
        //         Debug.LogError("[GetActorAsync] 回调返回的go为空！");
        //     }
        // }
    }
}