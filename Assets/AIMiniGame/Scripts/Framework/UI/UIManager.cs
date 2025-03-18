using System.Collections.Generic;
using AIMiniGame.Scripts.Framework.Resource;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

/*
UI Framework
├── **Resource Layer**      -- 资源管理层（加载/卸载/缓存）
│    ├── AddressablesLoader
│    └── PoolManager
├── **Core Layer**          -- 核心逻辑层
│    ├── UIManager          -- 界面生命周期管理
│    ├── UILayerController  -- 层级控制（如HUD/弹窗/全局）
│    └── EventSystem        -- 事件分发中心
├── **Logic Layer**         -- 业务逻辑层
│    ├── BaseUI             -- 所有UI的基类
│    ├── MVVM Components    -- 数据绑定组件
│    └── AnimationSystem    -- 动画控制器
└──── **Adapter Layer**     -- 适配层
     ├── DeviceAdapter      -- 设备分辨率适配
     └── Localization       -- 多语言支持
 */

// 路径图：
// 1. 界面传参
// 2. 内存管理
//      定期清理长时间未使用的缓存界面
//      使用Resources.UnloadUnusedAssets释放残留资源
// 3. 预加载高频使用界面
public class UIManager : MonoSingleton<UIManager> {
    private Stack<BaseUI> _uiStack = new Stack<BaseUI>(); // 界面堆栈
    private Dictionary<string, BaseUI> _uiCache = new Dictionary<string, BaseUI>(); // 缓存已加载界面

    // 打开界面
    public T OpenUI<T>() where T : BaseUI {
        var uiName = typeof(T).Name;
        if (_uiCache.TryGetValue(uiName, out BaseUI ui)) {
            // 从缓存中激活
            ui.gameObject.SetActive(true);
            ui.OnShow();
        } else {
            // 异步加载预制体
            var config = UIViewDefineConfig.Get(uiName);
            if (config == null) {
                Debug.LogError($"UI {uiName} not found in UIViewDefineConfig");
                return null;
            }

            var assetPath = $"{ResourceConfig.Instance.UIAssetPathPrefix}/{config.assetName}.prefab";
            Addressables.LoadAssetAsync<GameObject>(assetPath).Completed += handle => {
                GameObject go = Instantiate(handle.Result);
                ui = go.GetComponent<T>();
                ui.Init((UILayer)config.uILayer);
                _uiCache.Add(uiName, ui);
                _uiStack.Push(ui);
            };
        }

        return ui as T;
    }

    // 关闭当前界面
    public void CloseTopUI() {
        if (_uiStack.Count > 0) {
            BaseUI ui = _uiStack.Pop();
            ui.OnHide();
            ui.gameObject.SetActive(false);
        }
    }
}