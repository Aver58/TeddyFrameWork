#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ModuleManager.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/12 15:14:12
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

public class ModuleManager : Singleton<ModuleManager>
{
	private bool m_bIsInit = false;
	private bool m_bIsRunning = false;

	List<ModuleBase> m_Modules = new List<ModuleBase>();

    public ModuleManager()
    {
        Add<LoadModule>();
    }

	public ModuleBase Get(string name)
	{
		for(int i = 0; i < m_Modules.Count; i++)
		{
			if(m_Modules[i].Name == name)
			{
				return m_Modules[i];
			}
		}
		Debug.LogErrorFormat("[Module获取失败] {0}: 未被注册.", name);
		return null;
	}

	public T Get<T>() where T : ModuleBase
	{
		return Get(typeof(T).Name) as T;
	}

	public void Add<T>() where T : ModuleBase, new()
    {
        T module = new T();
        m_Modules.Add(module);
    }

	public void Init()
	{
		if(m_bIsInit)
			return;

		for(int i = 0; i < m_Modules.Count; ++i)
		{
			m_Modules[i].Init();
		}
	}

	public void UnInit()
	{
		m_bIsRunning = false;
		m_bIsInit = false;
		for(int i = m_Modules.Count - 1; i >= 0; --i)
		{
			m_Modules[i].UnInit();
		}
	}

	public void Update()
	{
		if(!m_bIsInit)
			return;

		// 初始化完就可以走C#层的模块update
		for(int i = 0; i < m_Modules.Count; ++i)
		{
			m_Modules[i].Update(Time.deltaTime);
		}
	}
}