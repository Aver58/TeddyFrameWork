using System;
using UnityEngine;

namespace TrueSync
{
    // ֡ͬ�������Ϣ
	[Serializable]
	public class TSPlayerInfo
	{
		[SerializeField]
		internal byte id;

		[SerializeField]
		internal string name;

		public byte Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public TSPlayerInfo(byte id, string name)
		{
			this.id = id;
			this.name = name;
		}
	}
}
