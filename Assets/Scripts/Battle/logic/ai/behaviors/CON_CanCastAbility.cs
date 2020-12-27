using TsiU;

public class CON_CanCastAbility : TBTPreconditionLeaf
{
    public override bool IsTrue(TBTWorkingData wData)
    {
        BattleBehaviorWorkingData behaviorData = wData as BattleBehaviorWorkingData;
        AutoCastAbilityRequest request = behaviorData.request as AutoCastAbilityRequest;
        //BattleLog.Log("【CON_CanCastAbility】能否施法：{0}", request.ability.IsCastable().ToString());
        return request.ability.IsCastable();
    }
}