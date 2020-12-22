#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    Packet.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 13:59:54
=====================================================
*/
#endregion

using System;
using System.Text;

public class Packet
{
    public byte[] Data { get; set; }

    public int Size { get; set; }

    protected int m_index = 0;

    public Packet()
    {
    }

    public Packet(byte[] data, int size)
    {
        Data = data;
        Size = size;
        m_index = 0;
    }

    public void InitIndex(int value)
    {
        m_index = value;
    }

    #region Write

    public bool WriteInt(int value)
    {
        if(Size < m_index + 4 || Data == null)
        {
            return false;
        }
        // todo 打印一下
        Data[m_index] = (byte)((value >> 24) & 0xFF);
        Data[m_index + 1] = (byte)((value >> 16) & 0xFF);
        Data[m_index + 2] = (byte)((value >> 8) & 0xFF);
        Data[m_index + 3] = (byte)(value & 0xFF);
        m_index += 4;
        return true;
    }

    public bool WriteUInt(uint value)
    {
        if(Size < m_index + 4 || Data == null)
        {
            return false;
        }
        Data[m_index] = (byte)((value >> 24) & 0xFF);
        Data[m_index + 1] = (byte)((value >> 16) & 0xFF);
        Data[m_index + 2] = (byte)((value >> 8) & 0xFF);
        Data[m_index + 3] = (byte)(value & 0xFF);
        m_index += 4;
        return true;
    }

    public bool WriteShort(short value)
    {
        if(Size < m_index + 2 || Data == null)
        {
            return false;
        }
        Data[m_index] = (byte)((value >> 8) & 0xFF);
        Data[m_index + 1] = (byte)(value & 0xFF);
        m_index += 2;
        return true;
    }

    public bool WriteBytes(byte[] data)
    {
        if(Size < m_index + data.Length || Data == null)
        {
            return false;
        }
        Array.Copy(data, 0, Data, m_index, data.Length);
        m_index += data.Length;
        return true;
    }

    #endregion

    #region Read

    public int ReadShort()
    {
        if(Size < m_index + 2)
        {
            Debug.LogError("ReadShort index 超出数据长度");
            return -1;
        }
        int value = Data[m_index++] << 8;
        value += Data[m_index++];
        return value;
    }

    public int ReadInt()
    {
        if(Size < m_index + 4)
        {
            Debug.LogError("ReadShort index 超出数据长度");
            return -1;
        }
        int value = Data[m_index++] << 24;
        value += Data[m_index++] << 16;
        value += Data[m_index++] << 8;
        value += Data[m_index++];
        return value;
    }

    #endregion


    public override string ToString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(string.Format("Size:{0}, Length:{1}, Data:", Size, Data.Length));
        for(int i = 0; i < Size; i++)
        {
            Byte b = Data[i];
            stringBuilder.Append(b.ToString("X2"));
        }
        return stringBuilder.ToString();
    }
}