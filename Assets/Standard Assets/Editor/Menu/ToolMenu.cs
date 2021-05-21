#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ToolMenu.cs
 Author:      Zeng Zhiwei
 Time:        2018/11/9 13:53:33
=====================================================
*/
#endregion
using UnityEditor;

public class ToolMenu
{
    [MenuItem("Tools/导出AssetBundle", false, 1)]
    static void ExportBundle()
    {
        ExportAssetBundle.BuildBundle();
    }

    //[MenuItem("Tools/设置AssetBundle", false, 2)]
    //static void SetAllAssetBundle()
    //{
    //     demo 先不写了，直接手动设置
    //    ImportAssetBundle.Reimport();
    //}

    //[MenuItem("Tools/导出动态加载图片配置", false, 3)]
    //static void ExportDynSpriteCfg()
    //{
    //    ExportAtlasCfg.ExportDynCfg();
    //}

    //[MenuItem("Tools/设置所有图集", false, 4)]
    //static void SetAllSpriteAtlas()
    //{
    //    ImportSprite.ImportAll();
    //}

    [MenuItem("Tools/初始化shader", false, 5)]
    static void InitShader()
    {
        UnityEngine.Shader.SetGlobalFloat("_GlobalBrightness", 1);
        UnityEngine.Shader.SetGlobalFloat("_SingleBrightness", 0);
        //ShaderManager.InitGlobal();
    }
    //创建so
    //[MenuItem("Tools/ConfigData Creator")]
    //chestatic void NewSO()
    //{
    //    string path = BaseDef.CONFIG;
    //    EditorHelper.CreateAsset<ConfigData>(path);
    //}
}

