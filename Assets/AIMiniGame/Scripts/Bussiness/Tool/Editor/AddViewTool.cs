using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddViewTool : EditorWindow {
    // todo viewName 缓存
    private string viewName = ""; // 界面名称
    private bool isGenerateController = true;
    private bool isGenerateModel = false;
    private bool isGenerateView = true;
    private bool isGenerateViewCsv = true;
    [MenuItem("Tools/AddViewTool")]
    private static void GetWindow() {
        GetWindow<AddViewTool>("添加界面工具");
    }

    // 一键新增界面
    private void OnGUI() {
        // 文本输入框
        viewName = EditorGUILayout.TextField("界面名称", viewName);
        isGenerateController = EditorGUILayout.Toggle("生成Controller", isGenerateController);
        isGenerateModel = EditorGUILayout.Toggle("生成Model", isGenerateModel);
        isGenerateView = EditorGUILayout.Toggle("生成View", isGenerateView);
        isGenerateViewCsv = EditorGUILayout.Toggle("生成UIViewDefine.csv", isGenerateViewCsv);
        // 确认按钮
        if (GUILayout.Button("添加界面")){
            // 生成界面
            CreateView(viewName);
        }

        GUILayout.Label("注意：生成预制体需要在View class 编译后才能挂载，请确保脚本编译完成后再使用！");
        if (GUILayout.Button("生成预制体")) {
            GeneratePrefab(viewName);
        }
    }

    private void CreateView(string viewName) {
        // 检查输入是否合法
        if (string.IsNullOrEmpty(viewName)) {
            EditorUtility.DisplayDialog("错误", "请输入合法的界面名称", "确定");
            return;
        }

        if (isGenerateController) {
            // 1.Assets\AIMiniGame\Scripts\Bussiness\Controller 文件夹下新增文件 viewNameController.cs
            var controllerPath = $"Assets/AIMiniGame/Scripts/Bussiness/Controller/{viewName}Controller.cs";
            // 如果文件已存在，提示用户
            if (System.IO.File.Exists(controllerPath)) {
                EditorUtility.DisplayDialog("错误", $"{viewName}Controller.cs 文件已存在，请更换名称", "确定");
                return;
            }

            // 创建文件夹
            var controllerDir = System.IO.Path.GetDirectoryName(controllerPath);
            if (!System.IO.Directory.Exists(controllerDir)) {
                System.IO.Directory.CreateDirectory(controllerDir);
            }

            // 替换 ControllerTemplate.txt 的 #ClassName# 为 viewName
            var templateFilePath = $"Assets/AIMiniGame/Scripts/Bussiness/Tool/AddViewTemplate/ControllerTemplate.txt";
            var controllerTemplate = System.IO.File.ReadAllText(templateFilePath);
            controllerTemplate = controllerTemplate.Replace("#ClassName#", viewName);
            System.IO.File.WriteAllText(controllerPath, controllerTemplate);
        }

        if (isGenerateModel) {
            // 2. Assets\AIMiniGame\Scripts\Bussiness\Model 文件夹下新增文件 viewNameModel.cs
            var modelPath = $"Assets/AIMiniGame/Scripts/Bussiness/Model/{viewName}Model.cs";
            // 如果文件已存在，提示用户
            if (System.IO.File.Exists(modelPath)) {
                EditorUtility.DisplayDialog("错误", $"{viewName}Model.cs 文件已存在，请更换名称", "确定");
                return;
            }
            // 创建文件夹
            var modelDir = System.IO.Path.GetDirectoryName(modelPath);
            if (!System.IO.Directory.Exists(modelDir)) {
                System.IO.Directory.CreateDirectory(modelDir);
            }
            // 替换 ModelTemplate.txt 的 #ClassName# 为 viewName
            var modelTemplateFilePath = $"Assets/AIMiniGame/Scripts/Bussiness/Tool/AddViewTemplate/ModelTemplate.txt";
            var modelTemplate = System.IO.File.ReadAllText(modelTemplateFilePath);
            modelTemplate = modelTemplate.Replace("#ClassName#", viewName);
            System.IO.File.WriteAllText(modelPath, modelTemplate);
        }

        if (isGenerateView) {
            // 3. Assets\AIMiniGame\Scripts\Bussiness\View 文件夹下新增文件 viewNameView.cs
            var viewPath = $"Assets/AIMiniGame/Scripts/Bussiness/View/{viewName}View.cs";
            // 如果文件已存在，提示用户
            if (System.IO.File.Exists(viewPath)) {
                EditorUtility.DisplayDialog("错误", $"{viewName}View.cs 文件已存在，请更换名称", "确定");
                return;
            }
            // 创建文件夹
            var viewDir = System.IO.Path.GetDirectoryName(viewPath);
            if (!System.IO.Directory.Exists(viewDir)) {
                System.IO.Directory.CreateDirectory(viewDir);
            }
            // 替换 ViewTemplate.txt 的 #ClassName# 为 viewName
            var viewTemplateFilePath = $"Assets/AIMiniGame/Scripts/Bussiness/Tool/AddViewTemplate/ViewTemplate.txt";
            var viewTemplate = System.IO.File.ReadAllText(viewTemplateFilePath);
            viewTemplate = viewTemplate.Replace("#ClassName#", viewName);
            System.IO.File.WriteAllText(viewPath, viewTemplate);
        }

        if (isGenerateViewCsv) {
            // 4. UIViewDefine.csv 新增配置
            var csvPath = $"Assets/AIMiniGame/ToBundle/Config/UIViewDefine.csv";
            // 如果文件不存在，提示用户
            if (!System.IO.File.Exists(csvPath)) {
                EditorUtility.DisplayDialog("错误", "UIViewDefine.csv 文件不存在，请检查", "确定");
                return;
            }

            // csvContent 文件尾新增一行 PetView,宠物界面,PetView,1
            var csvLines = System.IO.File.ReadAllLines(csvPath).ToList();
            var viewId = $"{viewName}";
            var newLine = $"{viewId},{viewName}界面,{viewName}View,1";
            csvLines.Add(newLine);
            System.IO.File.WriteAllLines(csvPath, csvLines);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void GeneratePrefab(string viewName) {
        if (!string.IsNullOrEmpty(viewName)) {
            // 5. 新增预制
            var prefabPath = $"Assets/AIMiniGame/ToBundle/Prefabs/{viewName}View.prefab";
            // 如果文件已存在，提示用户
            if (System.IO.File.Exists(prefabPath)) {
                EditorUtility.DisplayDialog("错误", $"{viewName}View.prefab 预制体已存在，请更换名称", "确定");
            } else {
                // 创建文件夹
                var prefabDir = System.IO.Path.GetDirectoryName(prefabPath);
                if (!System.IO.Directory.Exists(prefabDir)) {
                    System.IO.Directory.CreateDirectory(prefabDir);
                }
                // 创建空节点，命名为 {viewName}View，新增组件 {viewName}View，保存预制
                var prefab = new GameObject($"{viewName}View");
                if (prefab != null) {
                    var viewComponentType = System.Type.GetType($"{viewName}View");
                    var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies) {
                        var type = assembly.GetType($"{viewName}View");
                        if (type != null) {
                            viewComponentType = type;
                            break;
                        }
                    }
                    if (viewComponentType == null) {
                        Debug.LogError($"未找到类型 {viewName}View，请检查类名和命名空间！");
                        return;
                    }
                    prefab.AddComponent(viewComponentType);
                    prefab.AddComponent<RectTransform>();

                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
                    Debug.Log($"已保存预制：{prefabPath}", prefab);

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    Selection.activeGameObject = prefab;
                    EditorGUIUtility.PingObject(prefab);
                }
            }
        }
    }
}
