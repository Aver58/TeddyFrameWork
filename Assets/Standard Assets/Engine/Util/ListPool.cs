using System.Collections.Generic;

public static class ListPool<T>
{
    static readonly ObjectPool<List<T>> s_listPool = new ObjectPool<List<T>>(null, l => l.Clear());

    public static List<T> Get()
    {
        return s_listPool.Get();
    }

    public static void Release(List<T> toRelease)
    {
        s_listPool.Release(toRelease);
    }
}