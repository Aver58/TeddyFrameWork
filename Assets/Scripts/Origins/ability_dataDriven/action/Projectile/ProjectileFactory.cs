using System;

namespace Battle.logic.ability_dataDriven {
    public abstract class ProjectileFactory {
        public abstract ProjectileEntity GetProjectile();
    }

    public sealed class LinearProjectileFactory : ProjectileFactory {
        private ObjectPool<LinearProjectile> projectilePool = new ObjectPool<LinearProjectile>();
        public override ProjectileEntity GetProjectile() {
            return projectilePool.Get();
        }
    }
}