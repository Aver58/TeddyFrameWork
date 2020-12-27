using UnityEngine;

public class BattleManager : MonoBehaviour
{
	private BattleClient m_BattleClient;
	private BattleLogic m_BattleLogic;
	public BattleClient battleClient;

	private void Awake()
	{
		Init();
		battleClient = m_BattleClient;
	}

	private void Init()
	{
		m_BattleLogic = BattleLogic.instance;
		m_BattleLogic.Init();

		// 剥离出场景的概念 todo
		m_BattleClient = BattleClient.instance;
		m_BattleClient.Init();
	}

	private void GetBattleData()
	{

	}

	private void Update()
	{
		m_BattleLogic.Update();
		m_BattleClient.Update();
	}
}
