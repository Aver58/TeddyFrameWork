#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ClickListener.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/17 10:34:40
=====================================================
*/
#endregion

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void BaseEventDelegate(Transform trans, BaseEventData eventData);

public class ClickListener : MonoBehaviour, IPointerClickHandler
{
    public BaseEventDelegate onClick;
    public bool canPassClickEvent { get; set; }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click(eventData, canPassClickEvent);
    }

    private void Click(PointerEventData eventData, bool passEvent)
    {
        if(onClick != null)
        {
            onClick(transform, eventData);
        }

        if(canPassClickEvent)
        {
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }
    }

    //把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        bool findCurrent = false;

        for(int i = 0; i < results.Count; i++)
        {
            if(!findCurrent && gameObject == results[i].gameObject)
            {
                findCurrent = true;
                continue;
            }

            if(findCurrent)
            {
                ExecuteEvents.Execute(results[i].gameObject, data, function);
                break;
                //RaycastAll后ugui会自己排序，如果你只想响应透下去的最近的一个响应，这里ExecuteEvents.Execute后直接break就行。
            }
        }

        if(!findCurrent && results.Count > 0)
        {
            ExecuteEvents.Execute(results[0].gameObject, data, function);
        }
    }

    public static T Get<T>(Component cmpt) where T : MonoBehaviour
    {
        GameObject go = cmpt.gameObject;
        T listener = go.GetComponent<T>();
        if(listener == null) 
            listener = go.AddComponent<T>();

        return listener;
    }

    public static void AddClick(Component component, BaseEventDelegate onClick, bool passClickEvent = false, bool posForce = false)
    {
        ClickListener cl = Get<ClickListener>(component);
        cl.onClick += onClick;
        cl.canPassClickEvent = passClickEvent;
    }

    public static void ClearListener(Component component)
    {
        ClickListener cl = Get<ClickListener>(component);
        cl.onClick = null;
    }
}