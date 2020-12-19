#region Copyright © 2020 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    SceneModule.cs
 Author:      Zeng Zhiwei
 Time:        2020\12\18 星期五 22:12:43
=====================================================
*/
#endregion


using System;
using UnityEngine;
/// <summary>
/// 多场景管理器
/// todo 场景栈
/// </summary>
public class SceneModule : ModuleBase
{
    private static SceneBase m_curScene;

    #region API

    public static void ChangeScene(SceneID sceneID)
    {
        var lastSceneID = GetSceneID();
        if(lastSceneID == sceneID)
        {
            Debug.Log("[ChangeScene]current scene is the target... sceneID:" + sceneID.ToString());
            return;
        }

        if(IsLoading())
        {
            Debug.Log("[ChangeScene]current scene is loading....:" + lastSceneID.ToString() + sceneID.ToString());
            return;
        }

        // 打开遮羞布，过渡场景，或者过渡UI
        ExitLastScene();

        //新场景的加载
        EnterCurrentScene(sceneID);

        // 场景加载回调关闭遮羞布
    }

    #endregion

    #region Private
    public override void Update(float dt) 
    {
        // curScene Update
    }

    private static SceneID GetSceneID()
    {
        if(m_curScene == null)
            return SceneID.None;

        return m_curScene.SceneID;
    }

    private static bool IsLoading()
    {
        if(m_curScene == null)
            return false;

        return m_curScene.IsLoading();
    }

    private static SceneBase CreateScene(SceneID sceneID)
    {
        SceneBase scene;

        SceneConfig sceneConfig;
        SceneDefine.SceneMap.TryGetValue(sceneID, out sceneConfig);
        Type sceneClass = sceneConfig.viewClass;

        // 反射拿到实例
        scene = Activator.CreateInstance(sceneClass) as SceneBase;
        if(scene != null)
        {
            scene.SceneID = sceneID;
            scene.assetPath = sceneConfig.path;
            scene.panelName = sceneConfig.name;
            return scene;
        }

        return null;
    }
    
    private static void ExitLastScene()
    {
        //卸载上一个场景，
        if(m_curScene != null)
            m_curScene.OnExit();

        //调用ui模块的退出场景，
        UIModule.SceneExit();
        // 卸载AB
        //主动触发GC
        System.GC.Collect();
        //清理内存
        Resources.UnloadUnusedAssets();
    }

    private static void EnterCurrentScene(SceneID sceneID)
    {
        var scene = CreateScene(sceneID);
        // 异步加载场景
        scene.Load(OnSceneLoaded);

        m_curScene = scene;
    }

    private static void OnSceneLoaded(AssetRequest assetRequest)
    {
        //调用ui模块的进入场景
        UIModule.SceneEnter();
        // 当前场景OnEnter
        m_curScene.OnEnter();
        //发送场景切换事件
    }
    #endregion
}