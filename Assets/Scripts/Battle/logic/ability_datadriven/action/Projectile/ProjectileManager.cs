using System.Collections.Generic;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : Singleton<ProjectileManager> {
        private List<ProjectileEntity> projectiles;
        private ObjectPool<ProjectileEntity> projectileEntityPool;

        public ProjectileManager() {
            projectiles = new List<ProjectileEntity>();
            projectileEntityPool = new ObjectPool<ProjectileEntity>();
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
        
        public ProjectileEntity GetProjectile(string effectSign) {
            var projectileEntity = projectileEntityPool.Get();
            
            // projectileEntity.AddProjectileActor(effectSign);
            return projectileEntity;
        }

        private ProjectileActor AddProjectileActor(string effectSign) {
            
            
            return default;
        }
    }
}