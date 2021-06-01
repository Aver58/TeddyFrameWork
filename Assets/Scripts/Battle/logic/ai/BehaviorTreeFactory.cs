// 共享行为树
//http://www.aisharing.com/archives/750
using TsiU;
using Aver3;

public class BehaviorTreeFactory : Singleton<BehaviorTreeFactory>
{
	private static TBTActionPrioritizedSelector m_DecisionTree;
	private static TBTActionPrioritizedSelector m_BehaviorTree;
	private static TBTActionPrioritizedSelector m_MobaBehaviorTree;

	public static TBTActionPrioritizedSelector GetDecisionTree()
	{
		if(m_DecisionTree == null)
		{
			m_DecisionTree = new TBTActionPrioritizedSelector();
			m_DecisionTree.SetPrecondition(new CON_CanUpdateRequest());
			m_DecisionTree.AddChild(new NOD_UpdateRequest());
		}
		return m_DecisionTree;
	}

	// 追逐节点
	public BTPrioritySelector BuildChaseNode()
	{
		var node = new BTPrioritySelector();
		node.SetPrecondition(new CON_IsChaseRequest());
		var turnToSelectorNode = new BTPrioritySelector();
		turnToSelectorNode.SetPrecondition(new CON_IsNeedTurnTo());

		//node.AddChild();
		return node;
	}

	public TBTActionPrioritizedSelector GetMobaBehaviorTree()
	{
		if(m_MobaBehaviorTree == null)
		{
			m_MobaBehaviorTree = new TBTActionPrioritizedSelector();
			//m_MobaBehaviorTree.
		}
		return m_MobaBehaviorTree;
	}

	public static TBTActionPrioritizedSelector GetBehaviorTree()
	{
		if(m_BehaviorTree == null)
		{
			// 父节点
			m_BehaviorTree = new TBTActionPrioritizedSelector();

			// 设置idle节点
			var setUnitIdleNode = new NOD_SetUnitIdle();
			// 转向节点
			var turnToSelectorNode = new TBTActionPrioritizedSelector();
			turnToSelectorNode.SetPrecondition(new CON_IsNeedTurnTo());
			turnToSelectorNode.AddChild(new NOD_TurnTo());//todo can turn to
			//turnToSelectorNode.AddChild(setUnitIdleNode);
			// 追逐节点
			var moveToSelectorNode = new TBTActionPrioritizedSelector();
			moveToSelectorNode.SetPrecondition(new CON_IsInRange());
			moveToSelectorNode.AddChild(new NOD_MoveTo());//todo can Move to
			//moveToSelectorNode.AddChild(setUnitIdleNode);

			// ①追逐
			//TBTAction chaseSelectorNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsChaseRequest());
			//chaseSelectorNode.AddChild(turnToSelectorNode);
			//chaseSelectorNode.AddChild(moveToSelectorNode);

			// ②自动战斗节点构建
			// 转向和追逐并行
			var abilityTurnMoveToParallelNode = new TBTActionParallel();
			// 技能转向节点
			TBTPreconditionAND abilityTurnCondition = new TBTPreconditionAND(new CON_IsAbilityNeedTurnTo(), new CON_IsNeedTurnTo());
			abilityTurnMoveToParallelNode.AddChild(new NOD_TurnTo().SetPrecondition(abilityTurnCondition));
			// 技能追逐节点
			TBTPreconditionNOT abilityMoveCondition = new TBTPreconditionNOT(new CON_IsInAbilityRange());
			abilityTurnMoveToParallelNode.AddChild(new NOD_MoveTo().SetPrecondition(abilityMoveCondition));
			abilityTurnMoveToParallelNode.SetEvaluationRelationship(TBTActionParallel.ECHILDREN_RELATIONSHIP.OR);

			// 技能施法节点
			var castAbilitySelectorNode = new TBTActionPrioritizedSelector();
			castAbilitySelectorNode.SetPrecondition(new CON_CanCastAbility());
			castAbilitySelectorNode.AddChild(new NOD_CastAbility());
			castAbilitySelectorNode.AddChild(setUnitIdleNode);

			// 转向、移动、施法
			//var autoCastAbilityNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsAutoCastAbilityRequest());
			//autoCastAbilityNode.AddChild(abilityTurnMoveToParallelNode);
			//autoCastAbilityNode.AddChild(castAbilitySelectorNode);

			// ③手动施放节点构建
			var manualCastAbilityNode = new TBTActionPrioritizedSelector().SetPrecondition(new CON_IsManualCastAbilityRequest());
			manualCastAbilityNode.AddChild(abilityTurnMoveToParallelNode);
			manualCastAbilityNode.AddChild(castAbilitySelectorNode);

			/// 开始构造树
			//m_BehaviorTree.AddChild(chaseSelectorNode);
			//m_BehaviorTreeNode.AddChild(autoCastAbilityNode);
			m_BehaviorTree.AddChild(manualCastAbilityNode);
		}

		return m_BehaviorTree;
	}
}
