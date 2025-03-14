using System;
using System.Collections.Generic;
using Origins;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : MonoSingleton<ProjectileManager>, ILifeCycle {
        public int AutoIndex = 0;

        private List<ProjectileEntity> projectiles;
        private ObjectPool<ProjectileEntity> projectileEntityPool;
        private Dictionary<ProjectileType, IProjectileFactory> projectileFactoryMap;

        public void OnInit() {
            projectiles = new List<ProjectileEntity>();
            projectileEntityPool = new ObjectPool<ProjectileEntity>();
            projectileFactoryMap = new Dictionary<ProjectileType, IProjectileFactory> {
                {ProjectileType.Linear, new LinearProjectileFactory()},
            };
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

        public void CreateLinearProjectile() {
            
        }
        
        public void CreateTrackingProjectile() {
            
        }
        
        public ProjectileEntity GetProjectile(ProjectileType projectileType) {
            var factory = GetProjectileFactory(projectileType);
            var projectileEntity = factory.GetProjectile();
            projectiles.Add(projectileEntity);
            
            return projectileEntity;
        }

        // 简单工厂
        private IProjectileFactory GetProjectileFactory(ProjectileType projectileType) {
            return projectileFactoryMap.ContainsKey(projectileType)? projectileFactoryMap[projectileType] : default;
        }
    }
}