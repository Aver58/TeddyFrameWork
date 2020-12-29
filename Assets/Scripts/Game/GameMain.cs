using System.Collections;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static System.DateTime appStartTime;
    public float FpsInterval = 0.5f;
    public int CurrFps = 0;
    public Config config;   // 游戏配置
    public static GameMain instance { get; private set; }
    ModuleManager m_moduleMgr = null;

    // Start is called before the first frame update
    private void Start()
    {
		instance = this;
        Screen.sleepTimeout = (int)SleepTimeout.NeverSleep;
        Application.targetFrameRate = 30;
        DontDestroyOnLoad(gameObject);
        config.Init();

        m_moduleMgr = ModuleManager.instance;

        AddModules();

        StartCoroutine(InitFramework());
    }

    private IEnumerator InitFramework()
    {
        m_moduleMgr.Init();

        while(!m_moduleMgr.IsAllInit())
            yield return null;

        StartGame();
    }

    void StartGame()
    {
        Debug.Log("---StartGame---");
        m_moduleMgr.StartGame();

        //UIModule.OpenView(ViewID.Test);
        SceneModule.ChangeScene(SceneID.Moba);
    }

    private void AddModules()
    {
        m_moduleMgr.Add<UIModule>();
    }
     
    private void Update()
    {
        m_moduleMgr.Update();
    }
}
