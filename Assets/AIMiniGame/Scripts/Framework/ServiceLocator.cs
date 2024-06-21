using System;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : Singleton<ServiceLocator> {
    private Dictionary<Type, object> services = new Dictionary<Type, object>();

    public void RegisterService<T>(T service) {
        var type = typeof(T);
        if (!services.ContainsKey(type)) {
            services[type] = service;
        } else {
            Debug.LogWarning($"Service of type {type} is already registered.");
        }
    }

    public T GetService<T>() {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service)) {
            return (T)service;
        } else {
            Debug.LogError($"Service of type {type} is not registered.");
            return default;
        }
    }

    public void UnregisterService<T>() {
        var type = typeof(T);
        if (services.ContainsKey(type)) {
            services.Remove(type);
        }
    }
}