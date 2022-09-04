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
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExportPanelHierarchy
{
    private const string ViewStr = "View";
    private const string PanelStr = "Panel";
    private const string UI_EXPORT_CS_VIEW_PATH = "Assets/Scripts/Game/View/UIBindView/";

    private static System.Type[] ms_componentTypes =
    {
        typeof(ETCJoystick),
        typeof(Button),
        typeof(Image),
        typeof(ImageEx),
        typeof(Text),
        typeof(TMPro.TextMeshProUGUI),
    };

    public static void ExportUIView()
    {
        var uiObj = Selection.activeObject;
        if(null == uiObj)
            return;
        var uiViewName = uiObj.name;
        if(uiViewName.EndsWith(PanelStr))
            uiViewName = uiViewName.Replace(PanelStr, ViewStr);

        var hierarchy = ExportNested(uiObj);

        GenUIViewCode(uiViewName, hierarchy);

        EditorUtility.SetDirty(uiObj);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static UIHierarchy ExportNested(Object obj)
    {
        GameObject root = obj as GameObject;
        if(root == null) return null;

        UIHierarchy hierarchy = root.GetComponent<UIHierarchy>();
        if(hierarchy == null)
        {
            hierarchy = root.AddComponent<UIHierarchy>();
        }

        //生成根节点层级
        List<UIHierarchy.EffectItemInfo> fxFields = new List<UIHierarchy.EffectItemInfo>();
        List<UIHierarchy.ItemInfo> fields = new List<UIHierarchy.ItemInfo>();
        GetChildComponentUtilHierarchy(root.transform, fxFields, fields);
        hierarchy.SetEffects(fxFields);
        hierarchy.SetWidgets(fields);

        //生成子panel层级
        UIHierarchy[] childHierarchys = root.GetComponentsInChildren<UIHierarchy>(true);
        for(int i = 1; i < childHierarchys.Length; i++)
        {
            UIHierarchy childHrcy = childHierarchys[i];

            List<UIHierarchy.EffectItemInfo> childFx = new List<UIHierarchy.EffectItemInfo>();
            List<UIHierarchy.ItemInfo> childUIItem = new List<UIHierarchy.ItemInfo>();
            GetChildComponentUtilHierarchy(childHrcy.transform, childFx, childUIItem);
            childHrcy.SetEffects(childFx);
            childHrcy.SetWidgets(childUIItem);
        }

        return hierarchy;
    }

    //导出传入节点的层级，直到某个子节点挂有UIHierarchy组件
    private static void GetChildComponentUtilHierarchy(Transform transRoot, List<UIHierarchy.EffectItemInfo> fxFields, List<UIHierarchy.ItemInfo> fields)
    {
        for(int i = 0; i < transRoot.childCount; i++)
        {
            Transform trans = transRoot.GetChild(i);

            UIHierarchy hrchy = trans.GetComponent<UIHierarchy>();
            if(hrchy != null)
            {
                fields.Add(new UIHierarchy.ItemInfo(hrchy.name, hrchy));
                continue;
            }

            //FxExportItem fxItem = trans.GetComponent<FxExportItem>();
            //if(fxItem != null)
            //{
            //    Object fieldItem = GetChildComponent(fxItem.gameObject) ?? fxItem.transform;
            //    fxFields.Add(new UIHierarchy.EffectItemInfo(fxItem.name, fieldItem, fxItem.transform.parent));
            //}
            //else
            //{
                UIExportItem uiItem = trans.GetComponent<UIExportItem>();
                if(uiItem != null)
                {
                    Object fieldItem = GetChildComponent(uiItem.gameObject);
                    if(fieldItem == null)
                    {
                        fieldItem = uiItem.transform;
                    }
                    fields.Add(new UIHierarchy.ItemInfo(uiItem.name, fieldItem));
                }
            //}
            GetChildComponentUtilHierarchy(trans, fxFields, fields);
        }
    }

    private static Object GetChildComponent(GameObject go)
    {
        Object component = null;
        for(int i = 0; i < ms_componentTypes.Length; ++i)
        {
            component = go.GetComponent(ms_componentTypes[i]);
            if(component != null)
                break;
        }
        return component;
    }

    private static void GenUIViewCode(string uiViewName, UIHierarchy uIHierarchy)
    {
        var codePath = UI_EXPORT_CS_VIEW_PATH + uiViewName + ".deginer.cs";

        StringBuilder sb = new StringBuilder(16);
        sb.Append("///[[Notice:This file is auto generate by ExportPanelHierarchy，don't modify it manually! it will be call OnLoaded--]]\n\n");
        sb.Append("public partial class " + uiViewName + "\n{\n");

        if(uIHierarchy.widgets.Count > 0)
        {
            sb.Append("\t//widgets\n");
            foreach(var itemInfo in uIHierarchy.widgets)
            {
                var typeName = itemInfo.item.GetType().ToString();
                sb.Append("\tprivate ").Append(typeName).Append(" ").Append(itemInfo.name).Append(";\n");
            }
        }

        if(uIHierarchy.externals.Count > 0)
        {
            sb.Append("\t//externals\n");
            foreach(var itemInfo in uIHierarchy.externals)
            {
                var typeName = itemInfo.item.GetType().ToString();
                sb.Append("\tprivate ").Append(typeName).Append(" ").Append(itemInfo.name).Append(";\n");
            }
        }

        sb.Append("\tprotected override void BindView()\n\t{\n\t\tvar UIHierarchy = this.transform.GetComponent<UIHierarchy>();\n");

        if(uIHierarchy.externals.Count > 0)
        {
            sb.Append("\t\t//widgets\n");
            for(int i = 0; i < uIHierarchy.widgets.Count; i++)
            {
                var itemInfo = uIHierarchy.widgets[i];
                var typeName = itemInfo.item.GetType().ToString();
                sb.Append("\t\t").Append(itemInfo.name).Append(" = (").Append(typeName).Append(")UIHierarchy.widgets[").Append(i).Append("].item;\n");
            }
        }

        if(uIHierarchy.externals.Count > 0)
        {
            sb.Append("\t\t//externals\n");
            for(int i = 0; i < uIHierarchy.externals.Count; i++)
            {
                var itemInfo = uIHierarchy.externals[i];
                var typeName = itemInfo.item.GetType().ToString();
                sb.Append("\t\t").Append(itemInfo.name).Append(" = (").Append(typeName).Append(")UIHierarchy.externals[").Append(i).Append("].item;\n");
            }
        }
        
        sb.Append("\t}\n");
        sb.Append("}\n");

        File.WriteAllText(codePath, sb.ToString());
    }
}