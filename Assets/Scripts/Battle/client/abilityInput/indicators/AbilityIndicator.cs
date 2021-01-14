#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AbilityIndicator.cs
 Author:      Zeng Zhiwei
 Time:        2021/1/12 10:03:17
=====================================================
*/
#endregion

using UnityEngine;

/// <summary>
/// 技能指示器抽象类
/// </summary>
public abstract class AbilityIndicator
{
    protected Transform m_indicatorTransform;
    protected Transform m_casterTransform;
    protected Vector3 m_position = Vector3.zero;

    #region API
    public AbilityIndicator(Transform indicatorTransform, Transform casterTransform = null)
    {
        m_indicatorTransform = indicatorTransform;
        m_casterTransform = casterTransform;
    }
    public virtual void Update() { }
    public virtual void Show() { }
    public virtual void Hide() { }
    #endregion

    #region Private
    protected virtual void OnUpdate(float casterX, float casterZ, float targetX, float targetZ, float targetForwardX, float targetForwardZ) { }
    protected virtual void OnShow() 
    {
        m_indicatorTransform.gameObject.SetActive(true);
    }

    protected virtual void OnHide() 
    { 
        m_indicatorTransform.gameObject.SetActive(false);
    }

    #endregion
}