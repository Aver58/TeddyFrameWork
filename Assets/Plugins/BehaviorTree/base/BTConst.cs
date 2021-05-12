#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    BTDefine.cs
 Author:      Zeng Zhiwei
 Time:        2021/5/11 16:14:33
=====================================================
*/
#endregion


namespace Aver3
{
    public enum BTResult
    {
        Ready,
        Running,
        Finished,
    }

    public enum BTActionStatus
    {
        Ready,
        Running,
        Finished,
    }

    public static class BTConst
    {
        public static bool ENABLE_BTACTION_LOG = true;
    }
}
