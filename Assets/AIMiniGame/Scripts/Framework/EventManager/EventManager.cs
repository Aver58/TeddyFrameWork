using System;
using System.Collections.Generic;

public class EventManager : Singleton<EventManager> {
    private Dictionary<int, Delegate> eventDictionary = new Dictionary<int, Delegate>();

    private void Register(int eventId, Delegate listener) {
        if (eventDictionary.TryGetValue(eventId, out var del)) {
            eventDictionary[eventId] = Delegate.Combine(del, listener);
        } else {
            eventDictionary[eventId] = listener;
        }
    }

    private void Unregister(int eventId, Delegate listener) {
        if (eventDictionary.TryGetValue(eventId, out var del)) {
            var currentDel = Delegate.Remove(del, listener);
            if (currentDel == null) {
                eventDictionary.Remove(eventId);
            } else {
                eventDictionary[eventId] = currentDel;
            }
        }
    }

    private void Trigger(int eventId, params object[] parameters) {
        if (eventDictionary.TryGetValue(eventId, out var del)) {
            del.DynamicInvoke(parameters);
        }
    }

    public void Register(int eventId, Action listener) => Register(eventId, (Delegate)listener);
    public void Register<T>(int eventId, Action<T> listener) => Register(eventId, (Delegate)listener);
    public void Register<T, U>(int eventId, Action<T, U> listener) => Register(eventId, (Delegate)listener);
    public void Register<T, U, V>(int eventId, Action<T, U, V> listener) => Register(eventId, (Delegate)listener);
    public void Register<T, U, V, W>(int eventId, Action<T, U, V, W> listener) => Register(eventId, (Delegate)listener);

    public void Unregister(int eventId, Action listener) => Unregister(eventId, (Delegate)listener);
    public void Unregister<T>(int eventId, Action<T> listener) => Unregister(eventId, (Delegate)listener);
    public void Unregister<T, U>(int eventId, Action<T, U> listener) => Unregister(eventId, (Delegate)listener);
    public void Unregister<T, U, V>(int eventId, Action<T, U, V> listener) => Unregister(eventId, (Delegate)listener);
    public void Unregister<T, U, V, W>(int eventId, Action<T, U, V, W> listener) => Unregister(eventId, (Delegate)listener);

    // 触发事件
    public void Trigger(int eventId) {
        if (eventDictionary.TryGetValue(eventId, out var value)) {
            Action callback = value as Action;
            callback?.Invoke();
        }
    }

    public void Trigger<T>(int eventId, T param) {
        if (eventDictionary.TryGetValue(eventId, out var value)) {
            Action<T> callback = value as Action<T>;
            callback?.Invoke(param);
        }
    }

    public void Trigger<T1, T2>(int eventId, T1 param1, T2 param2) {
        if (eventDictionary.TryGetValue(eventId, out var value)) {
            Action<T1, T2> callback = value as Action<T1, T2>;
            callback?.Invoke(param1, param2);
        }
    }

    public void Trigger<T1, T2, T3>(int eventId, T1 param1, T2 param2, T3 param3) {
        if (eventDictionary.TryGetValue(eventId, out var value)) {
            Action<T1, T2, T3> callback = value as Action<T1, T2, T3>;
            callback?.Invoke(param1, param2, param3);
        }
    }

    public void Trigger<T1, T2, T3, T4>(int eventId, T1 param1, T2 param2, T3 param3, T4 param4) {
        if (eventDictionary.TryGetValue(eventId, out var value)) {
            Action<T1, T2, T3, T4> callback = value as Action<T1, T2, T3, T4>;
            callback?.Invoke(param1, param2, param3, param4);
        }
    }
}