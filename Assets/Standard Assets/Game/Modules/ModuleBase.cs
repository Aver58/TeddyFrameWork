public class ModuleBase : Singleton<ModuleBase>
{
    private string m_Name;
    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(m_Name))
                m_Name = GetType().Name;

            return m_Name;
        }
    }

    protected bool m_bInit = false; //异步初始化辅助字段 会在两段Init中重复使用
    public bool IsInit { get { return m_bInit; } }

    public virtual void Init() { m_bInit = false; }

    // 重启游戏时候调用
    public virtual void UnInit() { m_bInit = false; }

    public virtual void StartGame() { }

    public virtual void Update(float time) { }

    public virtual void Dispose() { }

}
