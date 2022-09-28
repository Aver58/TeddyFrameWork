#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    D2Action_IsHit.cs
 Author:      Zeng Zhiwei
 Time:        2020\6\21 星期日 13:18:01
=====================================================
*/
#endregion

using System.Collections.Generic;

public class D2Action_IsHit : D2Action
{
    private List<D2Action> m_SuccessActions;
    public D2Action_IsHit(List<D2Action> successActions, ActionTarget actionTarget) : base(actionTarget)
    {
        m_SuccessActions = successActions;
    }

    //todo 计算闪避
    private bool IsDodge(BattleUnit source, BattleUnit target)
    {
        return false;
    }

    protected override void ExecuteByUnit(BattleUnit source, List<BattleUnit> targets)
    {
        for(int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if(target != null)
            {
                if(IsDodge(source,target))
                {
                    BattleLog.LogTargetDodge(BattleLogic.instance.logicFrame, source, target, abilityData.configFileName);
                    GameMsg.instance.DispatchEvent(GameMsgDef.DAMAGE_EFFECT_NOTICED, target.GetUniqueID(), DamageEffectDefine.DODGE);
                }
                else
                {
                    if(m_SuccessActions != null)
                    {
                        for(int j = 0; j < m_SuccessActions.Count; j++)
                            m_SuccessActions[j].Execute(source, abilityData, requestTarget);
                    }
                }
            }
        }
    }
}