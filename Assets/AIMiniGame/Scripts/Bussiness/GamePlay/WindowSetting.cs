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

    #region 嘿莱赣ぃ睹э琌ㄏノ Window 矗ㄑ dll

    private struct MARGINS {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    #region Define function signatures to import from Windows APIs (把计嘿璹)

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
#if !UNITY_EDITOR
        window = GetActiveWindow();

        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);
        SetWindowLong(window, GWL_EXSTYLE, WS_EX_LAYERED);
        // 透明
        SetLayeredWindowAttributes(window, crKey: 0, bAlpha: 0, LWA_COLORKEY);
        SetWindowPos(window, HWND_TOPMOST, 0, 0, 0, 0, 0);

        Application.runInBackground = true;
#endif
    }
}