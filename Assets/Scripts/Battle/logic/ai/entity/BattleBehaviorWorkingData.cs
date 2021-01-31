
public class BattleBehaviorWorkingData : BattleWorkingData
{
    public float deltaTime { get; set; }
    public float gameTime { get; set; }

    public BattleBehaviorWorkingData(BattleUnit battleEntity):base(battleEntity)
    {

    }
}