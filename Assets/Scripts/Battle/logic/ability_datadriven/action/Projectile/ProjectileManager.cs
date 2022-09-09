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

        // public T GetProjectile<T>() where T : Projectile {
        //     T projectile = default;
        //     var length = projectilePool.Count;
        //     if (length == 0) {
        //
        //     } else {
        //         projectile = projectilePool[length - 1];
        //         // enemyActorPool.RemoveAt(length - 1);
        //         // enemyActor.gameObject.SetActive(true);
        //     }
        // }
    }
}