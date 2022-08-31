using System;

public class SingletonTable<T>
{
    protected static readonly T instance = Activator.CreateInstance<T>();
    public static T Instance { get { return instance; } }
    protected SingletonTable() { }
}
