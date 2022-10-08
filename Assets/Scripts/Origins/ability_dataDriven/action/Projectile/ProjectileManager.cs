using System;
using System.Collections.Generic;
using Origins;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : Singleton<ProjectileManager>, ILifeCycle {
        public int AutoIndex = 0;

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
        private ProjectileFactory GetProjectileFactory(ProjectileType projectileType) {
            switch (projectileType) {
                case ProjectileType.Linear:
                    return new LinearProjectileFactory();
                case ProjectileType.Tracking:
                    break;
                case ProjectileType.Bouncing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectileType), projectileType, null);
            }
            return default;
        }
    }
}