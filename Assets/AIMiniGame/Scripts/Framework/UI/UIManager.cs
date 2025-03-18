using System.Collections.Generic;
using AIMiniGame.Scripts.Framework.Resource;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

/*
UI Framework
├── **Core Layer**          -- 核心逻辑层
│    ├── UIManager          -- 界面生命周期管理
│    ├── UILayerController  -- 层级控制（如HUD/弹窗/全局）
├── **Logic Layer**         -- 业务逻辑层
│    ├── BaseUI             -- 所有UI的基类
│    ├── MVVM Components    -- 数据绑定组件
│    └── AnimationSystem    -- 动画控制器
└──── **Adapter Layer**     -- 适配层
     ├── DeviceAdapter      -- 设备分辨率适配
     └── Localization       -- 多语言支持
 */

// 路线图：
// 1. 界面传参
// 2. 内存管理
//      定期清理长时间未使用的缓存界面
//      使用Resources.UnloadUnusedAssets释放残留资源
// 3. 预加载高频使用界面
// 4. 返回导航栈
// 5. 弹窗模糊实现
// 6. 数据绑定
public class UIManager : MonoSingleton<UIManager> {
    public Canvas UICanvas; // UI根节点
    private Stack<UIViewBase> _uiStack = new(); // 界面堆栈
    private Dictionary<string, UIViewBase> _uiCache = new(); // 缓存已加载界面
    private const string Controller = "Controller";
    private const string ControllerNameSpace = "AIMiniGame.Scripts.Bussiness.Controller.";


    private void Awake() {
        UICanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
    }

    // 打开界面
    public T OpenUI<T>() where T : UIViewBase {
        var uiViewName = typeof(T).Name;
        if (_uiCache.TryGetValue(uiViewName, out UIViewBase uiView)) {
            uiView.gameObject.SetActive(true);
            uiView.OnOpen();
        } else {
            var config = UIViewDefineConfig.Get(uiViewName);
            if (config == null) {
                Debug.LogError($"UI {uiViewName} not found in UIViewDefineConfig");
                return null;
            }

            var assetPath = $"{ResourceConfig.Instance.UIAssetPathPrefix}/{config.assetName}.prefab";
            Addressables.LoadAssetAsync<GameObject>(assetPath).Completed += handle => {
                if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded) {
                    Debug.LogError($"Failed to load UI prefab: {assetPath}");
                    return;
                }

                GameObject go = Instantiate(handle.Result, UICanvas.transform);
                uiView = go.GetComponent<T>();
                var layer = (UILayer)config.uILayer;
                uiView.Init(layer);
                var controllerName = uiViewName + Controller;
                // 反射获取Controller
                var fullName = ControllerNameSpace + controllerName;
                var controllerType = System.Type.GetType(fullName);
                if (controllerType != null) {
                    var controller = System.Activator.CreateInstance(controllerType) as ControllerBase;
                    uiView.BindController(controller);
                } else {
                    Debug.LogError($"[OpenUI] Controller fullName {fullName} not found! uiViewName : {uiViewName}");
                }

                _uiCache.Add(uiViewName, uiView);
                _uiStack.Push(uiView);
            };
        }

        return uiView as T;
    }

    // 关闭当前界面
    public void CloseTopUI() {
        if (_uiStack.Count > 0) {
            var uiViewBase = _uiStack.Pop();
            uiViewBase.OnClose();
            uiViewBase.gameObject.SetActive(false);
        }
    }
}