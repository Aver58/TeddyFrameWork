#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    OpCode.cs
 Author:      Zeng Zhiwei
 Time:        2019/12/8 20:22:55
=====================================================
*/
#endregion

namespace Network
{
    public class OpCode
    {
        public const ushort S2C_Connect_Success = 100;
        public const ushort C2S_TestRequest = 101;
        public const ushort S2C_TestResponse = 102;
    }
}