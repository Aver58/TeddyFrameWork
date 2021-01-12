#region Copyright © 2018 Aver. All rights reserved.
/*
=====================================================
 AverFrameWork v1.0
 Filename:    ImporterHeroModel.cs
 Author:      Zeng Zhiwei
 Time:        2020/1/13 11:19:25
=====================================================
*/
#endregion

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering;

public class ImporterHeroModel
{
    // 新增模型路径映射：资源路径，预制路径
    private static Dictionary<string, string> _characterPathMap = new Dictionary<string, string>()
    {
        { "Assets/Art/character/hero", "Assets/Data/character/hero" },
    };

    // 目标控制器路径
    private static Dictionary<string, string> _targetAnimatorControllers = new Dictionary<string, string>()
    {
        { "hero", "Assets/Art/character/animator/hero.controller" },
    };

    // 是否需要额外修饰的材质映射，比如围猎需要个影子
    private static Dictionary<string, string> _decorateMaterialMap = new Dictionary<string, string>
    {
        //{ "hunt","Assets/Art/character/common/HuntPanelShadow.mat"},
    };

    // 业务需要的额外操作
    private static Dictionary<string, string> _finishHandlerMap = new Dictionary<string, string>
    {
        //{ "hunt","HuntModelFinishHandler"},
    };

    #region 业务按需拼接自己需要的模型导入功能

    //private void HuntModelFinishHandler(string characterModelPath, string prefabCharacterPath, string prefabPath)
    //{
    //    Debug.Log("HuntModelFinishHandler ：" + characterModelPath);

    //}

    #endregion

    #region Origin
    // 导出指定文件夹模型
    public static void ImportSelectFolder(Object obj)
    {
        string path = AssetDatabase.GetAssetPath(obj);

        // 排除非文件夹情况
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            return;

        int index = path.IndexOf("Assets/Art/character");
        if (index != -1)
        {
            // 选中了整个模型文件夹
            string prefabCharacterPath;
            string p = Path.GetDirectoryName(path).Replace("\\","/");
            if (_characterPathMap.TryGetValue(p, out prefabCharacterPath))
            {
                ImportOneCharacterModel(path, prefabCharacterPath);
            }
            else
            {
                Debug.LogError("没有找到对应的模型配置信息！" + p);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    // 导入一个模型
    public static void ImportOneCharacterModel(string characterModelPath, string prefabCharacterPath)
    {

        string prefabPath = ImportOneCharacterModelAnimator(characterModelPath, prefabCharacterPath);

        // 结束回调
        var finishHandlerName = "";
        foreach (var item in _finishHandlerMap)
        {
            if (characterModelPath.Contains(item.Key))
            {
                finishHandlerName = item.Value;
                break;
            }
        }
        if (!string.IsNullOrEmpty(finishHandlerName))
        {
            System.Type type = System.Type.GetType("ImporterHeroModel");                      // 通过类名获取同名类
            System.Object obj = System.Activator.CreateInstance(type);          // 创建实例
            MethodInfo method = type.GetMethod(finishHandlerName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.IgnoreCase);      // 获取方法信息
            object[] parameters = { characterModelPath, prefabCharacterPath, prefabPath };
            method.Invoke(obj, parameters);                                     // 调用方法，参数为空
        }
    }

    // 导入一个模型的所有Animator
    private static string ImportOneCharacterModelAnimator(string artModelfolder, string prefabCharacterPath)
    {
        Debug.Log(artModelfolder);
        string[] modelAssets = AssetDatabase.FindAssets("t:Model", new string[] { artModelfolder });
        if (modelAssets.Length == 0)
        {
            return "";
        }

        string characterAnimatorFolderName = Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(modelAssets[0]));
        string characterModelName = Path.GetFileNameWithoutExtension(artModelfolder + ".test").ToLower();

        ModelImporter mainModelImporter = null;
        List<ModelImporter> animationModelImporters = new List<ModelImporter>();
        UnityEngine.Object mainFbxAvatar = null;
        for (int i = 0; i < modelAssets.Length; i++)
        {
            string modelAssetPath = AssetDatabase.GUIDToAssetPath(modelAssets[i]);
            ModelImporter modelImporter = AssetImporter.GetAtPath(modelAssetPath) as ModelImporter;
            if (modelImporter == null)
                continue;

            modelImporter.isReadable = false;
            modelImporter.importTangents = ModelImporterTangents.None;
            modelImporter.animationType = ModelImporterAnimationType.Generic;

            if (modelAssetPath.Contains("animator"))
            {
                animationModelImporters.Add(modelImporter);
                continue;
            }

            if (modelAssetPath.Contains("model"))
            {
                if (mainModelImporter != null)
                {
                    Debug.LogErrorFormat("多余的Model模型FBX{0}", modelAssetPath);
                }

                modelImporter.materialLocation = ModelImporterMaterialLocation.External;// 2020.1.20 2017版本修改成外部材质
                mainModelImporter = modelImporter;
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(modelAssetPath);
                string avatarName = Path.GetFileNameWithoutExtension(modelAssetPath) + "Avatar";
                for (int z = 0; z < objs.Length; z++)
                {
                    if (objs[z].name == avatarName)
                    {
                        mainFbxAvatar = objs[z];
                        break;
                    }
                }

                continue;
            }
        }

        SetTextureImportSetting(artModelfolder);

        for (int i = 0; i < animationModelImporters.Count; i++)
        {
            animationModelImporters[i].sourceAvatar = mainFbxAvatar as Avatar;
            AssetDatabase.WriteImportSettingsIfDirty(animationModelImporters[i].assetPath);
        }

        AnimatorOverrideController animatorOverrideController;
        string prefabPath = "";
        if (GenerateAnimatorController(characterAnimatorFolderName, characterModelName, animationModelImporters, out animatorOverrideController))
        {
            prefabPath = SettingNormalModel(artModelfolder, characterModelName, prefabCharacterPath, mainModelImporter, animatorOverrideController);
        }

        return prefabPath;
    }

    private static string SettingNormalModel(string artModelfolder, string heroModelName, string prefabCharacterPath, ModelImporter mainModelImporters, AnimatorOverrideController animatorOverrideController)
    {
        if (mainModelImporters == null)
        {
            return "";
        }


        mainModelImporters.optimizeGameObjects = true;

        AssetDatabase.WriteImportSettingsIfDirty(mainModelImporters.assetPath);
        AssetDatabase.Refresh();

        GameObject sourceModelGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(mainModelImporters.assetPath);
        GameObject prefabGameObject = PrefabUtility.InstantiatePrefab(sourceModelGameObject) as GameObject;
        CreateDummyPoint(prefabGameObject);

        Animator animator = prefabGameObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorOverrideController;

        SettingDecorateMaterialAndRenderParam(prefabGameObject);

        string prefabPath;
        GameObject newPrefabGameObejct = CreateOrReplacePrefab(prefabCharacterPath, heroModelName, prefabGameObject, out prefabPath);// 在Data创建预制
        MonoBehaviour.DestroyImmediate(prefabGameObject);

        CreateExtraPrefabsByMaterials(artModelfolder, prefabCharacterPath, heroModelName, newPrefabGameObejct);

        return prefabPath;
    }

    //创建人物模型各个节点
    private static void CreateDummyPoint(GameObject prefabGameObject)
    {
        Transform prefabTransform = prefabGameObject.transform;

        Transform bipRootTransform = prefabTransform.Find("Bip001");
        if (bipRootTransform)
        {
            GameObject dummyFootGameObject = new GameObject("Dummy Bip001 Foot");
            dummyFootGameObject.transform.parent = bipRootTransform;
        }

        Transform bipHeadTransform = prefabTransform.Find("Bip001 Head");
        if (bipHeadTransform)
        {
            GameObject dummyOverHeadGameoObject = new GameObject("Dummy OverHead");
            Transform dummyOverHeadTransform = dummyOverHeadGameoObject.transform;
            Vector3 overHeadPosition = bipHeadTransform.transform.position;
            overHeadPosition.y += 0.4f;
            dummyOverHeadTransform.position = overHeadPosition;
            dummyOverHeadTransform.parent = prefabGameObject.transform;
        }

        Transform bidSpineTransform = prefabTransform.Find("Bip001 Spine1");
        if (bidSpineTransform == null)
        {
            bidSpineTransform = prefabTransform.Find("Bip001 Spine");
        }

        if (bidSpineTransform)
        {
            GameObject dummySpineGameObject = new GameObject("Dummy Spine");
            Transform dummySpineTransform = dummySpineGameObject.transform;
            dummySpineTransform.position = bidSpineTransform.position;
            dummySpineTransform.parent = prefabTransform;
        }
    }

    // 额外根据材质创建prefab
    private static void CreateExtraPrefabsByMaterials(string artModelfolder, string prefabCharacterPath, string heroModelName, GameObject sourcePrefab)
    {
        string[] materialAssets = AssetDatabase.FindAssets("t:Material", new string[] { artModelfolder });
        if (materialAssets.Length == 0)
            return;

        string extraPrefabMaterialSign = heroModelName + "_copy_";
        for (int i = 0; i < materialAssets.Length; i++)
        {
            string materialAssetPath = AssetDatabase.GUIDToAssetPath(materialAssets[i]);
            if (materialAssetPath.Contains(extraPrefabMaterialSign))
            {
                GameObject newPrefabGameObject = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;
                ReplaceMaterial(newPrefabGameObject, materialAssetPath, heroModelName);
                string prefabPath;
                CreateOrReplacePrefab(prefabCharacterPath, Path.GetFileNameWithoutExtension(materialAssetPath), newPrefabGameObject, out prefabPath);
                MonoBehaviour.DestroyImmediate(newPrefabGameObject);
            }
        }
    }

    private static void ReplaceMaterial(GameObject prefabGameObject, string newMaterialPath, string oldMaterialName)
    {
        var skinnedMeshRenders = prefabGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skinnedMeshRenders.Length; i++)
        {
            var sharedMaterials = skinnedMeshRenders[i].sharedMaterials;
            for (int j = 0; j < sharedMaterials.Length; j++)
            {
                if (sharedMaterials[j].name == oldMaterialName)
                {
                    sharedMaterials[j] = AssetDatabase.LoadAssetAtPath<Material>(newMaterialPath);
                }
            }

            skinnedMeshRenders[i].sharedMaterials = sharedMaterials;
        }
    }

    private static GameObject CreateOrReplacePrefab(string prefabCharacterPath, string heroModelName, GameObject prefabGameObject, out string prefabPath)
    {
        prefabPath = prefabCharacterPath + "/" + heroModelName + ".prefab";
        if (File.Exists(prefabPath))
        {
            UnityEngine.Object oldPrefab = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(prefabPath);
            return PrefabUtility.ReplacePrefab(prefabGameObject, oldPrefab, ReplacePrefabOptions.ReplaceNameBased);
        }
        else
        {
            FileHelper.CreateDirectory(prefabCharacterPath);
            return PrefabUtility.CreatePrefab(prefabPath, prefabGameObject);
        }
    }

    private static void SettingDecorateMaterialAndRenderParam(GameObject prefabGameobject)
    {
        SkinnedMeshRenderer[] skinRenders = prefabGameobject.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < skinRenders.Length; i++)
        {
            SkinnedMeshRenderer skinRender = skinRenders[i];
            skinRender.lightProbeUsage = LightProbeUsage.Off;
            skinRender.reflectionProbeUsage = ReflectionProbeUsage.Off;
            skinRender.shadowCastingMode = ShadowCastingMode.Off;
            skinRender.receiveShadows = false;
            skinRender.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            string _decorateMaterial = "";
            foreach (var item in _decorateMaterialMap)
            {
                if (prefabGameobject.name.Contains(item.Key))
                {
                    _decorateMaterial = item.Value;
                    break;
                }
            }
            if (!string.IsNullOrEmpty(_decorateMaterial))
            {
                List<Material> newMaterials = new List<Material>();
                newMaterials.AddRange(skinRender.sharedMaterials);

                Material material = AssetDatabase.LoadAssetAtPath<Material>(_decorateMaterial);
                newMaterials.Add(material);

                skinRender.sharedMaterials = newMaterials.ToArray();
            }
        }
    }

    // 材质纹理属性设置mipmapEnabled、isReadable关闭
    private static void SetTextureImportSetting(string artModelfolder)
    {
        string[] modelAssets = AssetDatabase.FindAssets("t:Texture", new string[] { artModelfolder });
        if (modelAssets.Length == 0)
        {
            return;
        }

        for (int i = 0; i < modelAssets.Length; i++)
        {
            string modelAssetPath = AssetDatabase.GUIDToAssetPath(modelAssets[i]);
            TextureImporter textureImporter = AssetImporter.GetAtPath(modelAssetPath) as TextureImporter;
            if (textureImporter == null)
            {
                continue;
            }

            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false;
        }
    }

    // 生成动画控制器
    private static bool GenerateAnimatorController(string characterAnimatorFolderName, string characterModelName, List<ModelImporter> animModelImporters, out AnimatorOverrideController animatorOverrideController)
    {
        animatorOverrideController = null;

        Dictionary<string, AnimationClip> newAnimationClips = new Dictionary<string, AnimationClip>();
        for (int i = 0; i < animModelImporters.Count; i++)
        {
            ModelImporter animModelImporter = animModelImporters[i];
            Object[] sourceAniamtionClips = AssetDatabase.LoadAllAssetsAtPath(animModelImporter.assetPath);
            if (sourceAniamtionClips == null)
            {
                continue;
            }

            for (int j = 0; j < sourceAniamtionClips.Length; j++)
            {
                var sourceAnimationClip = sourceAniamtionClips[j];
                if (sourceAnimationClip.GetType() != typeof(AnimationClip))
                {
                    continue;
                }

                if (sourceAnimationClip.name.Contains("_preview"))
                {
                    continue;
                }

                newAnimationClips[sourceAnimationClip.name.ToLower()] = (AnimationClip)sourceAnimationClip;
            }
        }

        if (newAnimationClips.Count == 0)
        {
            return false;
        }

        string targetControllerPath = string.Empty;
        foreach (var item in _targetAnimatorControllers)
        {
            if (characterModelName.Contains(item.Key))
            {
                targetControllerPath = item.Value;
                break;
            }
        }

        if (string.IsNullOrEmpty(targetControllerPath))
        {
            Debug.LogErrorFormat("模型{0}没找到目标控制器", characterModelName);
            return false;
        }

        AnimatorController targetAnimatorController = AssetDatabase.LoadAssetAtPath<AnimatorController>(targetControllerPath);
        string animatorOverrideControllerPath = characterAnimatorFolderName + "/" + characterModelName + ".overrideController";
        // 2020.08.10 直接SetDirty有bug，会存在旧引用,改成直接删除，每次都重新创建animatorOverrideController
        if(File.Exists(animatorOverrideControllerPath))
        {
            AssetDatabase.DeleteAsset(animatorOverrideControllerPath);
        }

        animatorOverrideController = new AnimatorOverrideController(targetAnimatorController);

        var clips = animatorOverrideController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            string clipName = clips[i].name.ToLower();
            AnimationClip overrideAnimClip;
            if (newAnimationClips.TryGetValue(clipName, out overrideAnimClip))
            {
                animatorOverrideController[clipName] = overrideAnimClip;
            }
            else
            {
                Debug.LogWarningFormat("模型{0}fbx中未找到名字为{1}的动画文件", characterModelName, clipName);
            }
        }

        AssetDatabase.CreateAsset(animatorOverrideController, animatorOverrideControllerPath);

        return true;
    }
    #endregion
}