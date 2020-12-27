// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com

using UnityEngine;
using UnityEditor;
using System;

[InitializeOnLoad]
public class EasytouchHierachyCallBack{
	
	private static readonly EditorApplication.HierarchyWindowItemCallback hiearchyItemCallback;
	private static Texture2D hierarchyIcon;
	private static Texture2D HierarchyIcon {
		get {
			if (EasytouchHierachyCallBack.hierarchyIcon==null){
				EasytouchHierachyCallBack.hierarchyIcon = (Texture2D)Resources.Load( "EasyTouch_Icon");
			}
			return EasytouchHierachyCallBack.hierarchyIcon;
		}
	}	
	// constructor
	static EasytouchHierachyCallBack()
	{
//		EasytouchHierachyCallBack.hiearchyItemCallback = new EditorApplication.HierarchyWindowItemCallback(EasytouchHierachyCallBack.DrawHierarchyIcon);
//		EditorApplication.hierarchyWindowItemOnGUI = (EditorApplication.HierarchyWindowItemCallback)Delegate.Combine(EditorApplication.hierarchyWindowItemOnGUI, EasytouchHierachyCallBack.hiearchyItemCallback);
		
	}
	
	private static void DrawHierarchyIcon(int instanceID, Rect selectionRect)
	{
		GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if (gameObject != null && gameObject.GetComponent<EasyTouch>() != null)
		{
			Rect rect = new Rect(selectionRect.x + selectionRect.width - 16f, selectionRect.y, 16f, 16f);
			GUI.DrawTexture( rect,EasytouchHierachyCallBack.HierarchyIcon);
		}
	}
		
}
