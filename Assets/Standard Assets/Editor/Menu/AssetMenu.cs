#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    AssetMenu.cs
 Author:      Zeng Zhiwei
 Time:        2020/12/14 15:01:09
=====================================================
*/
#endregion

using UnityEditor;

public class AssetMenu
{
    //[MenuItem("Asset/导出界面层级（嵌套层级）")]
    //public static void Assets_ExportPanelHierarchy_Nested()
    //{
    //    EditorHelper.ExportSelection("Export Panel Hierarchy Nested", ExportPanelHierarchy.ExportNested);
    //}

    [MenuItem("Assets/生成选中目录的模型预制，可以选择一个或者多个文件夹", false, 301)]
    public static void SetSelectHeroModels()
    {
        EditorHelper.ExportSelection("SetSelectCharacterModel ", ImporterHeroModel.ImportSelectFolder, SelectionMode.TopLevel);
    }
}