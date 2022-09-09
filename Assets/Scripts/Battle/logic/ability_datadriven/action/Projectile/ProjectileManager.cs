using System.Collections.Generic;

namespace Battle.logic.ability_dataDriven {
    public class ProjectileManager : Singleton<ProjectileManager> {
        private List<Projectile> projectiles;
        private List<Projectile> projectilePool;

        public ProjectileManager() {
            projectiles = new List<Projectile>();
            projectilePool = new List<Projectile>();
        }

        public void OnUpdate() {
            for (var i = 0; i < projectiles.Count; i++) {
                var projectile = projectiles[i];
                projectile.OnUpdate();
            }
        }

        private void ReleaseProjectile(Projectile projectile) {
            for (int i = 0; i < projectiles.Count; i++) {
                if (projectiles[i] == projectile) {
                    projectiles.RemoveAt(i);
                    projectilePool.Add(projectile);
                    projectile.gameObject.SetActive(false);
                }
            }
        }
        
        public T GetProjectile<T>(string effectSign) where T: Projectile {
            var length = projectilePool.Count;
            if (length == 0) {
                return GenerateProjectile<T>(effectSign);;
            } else {
                for (int i = 0; i < length; i++) {
                    var projectile = projectilePool[i];
                    // 性能热点！！
                    if (projectile.GetType() == typeof(T)) {
                        projectilePool.RemoveAt(i);
                        projectile.gameObject.SetActive(true);
                        return projectile as T;
                    }
                }
            }

            return default;
        }

        private T GenerateProjectile<T>(string effectSign) where T: Projectile {
            return default;
        }
    }
}