
public class BattleBehaviorWorkingData : BattleWorkingData
{
    public float deltaTime { get; set; }
    public float gameTime { get; set; }

    public BattleBehaviorWorkingData(BattleEntity battleEntity):base(battleEntity)
    {

    }
}