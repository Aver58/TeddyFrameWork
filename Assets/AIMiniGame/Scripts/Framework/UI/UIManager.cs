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
public class UIManager : Singleton<UIManager> {
    private Canvas _uiCanvas; // UI根节点
    public Canvas UICanvas {
        get {
            if (_uiCanvas == null) {
                _uiCanvas = GameObject.Find("UICanvas").GetComponent<Canvas>();
            }

            return _uiCanvas;
        }
    }
    private Stack<UIViewBase> _uiStack = new(); // 界面堆栈
    private Dictionary<string, UIViewBase> _uiCache = new(); // 缓存已加载界面
    private readonly Dictionary<string, UIViewBase> windows = new ();

    // 打开界面
    public UIViewBase Open(ControllerBase controller) {
        var viewName = controller.FunctionName;
        var window = FindWindow(viewName);
        if (window == null) {
            window = InitializeWindow(controller);
        }

        return window;
    }

    // 关闭当前界面
    public void CloseTopUI() {
        if (_uiStack.Count > 0) {
            var uiViewBase = _uiStack.Pop();
            uiViewBase.Close();
            uiViewBase.gameObject.SetActive(false);
        }
    }

    private UIViewBase FindWindow(string windowName) {
        _uiCache.TryGetValue(windowName, out var window);
        return window;
    }

    private UIViewBase InitializeWindow(ControllerBase controller) {
        string viewName = controller.FunctionName;
        var parent = UICanvas.transform;
        if (_uiCache.TryGetValue(viewName, out var uiViewBase)) {
            uiViewBase.gameObject.SetActive(true);
            uiViewBase.Open();
        } else {
            var config = UIViewDefineConfig.Get(viewName);
            if (config == null) {
                Debug.LogError($"{viewName} not found in UIViewDefineConfig.csv");
                return null;
            }

            var assetPath = $"{ResourceConfig.Instance.UIAssetPathPrefix}/{config.assetName}.prefab";
            Addressables.LoadAssetAsync<GameObject>(assetPath).Completed += handle => {
                if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded) {
                    Debug.LogError($"Failed to load UI prefab: {assetPath}");
                    return;
                }

                var go = Object.Instantiate(handle.Result, parent);
                uiViewBase = go.GetComponent<UIViewBase>();
                if (uiViewBase == null) {
                    Debug.LogError($"{go} prefab not found UIViewBase component!");
                    return;
                }

                var layer = (UILayer)config.uILayer;
                uiViewBase.Init(layer);
                uiViewBase.BindController(controller);
                uiViewBase.Open();
                controller.Window = uiViewBase;
                _uiCache.Add(viewName, uiViewBase);
                _uiStack.Push(uiViewBase);
            };
        }

        return uiViewBase;
    }
}