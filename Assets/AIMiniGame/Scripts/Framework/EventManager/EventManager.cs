using System;
using System.Collections.Generic;

/// <summary>
/// 观察者模式
/// </summary>
public class EventManager : MonoSingleton<EventManager> {
    private Dictionary<int, List<Delegate>> eventMap = new Dictionary<int, List<Delegate>>();

    private void Register(int id, Delegate handler) {
        if (Instance == null) {
            return;
        }

        if (eventMap.TryGetValue(id, out var listeners)) {
            eventMap[id].Add(handler);
        } else {
            eventMap[id] = new List<Delegate> { handler };
        }
    }

    private void Unregister(int id, Delegate handler) {
        if (Instance == null) {
            return;
        }

        if (eventMap.TryGetValue(id, out var handlers)) {
            for (int i = 0; i < handlers.Count; i++) {
                var currentDel = handlers[i];
                if (currentDel == handler) {
                    handlers.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void Register(int id, Action handler) => Register(id, (Delegate)handler);
    public void Register<T1>(int id, Action<T1> handler) => Register(id, (Delegate)handler);
    public void Register<T1, T2>(int id, Action<T1, T2> handler) => Register(id, (Delegate)handler);
    public void Register<T1, T2, T3>(int id, Action<T1, T2, T3> handler) => Register(id, (Delegate)handler);
    public void Register<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> handler) => Register(id, (Delegate)handler);

    public void Unregister(int id, Action handler) => Unregister(id, (Delegate)handler);
    public void Unregister<T1>(int id, Action<T1> handler) => Unregister(id, (Delegate)handler);
    public void Unregister<T1, T2>(int id, Action<T1, T2> handler) => Unregister(id, (Delegate)handler);
    public void Unregister<T1, T2, T3>(int id, Action<T1, T2, T3> handler) => Unregister(id, (Delegate)handler);
    public void Unregister<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> listener) => Unregister(id, (Delegate)listener);

    public void Dispatch(int id) {
        if (eventMap.TryGetValue(id, out var listeners)) {
            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                listener.DynamicInvoke();
            }
        }
    }

    public void Dispatch<T1>(int id, T1 param) {
        if (eventMap.TryGetValue(id, out var listeners)) {
            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                listener.DynamicInvoke(param);
            }
        }
    }

    public void Dispatch<T1, T2>(int id, T1 param1, T2 param2) {
        if (eventMap.TryGetValue(id, out var listeners)) {
            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                listener.DynamicInvoke(param1, param2);
            }
        }
    }

    public void Dispatch<T1, T2, T3>(int id, T1 param1, T2 param2, T3 param3) {
        if (eventMap.TryGetValue(id, out var listeners)) {
            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                listener.DynamicInvoke(param1, param2, param3);
            }
        }
    }

    public void Dispatch<T1, T2, T3, T4>(int id, T1 param1, T2 param2, T3 param3, T4 param4) {
        if (eventMap.TryGetValue(id, out var listeners)) {
            for (int i = 0; i < listeners.Count; i++) {
                var listener = listeners[i];
                listener.DynamicInvoke(param1, param2, param3, param4);
            }
        }
    }
}