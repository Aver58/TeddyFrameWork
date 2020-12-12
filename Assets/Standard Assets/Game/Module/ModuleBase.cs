#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ModuleBase.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/12 11:28:38
=====================================================
*/
#endregion

public class ModuleBase
{
    public ModuleBase() { }

    private string m_Name;
    public string Name
    {
        get
        {
            if(string.IsNullOrEmpty(m_Name))
                m_Name = GetType().Name;
            return m_Name;
        }
    }
    protected bool m_bInit = false;
    public bool IsInit { get { return m_bInit; } }

    public virtual void Init() { m_bInit = true; }
    public virtual void UnInit() { m_bInit = false; }
    public virtual void StartGame() { }
    public virtual void Update(float dt) { }
    public virtual void Dispose() { }
}