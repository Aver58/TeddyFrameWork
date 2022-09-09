#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ObjectPool.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\22 星期二 20:10:48
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine.Events;

public class ObjectPool<T> where T : new() {
	private readonly Stack<T> m_Stack = new Stack<T>();
	private readonly UnityAction<T> m_ActionOnGet;
	private readonly UnityAction<T> m_ActionOnRelease;

	public int countAll { get; private set; }
	public int countActive { get { return countAll - countInactive; } }
	public int countInactive { get { return m_Stack.Count; } }

	public ObjectPool(UnityAction<T> actionOnGet = null, UnityAction<T> actionOnRelease = null) {
		m_ActionOnGet = actionOnGet;
		m_ActionOnRelease = actionOnRelease;
	}

	public T Get() {
		T element;
		if(m_Stack.Count == 0) {
			element = new T();
			countAll++;
		} else {
			element = m_Stack.Pop();
		}

		if (m_ActionOnGet != null) {
			m_ActionOnGet(element);
		}
		
		return element;
	}

	public void Release(T element) {
		if(m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
			GameLog.LogError("Internal error. Trying to destroy object that is already released to pool.");
		if(m_ActionOnRelease != null)
			m_ActionOnRelease(element);
		m_Stack.Push(element);
	}

	public void PrintInfo() {
		GameLog.Log("当前池子中对象数量：{0}", m_Stack.Count);
	}
}
