using System;

public class SingletonTable<T>
{
    protected static readonly T ms_instance = Activator.CreateInstance<T>();
    public static T Instance { get { return ms_instance; } }
    protected SingletonTable() { }
}
