using System;

namespace AIMiniGame.Scripts.Framework.UI {
    [Serializable]
    public enum UILayer {
        Background = 0,   // 底层UI（如场景背景）
        Normal = 1,       // 普通界面（如设置面板）
        Popup = 2,        // 弹窗（如确认框）
        Top = 3,          // 顶层UI（如加载提示）
        System = 4        // 系统级（如FPS显示）
    }
}