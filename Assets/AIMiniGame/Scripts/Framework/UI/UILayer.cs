using System;

namespace AIMiniGame.Scripts.Framework.UI {
    [Serializable]
    public enum UILayer {
        Background,  // 底层UI（如场景背景）
        Normal,       // 普通界面（如设置面板）
        Popup,        // 弹窗（如确认框）
        Top,          // 顶层UI（如加载提示）
        System        // 系统级（如FPS显示）
    }
}