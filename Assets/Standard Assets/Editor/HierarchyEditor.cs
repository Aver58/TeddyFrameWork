using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[InitializeOnLoad]
public class HierarchyEditor
{
	static GameObject[] m_selectionObjs;
	static Dictionary<GameObject, Rect> m_GO2RectMap = new Dictionary<GameObject, Rect> ();

    static HierarchyEditor()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
		Selection.selectionChanged += OnSelectionChanged;
    }

    static void HierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        CustomItem(selectionRect, obj);
    }

    /// <summary>
    /// 自定义显示
    /// </summary>
    /// <param name="selectionRect"></param>
    /// <param name="obj"></param>
    static void CustomItem(Rect selectionRect, Object obj)
    {
        if (obj is GameObject)
        {
            GameObject go = obj as GameObject;

			if (!m_GO2RectMap.ContainsKey (go)) {
				m_GO2RectMap.Add (go, selectionRect);
			}

            IsExportItem(go, selectionRect);
            ShowOrHideGameObject(go, selectionRect);
            CalculateChildNode(go, selectionRect);
        }
    }

    /// <summary>
    /// 显示或者隐藏
    /// </summary>
    /// <param name="go"></param>
    /// <param name="selectionRect"></param>
    static void ShowOrHideGameObject(GameObject go, Rect selectionRect)
    {
		Rect r = new Rect(selectionRect);
		r.x += r.width - 30f;

		bool isActive = GUI.Toggle(r, go.activeSelf, "");

		if (isActive != go.activeSelf) {
			if (m_selectionObjs != null && m_selectionObjs.Length > 1) {
				for (int i = 0; i < m_selectionObjs.Length; i++) {
					if (m_selectionObjs[i] != go) {
						m_selectionObjs [i].SetActive(isActive);
					}
				}
			}
		}

		go.SetActive(isActive);
    }

    static void IsExportItem(GameObject go, Rect selectionRect)
    {
        Rect r = new Rect(selectionRect);
        r.x += r.width - 55f;
        r.width = 25f;

        UIHierarchy uiHierarchy = go.GetComponent<UIHierarchy>();
        if (uiHierarchy)return;

        UIExportItem uiExportItem = go.GetComponent<UIExportItem>();
        if (uiExportItem)
        {
            if (GUI.Button(r, "-"))
            {
                GameObject.DestroyImmediate(uiExportItem);
            }
        }
        else
        {
            if (GUI.Button(r, "+"))
            {
                go.AddComponent<UIExportItem>();
            }
        }
    }

    /// <summary>
    /// 计算子节点数量
    /// </summary>
    /// <param name="go"></param>
    /// <param name="selectionRect"></param>
    static void CalculateChildNode(GameObject go, Rect selectionRect)
    {
        Transform trans = go.transform;
        Rect r = new Rect(selectionRect);
        r.x += r.width - 15f;

        int childCount = 0;
        Transform[] childs = trans.GetComponentsInChildren<Transform>(true);

        childCount = childs.Length - 1;
        TextAnchor ta = GUI.skin.label.alignment;
        GUI.skin.label.alignment = TextAnchor.MiddleRight;
        GUI.Label(r, childCount == 0 ? "" : childCount.ToString());
        GUI.skin.label.alignment = ta;
    }

	static void OnSelectionChanged(){
		m_selectionObjs = Selection.gameObjects;
	}
}
