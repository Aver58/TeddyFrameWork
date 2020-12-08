#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    NetMsgData.cs
 Author:      Zeng Zhiwei
 Time:        2019/12/9 14:31:27
=====================================================
*/
#endregion

using ProtoBuf;

[ProtoContract]
public class NetMsgData
{
    [ProtoMember(1)]
    public ushort ID { get; set; }
    [ProtoMember(2)]
    public string Data { get; set; }

    public NetMsgData(ushort id, string data)
    {
        ID = id;
        Data = data;
    }
}

