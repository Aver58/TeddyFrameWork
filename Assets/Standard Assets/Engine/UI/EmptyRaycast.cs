#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    EmptyRaycast.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/16 21:17:03
=====================================================
*/
#endregion

using UnityEngine.UI;

public class EmptyRaycast : MaskableGraphic
{
	protected EmptyRaycast()
	{
		useLegacyMeshGeneration = false;
	}

	protected override void OnPopulateMesh(VertexHelper toFill)
	{
		toFill.Clear();
	}
}