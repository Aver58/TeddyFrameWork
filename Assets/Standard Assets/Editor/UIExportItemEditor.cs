using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIExportItemEditor
{
    [MenuItem("GameObject/Add UIExportItem", false, 32)]
    static void AddExportItem()
    {
        GameObject root = Selection.activeGameObject as GameObject;
        if (root)
        {
            var item = root.AddComponent<UIExportItem>();
            var end = UIComponentType.MAX_NUM;
            for (int i = 0; i < end; i++)
            {
                var t = UIComponentType.TypeArray[i];
                var comp = root.GetComponent(t);
                if (comp)
                {
                    item.Type = (UIComponentEnum)i;
                    break;
                }
            }
        }
    }
}
