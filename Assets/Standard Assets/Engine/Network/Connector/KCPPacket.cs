#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    KCPPacket.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 14:22:18
=====================================================
*/
#endregion

//Operation Message Format
//AAAAAABBCCCC
//AAAAAA : 6 bit 
//BB : 2 bit, PacketCMD
//CCCC : data , only used in ConnectResponse, is conv
public class KCPPacket : Packet
{
    public class KOperateStruct
    {
        public KConnCmd cmd;
        public int flowID;
        public uint conv;
        public int content;

        public override string ToString()
        {
            return string.Format("cmd:{0}, flowID:{1}, conv:{2}, content:{3}", cmd, flowID, conv, content);
        }
    }

    public KCPPacket(byte[] data, int size) : base(data, size)
    {
    }

    // 解析操作包
    public KOperateStruct ParseOperatePkg()
    {
        int headSize = this.ReadShort();
        if(headSize + 2 != this.Size)
        {
            Debug.Log("包长验证失败,headSize:{0}, pktSize:{1}", headSize, Size);
            return null;
        }
        KOperateStruct optInfo = new KOperateStruct();
        optInfo.cmd = (KConnCmd)this.ReadShort();
        optInfo.flowID = this.ReadShort();
        optInfo.conv = (uint)this.ReadInt();
        optInfo.content = this.ReadInt();
        Debug.Log("ParseOperatePkg,info:" + optInfo.ToString());
        return optInfo;
    }
}