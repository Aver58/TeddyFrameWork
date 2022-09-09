using System.Collections.Generic;

namespace Origins {
    public interface IAbilityEntity {
        List<Battle.logic.ability_dataDriven.Ability> GetAllAbility();
        IAbilityEntity GetCaster();
    }
}