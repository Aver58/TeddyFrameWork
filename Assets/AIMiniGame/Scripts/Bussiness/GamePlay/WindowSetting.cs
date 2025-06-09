using System;
using System.Runtime.InteropServices;
using UnityEngine;

/* NOTE:
 * # Project Settings > Player > Resolution and Presentation > Use DXGI Flip Model Swapchain for D3D11(DO NOT CHECK) */
public class WindowSetting : MonoBehaviour {
    IntPtr window;

    // Definitions of window styles
    const int GWL_EXSTYLE = -20;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint LWA_COLORKEY = 0x00000001;

    // Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
    static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    #region

    private struct MARGINS {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region Define function signatures to import from Windows APIs

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    #endregion

    #endregion

    void Start() {
//#if !UNITY_EDITOR
//        window = GetActiveWindow();
//        // 全屏模式下扩展窗口到客户端区域 -> 为了透明 // 边距内嵌值确定在窗口四侧扩展框架的距离 -1为没有窗口边框
//        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
//        DwmExtendFrameIntoClientArea(window, ref margins);
//        // 设置为透明、无边框
//        SetWindowLong(window, GWL_EXSTYLE, WS_EX_LAYERED);
//        // bAlpha 透明
//        SetLayeredWindowAttributes(window, crKey: 0, bAlpha: 0, LWA_COLORKEY);
//        // 窗口置顶
//        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);
//#endif
        // 设置窗口最大化
        Application.runInBackground = true;
    }
}