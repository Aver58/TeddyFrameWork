#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    UIEventArgs.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\13 星期日 23:35:01
=====================================================
*/
#endregion

// todo 改成 T
public class UIEventArgs : EventArgs { }

public class UIEventArgs<T> : UIEventArgs
{
    public readonly T Arg1;

    public UIEventArgs(T param)
    {
        Arg1 = param;
    }

    #region Static Arg1 
    public T Get()
    {
        return Arg1;
    }

    #endregion
}

public class UIEventArgs<T, K> : UIEventArgs
{
    public readonly T Arg1;
    public readonly K Arg2;

    public UIEventArgs(T param, K param2)
    {
        Arg1 = param;
        Arg2 = param2;
    }

    #region Static Arg2
    public T Get()
    {
        return Arg1;
    }

    public K Get2()
    {
        return Arg2;
    }
    #endregion
}