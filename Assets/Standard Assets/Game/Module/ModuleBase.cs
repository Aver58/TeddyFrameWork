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

public class ModuleBase {
    private string name;
    public string Name {
        get {
            if(string.IsNullOrEmpty(name))
                name = GetType().Name;
            return name;
        }
    }

    protected bool isInit = false;
    public bool IsInit { get { return isInit; } }

    public virtual void Init() { isInit = true; }
    public virtual void UnInit() { isInit = false; }
    public virtual void StartGame() { }
    public virtual void Update(float dt) { }
    public virtual void Dispose() { }
}