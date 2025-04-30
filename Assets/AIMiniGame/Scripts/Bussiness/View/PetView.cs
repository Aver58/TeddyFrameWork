using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using AIMiniGame.Scripts.Framework.UI;
using Debug = UnityEngine.Debug;

public class PetView : UIViewBase
{
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    const uint HWND_TOPMOST = 0x0001;
    const uint SWP_NOSIZE = 0x0001;
    const uint SWP_NOMOVE = 0x0002;

    private void Start()
    {
        Debug.LogError($"XXXX");
        SetWindowPos(Process.GetCurrentProcess().MainWindowHandle, 
                    (IntPtr)HWND_TOPMOST, 
                    0, 0, 0, 0, 
                    SWP_NOSIZE | SWP_NOMOVE);
    }

    protected override void UpdateView() {
        // if (Controller is TestViewController testViewController) {
        //     _testText.text = $"Health: {testViewController.Model.Health}";
        // }
    }
}
