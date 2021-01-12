using System.Collections.Generic;

public enum BattleCamp
{
    FRIENDLY = 0,           // 友方
    ENEMY,                  // 敌方
}

public enum HeroPropertyType
{
    MAX_HP = 1,   
    MAGIC = 2,     
}
public enum RequestType
{
    Idle = 1,
    Chase,
    AutoCastAbility,
    ManualCastAbility,
}

public static class BattleDefine
{
    public static float FireInterval = 1f;

    public static Dictionary<HeroPropertyType, string> HERO_PROPERTY_TYPE_MAP = new Dictionary<HeroPropertyType, string>{
        { HeroPropertyType.MAX_HP , "maxhp"},
    };
}

