// 共享行为树
//http://www.aisharing.com/archives/750
using TsiU;
using Aver3;

public class BehaviorTreeFactory : Singleton<BehaviorTreeFactory>
{
	private static TBTActionPrioritizedSelector m_DecisionTree;
	private static TBTActionPrioritizedSelector m_BehaviorTree;
	private static BTPrioritySelector m_mobaBehaviorTree;

	public TBTActionPrioritizedSelector GetDecisionTree()
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
	private BTPrioritySelector GetChaseNode()
	{
		var nodeChase = new BTPrioritySelector();
		nodeChase.SetPrecondition(new CON_IsChaseRequest());

		var nodeTurnAndMove = new BTParallel(ParallelFunction.Or);
		var condition = new BTPreconditionOR(new CON_IsNeedTurnTo(), new CON_IsNeedMoveTo());
		nodeTurnAndMove.SetPrecondition(condition);//todo 是否可以转向 CON_CanTurnTo //todo 是否可以移动 CON_CanMoveTo

		nodeTurnAndMove.AddChild(new NOD_TurnTo());// 旋转
		nodeTurnAndMove.AddChild(new NOD_MoveTo());// 移动

		nodeChase.AddChild(nodeTurnAndMove);
		nodeChase.AddChild(new NOD_CompleteRequest());

		return nodeChase;
	}

	// 释放技能节点
	public BTPrioritySelector GetSkillNode()
	{
		var nodeSkill = new BTPrioritySelector();
		nodeSkill.SetPrecondition(new CON_IsManualCastAbilityRequest());

		var nodeCompleteRequest = new NOD_CompleteRequest();

		// 1. 不在视野范围内，播放技能动画，不释放技能
		var nodeOutOfView = new BTPrioritySelector();
		nodeOutOfView.SetPrecondition(new BTPreconditionNOT(new CON_IsInViewRange()));
		var nodeCastAbilityAnimation = new NOD_CastAbilityAnimation();
		nodeOutOfView.AddChild(nodeCastAbilityAnimation);
		nodeOutOfView.AddChild(nodeCompleteRequest);

		// 2. 视野范围内，不在技能范围内，追逐，释放技能
		var nodeInView = new BTPrioritySelector();
		nodeInView.SetPrecondition(new BTPreconditionNOT(new CON_IsInAbilityRange()));
		var nodeTurnAndMove = new BTParallel(ParallelFunction.Or);
		var condition = new BTPreconditionOR(new CON_IsAbilityNeedTurnTo(), new CON_IsNeedMoveTo());
		nodeTurnAndMove.SetPrecondition(condition);//todo 是否可以转向 CON_CanTurnTo //todo 是否可以移动 CON_CanMoveTo

		nodeTurnAndMove.AddChild(new NOD_TurnTo());// 旋转
		nodeTurnAndMove.AddChild(new NOD_MoveTo());// 移动

		nodeInView.AddChild(nodeTurnAndMove);
		nodeInView.AddChild(new NOD_CastAbility());
		nodeInView.AddChild(nodeCompleteRequest);

		// 3. 在技能范围内，直接释放技能
		var nodeInAbilityRange = new BTPrioritySelector();
		nodeInAbilityRange.AddChild(new NOD_CastAbility());
		nodeInAbilityRange.AddChild(nodeCompleteRequest);

		nodeSkill.AddChild(nodeOutOfView);
		nodeSkill.AddChild(nodeInView);
		nodeSkill.AddChild(nodeInAbilityRange);

		return nodeSkill;
	}

	public BTAction GetMobaBehaviorTree()
	{
		if(m_mobaBehaviorTree == null)
		{
			m_mobaBehaviorTree = new BTPrioritySelector();
			m_mobaBehaviorTree.AddChild(GetChaseNode());
			m_mobaBehaviorTree.AddChild(GetSkillNode());
		}
		return m_mobaBehaviorTree;
	}
}
