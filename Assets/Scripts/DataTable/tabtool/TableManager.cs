﻿using System.Collections.Generic;

public abstract class TableManager<T, U> : SingletonTable<U>
{
    protected Dictionary<int, T> m_Items = new Dictionary<int, T>();

    public Dictionary<int, T> GetTable()
    {
        return m_Items;
    }

    public T GetTableItem(int key)
    {
        T t;
        if(m_Items.TryGetValue(key, out t))
        {
            return t;
        }
        return default(T);
    }

    public abstract bool Load();
}