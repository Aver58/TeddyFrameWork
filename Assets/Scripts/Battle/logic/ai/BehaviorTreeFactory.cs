// 共享行为树
//http://www.aisharing.com/archives/750
using TsiU;

public class BehaviorTreeFactory : Singleton<BehaviorTreeFactory>
{
	private static TBTAction m_DecisionTree;
	private static TBTAction m_BehaviorTreeNode;

	public static TBTAction GetDecisionTree()
	{
		if(m_DecisionTree == null)
		{
			m_DecisionTree = new TBTActionPrioritizedSelector();
			m_DecisionTree.SetPrecondition(new CON_CanUpdateRequest());
			m_DecisionTree.AddChild(new NOD_UpdateRequest());
		}
		return m_DecisionTree;
	}

	public static TBTAction GetBehaviorTree()
	{
		if(m_BehaviorTreeNode == null)
		{
			// 父节点
			m_BehaviorTreeNode = new TBTActionPrioritizedSelector();

			/// 通用节点
			// 追逐节点
			TBTTreeNode moveToNode = new NOD_MoveTo().SetPrecondition(new CON_IsInRange());
			// 转向节点
			TBTTreeNode turnToNode = new NOD_TurnTo().SetPrecondition(new CON_IsAngleNeedTurnTo());

			/// 开始构造树
			// ①追逐
			TBTAction chaseSelectorNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsChaseRequest());
			chaseSelectorNode.AddChild(turnToNode);
			chaseSelectorNode.AddChild(moveToNode);

			// 技能转向节点
			TBTPreconditionAND abilityTurnCondition = new TBTPreconditionAND(new CON_IsAbilityNeedTurnTo(),new CON_IsAngleNeedTurnTo());
			TBTActionPrioritizedSelector abilityTurnToNode = new TBTActionPrioritizedSelector();
			abilityTurnToNode.SetPrecondition(abilityTurnCondition);
			abilityTurnToNode.AddChild(new NOD_TurnTo());
			// 技能追逐节点
			TBTPreconditionNOT abilityMoveCondition = new TBTPreconditionNOT(new CON_IsInAbilityRange());
			TBTActionPrioritizedSelector abilityMoveToNode = new TBTActionPrioritizedSelector();
			abilityMoveToNode.SetPrecondition(abilityMoveCondition);
			abilityMoveToNode.AddChild(new NOD_MoveTo());
			// 技能施法节点
			TBTActionPrioritizedSelector castAbilityNode = new TBTActionPrioritizedSelector();
			castAbilityNode.SetPrecondition(new CON_CanCastAbility());
			castAbilityNode.AddChild(new NOD_CastAbility());

			// ②自动战斗
			TBTAction autoCastAbilitySelectorNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsAutoCastAbilityRequest());
			// 停止施法、转向、移动、施法
			autoCastAbilitySelectorNode.AddChild(abilityTurnToNode);
			autoCastAbilitySelectorNode.AddChild(abilityMoveToNode);
			autoCastAbilitySelectorNode.AddChild(castAbilityNode);

			m_BehaviorTreeNode.AddChild(chaseSelectorNode);
			m_BehaviorTreeNode.AddChild(autoCastAbilitySelectorNode);
		}

		return m_BehaviorTreeNode;
	}
}
