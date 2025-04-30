using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class PetView : UIViewBase {
    public Button _testButton;
    private IntPtr window;

    #region WindowsAPI
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    private struct MARGINS {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowlonga
    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hwnd, int index);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

    const int GWL_EXSTYLE = -20;
    const int LWA_ALPHA = 0x2;
    const int SWP_NOMOVE = 0x0002;
    const int SWP_NOSIZE = 0x0001;
    const int SWP_SHOWWINDOW = 0x0040;
    const int WM_NCHITTEST = 0x0084;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint LWA_COLORKEY = 0x00000001;
    #endregion

    private void Start() {
        Debug.LogError($"Start");
        _testButton.onClick.AddListener(() => {
            Debug.LogError($"_testButton.onClick");

        });

        var window = GetWindowHandle();
#if !UNITY_EDITOR
         // 设置窗口无边框+置顶
        SetWindowLong(window, GWL_EXSTYLE, WS_EX_LAYERED);
        // 设置透明度（0完全透明，255不透明）
        SetLayeredWindowAttributes(window, 0, 0, LWA_ALPHA);
        // 窗口置顶
        SetWindowPos(window, (IntPtr)(-1), 0, 0, 0, 0, 0);//SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW
        // 将 cxLeftWidth 设置为 -1 是一个特殊值，表示扩展整个窗口的客户区，使其透明
        // 创建无边框窗口或实现透明效果
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(window, ref margins);

#endif

        Application.runInBackground = true;
    }

    private IntPtr GetWindowHandle() {
        window = GetActiveWindow();
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return Process.GetCurrentProcess().MainWindowHandle;
#else
        return window;
#endif
    }

    private void Update() {
        // if (Input.GetMouseButtonDown(0)) {
        //     Debug.LogError($"GetMouseButtonDown 0");
        //     // 将点击事件透传到底层窗口
        //     SendMessage(GetWindowHandle(), WM_NCHITTEST, IntPtr.Zero, IntPtr.Zero);
        // }
    }

    protected override void UpdateView() {
        // if (Controller is TestViewController testViewController) {
        //     _testText.text = $"Health: {testViewController.Model.Health}";
        // }
    }
}
