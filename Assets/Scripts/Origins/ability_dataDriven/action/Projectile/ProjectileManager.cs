using System;
using System.Collections.Generic;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : Singleton<ProjectileManager>, ILifeCycle {
        private List<ProjectileEntity> projectiles;
        private ObjectPool<ProjectileEntity> projectileEntityPool;

        public void OnInit() {
            projectiles = new List<ProjectileEntity>();
            projectileEntityPool = new ObjectPool<ProjectileEntity>();
        }

        public void OnUpdate() {
            for (var i = 0; i < projectiles.Count; i++) {
                var projectile = projectiles[i];
                projectile.OnUpdate();
            }
        }

        public void OnClear() {

        }

        private void ReleaseProjectile(ProjectileEntity projectileEntity) {
            // for (int i = 0; i < projectiles.Count; i++) {
            //     if (projectiles[i].Id == projectile.Id) {
            //         projectiles.RemoveAt(i);
            //         linearProjectilePool.Add(projectile);
            //         projectile.gameObject.SetActive(false);
            //     }
            // }
        }
        
        public T GetProjectile<T>(ProjectileType projectileType) where T : ProjectileEntity {
            // todo 这里怎么做泛型？ projectileType 对应 不同的对象池

            var projectileEntity = projectileEntityPool.Get();
            projectiles.Add(projectileEntity);

            if (!(projectileEntity is T returnValue)) {
                return default;
            }

            returnValue.ProjectileType = projectileType;
            return returnValue;
        }
    }
}