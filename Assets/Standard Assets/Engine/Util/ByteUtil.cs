#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ByteUtil.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/22 14:26:19
=====================================================
*/
#endregion

using System.Text;

public static class ByteUtil
{
	public static string ToHex(this byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach(byte b in bytes)
		{
			stringBuilder.Append(b.ToString("X2"));
		}
		return stringBuilder.ToString();
	}
}