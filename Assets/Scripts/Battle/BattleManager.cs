using UnityEngine;

public class BattleManager : MonoBehaviour
{
	private BattleLogic m_BattleLogic;

	private void Awake()
	{
		Init();
	}

	private void Init()
	{
		m_BattleLogic = BattleLogic.instance;
		m_BattleLogic.Init();
	}

	private void Update()
	{
		m_BattleLogic.Update();
	}
}
