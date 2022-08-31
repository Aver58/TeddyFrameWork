using System.Collections.Generic;
using UnityEngine;

public abstract class TableManager<T, U> : SingletonTable<U>
{
    protected readonly Dictionary<int, T> m_Items = new Dictionary<int, T>();
    public T Get(int id) {
        if (m_Items.TryGetValue(id, out var item)) {
            return item;
        }
        else {
            Debug.LogError($" {typeof(T).Name} 没有找到指定id的配置，id：{id}");
            return default;
        }
    }

    public abstract bool Load();

    public Dictionary<int, T> GetTable() {
        return m_Items;
    }
}