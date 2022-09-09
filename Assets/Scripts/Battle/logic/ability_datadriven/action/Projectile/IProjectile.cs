using System;

namespace Battle.logic.ability_dataDriven {
    public interface IProjectile {
        void OnUpdate();
        void OnClear();
    }
}