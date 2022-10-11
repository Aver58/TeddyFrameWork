using System;

namespace Battle.logic.ability_dataDriven {
    public interface IProjectileFactory { 
        ProjectileEntity GetProjectile();
    }

    public sealed class LinearProjectileFactory : IProjectileFactory {
        private ObjectPool<LinearProjectile> projectilePool = new ObjectPool<LinearProjectile>();
        public ProjectileEntity GetProjectile() {
            return projectilePool.Get();
        }
    }
}