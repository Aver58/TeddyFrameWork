#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Reference.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/9 10:22:17
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 引用计数类
/// </summary>
public class Reference
{
    private List<Object> _requires;
    public int refCount;

	public virtual void Retain()
	{
		refCount++;
	}

	public virtual void Release()
	{
		refCount--;
	}

	/// <summary>
	/// 引用
	/// </summary>
	/// <param name="obj"></param>
	public void Require(Object obj)
	{
		if(_requires == null)
			_requires = new List<Object>();

		_requires.Add(obj);
		Retain();
	}

	/// <summary>
	/// 取消引用
	/// </summary>
	/// <param name="obj"></param>
	public void Dequire(Object obj)
	{
		if(_requires == null)
			return;

		if(_requires.Remove(obj))
			Release();
	}

	/// <summary>
	/// 是否没有引用
	/// </summary>
	/// <returns></returns>
	public bool IsUnused()
	{
		if(_requires != null)
		{
			for(var i = 0; i < _requires.Count; i++)
			{
				var item = _requires[i];
				if(item != null)
					continue;
				Release();
				_requires.RemoveAt(i);
				i--;
			}
			if(_requires.Count == 0)
				_requires = null;
		}
		return refCount <= 0;
	}
}