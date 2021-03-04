#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    EditorHelper.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 15:03:42
=====================================================
*/
#endregion

using UnityEditor;
using UnityEngine;

public class EditorHelper
{
	//批量选中Assets目录下的对象的处理
	public static void ExportSelection(string name, System.Action<Object> onAction, SelectionMode mode = SelectionMode.DeepAssets)
	{
		Object[] items = Selection.GetFiltered(typeof(Object), mode);
		int total = items.Length;
		for(int i = 0; i < total; ++i)
		{
			if(onAction != null)
			{
				onAction(items[i]);
			}
			UpdateProgress("name", i + 1, total);
		}
		CloseProgress();

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		GameLog.Log(string.Format("{0} Completed", name));
	}

	public static void UpdateProgress(string info, int current, int total)
	{
		EditorUtility.DisplayProgressBar("Generate...", string.Format("{0} {1}/{2}", info, current, total), Mathf.InverseLerp(0, total, current));
	}

	public static void CloseProgress()
	{
		EditorUtility.ClearProgressBar();
	}
}