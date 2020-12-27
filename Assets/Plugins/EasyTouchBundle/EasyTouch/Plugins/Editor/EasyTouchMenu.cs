using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.EventSystems;

public static class EasyTouchMenu{

	[MenuItem ("GameObject/EasyTouch/Add EasyTouch", false, 0)]
	static void  AddEasyTouch(){

		// EasyTouch
		if (GameObject.FindObjectOfType<EasyTouch>()==null){
			new GameObject("EasyTouch", typeof(EasyTouch));
		}
		else{
			EditorUtility.DisplayDialog("Warning","EasyTouch is already exist in your scene","OK");
		}

		Selection.activeObject = GameObject.FindObjectOfType<EasyTouch>().gameObject;
	}
	
}

