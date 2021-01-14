#region Copyright © 2020 Aver. All rights reserved.

/*
=====================================================
 AverFrameWork v1.0
 Filename:    ExportPanelHierarchy.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 15:05:29
=====================================================
*/

#endregion

using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class ExportPanelHierarchy
{
    public static void ExportNested(GameObject root)
    {
        var path = GetPath(root);
        var name = Path.GetFileNameWithoutExtension(path);
        var ctx = @"using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class {0} : MainViewBase
{{{1}
    public GameObject[] Extends;
}}";
        var items = root.GetComponentsInChildren<UIExportItem>(true);

        var field = "";
        var compEnum = typeof(UIComponentEnum);
        foreach (var item in items)
        {
            field += $"\n    public {compEnum.GetEnumName(item.Type)} {item.name};";
        }

        GameObject[] array = null;
        if (File.Exists(path))
        {
            var compType = Assembly.Load("Assembly-CSharp").GetType(name);
            var oldComp = root.GetComponent(compType);
            var fieldInfo = compType.GetField("Extends", BindingFlags.Instance | BindingFlags.Public);
            var extends = fieldInfo.GetValue(oldComp);
            if (extends != null)
            {
                array = (GameObject[])extends;
                foreach (var go in array)
                {
                    field += $"\n    public GameObject {go.name};";
                }
            }
        }

        ctx = string.Format(ctx, name, field).Replace("\r", string.Empty);
        if (File.Exists(path))
        {
            var old = File.ReadAllText(path);
            if (old == ctx)
            {
                return;
            }

            var extendsTemp = root.AddComponent<ComponentExtendsTemp>();
            extendsTemp.Extends = array;
            ProcessUIPrefab(root);
        }

        File.WriteAllText(path, ctx);

        EditorPrefs.SetBool("ScriptGenerator", true);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static string GetPath(GameObject root)
    {
        var name = root.name;
        name = name.Substring(0, name.Length - 5);
        name = $"{Path.GetDirectoryName(Application.dataPath)}/Assets/Scripts/Game/View/Generate/{name}View.cs";
        return name;
    }

    /// <summary>
    /// 检查如果是UI的prefab进行一些处理
    /// </summary>
    static void ProcessUIPrefab(GameObject instance)
    {
        string prefabPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(instance));
        PrefabUtility.SaveAsPrefabAssetAndConnect(instance, prefabPath, InteractionMode.AutomatedAction);
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        if (!EditorPrefs.GetBool("ScriptGenerator"))
        {
            return;
        }

        EditorPrefs.SetBool("ScriptGenerator", false);

        AssetDatabase.Refresh();
        GameObject root = Selection.activeGameObject as GameObject;
        if (root)
        {
            var path = GetPath(root);
            var name = Path.GetFileNameWithoutExtension(path);
            AddScript(root, name);
        }
    }

    private static void AddScript(GameObject root, string name)
    {
        var compType = Assembly.Load("Assembly-CSharp").GetType(name);
        if (compType == null)
        {
            return;
        }

        var comp = root.GetComponent(compType) ?? root.AddComponent(compType);
        var items = root.GetComponentsInChildren<UIExportItem>(true);
        foreach (var item in items)
        {
            var itemType = UIComponentType.TypeArray[(int)item.Type];
            Object fieldItem = item.GetComponent(itemType);
            if (fieldItem != null)
            {
                var fieldInfo = compType.GetField(fieldItem.name, BindingFlags.Instance | BindingFlags.Public);
                fieldInfo.SetValue(comp, fieldItem);
            }
        }

        var temp = root.GetComponent<ComponentExtendsTemp>();
        if (temp)
        {
            var fieldInfo = compType.GetField("Extends", BindingFlags.Instance | BindingFlags.Public);
            fieldInfo.SetValue(comp, temp.Extends);

            foreach (var go in temp.Extends)
            {
                fieldInfo = compType.GetField(go.name, BindingFlags.Instance | BindingFlags.Public);
                fieldInfo.SetValue(comp, go);
            }

            Object.DestroyImmediate(temp);
        }

        ProcessUIPrefab(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}