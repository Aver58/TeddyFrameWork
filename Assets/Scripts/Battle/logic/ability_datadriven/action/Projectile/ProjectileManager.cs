using System;
using System.Collections.Generic;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : Singleton<ProjectileManager> {
        private List<ProjectileEntity> projectiles;
        private ObjectPool<ProjectileEntity> projectileEntityPool;
        private GameObjectPool projectileActorPool;

        public ProjectileManager() {
            projectiles = new List<ProjectileEntity>();
            projectileEntityPool = new ObjectPool<ProjectileEntity>();
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
        
        public ProjectileEntity GetProjectile() {
            var projectileEntity = projectileEntityPool.Get();
            
            return projectileEntity;
        }

        public void GetActorAsync(string effectName, Action<GameObject> callback) {
            projectileActorPool.GetAsync(effectName, callback);
        }
    }
}