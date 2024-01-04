using UnityEngine;
using UnityEngine.UI;

public class OrientationController : MonoBehaviour {
    public Text text;
    public Text text1;
    private int lastScreenWidth;
    private float interval = 0.1f;

    private void Start() {
        // lastOrientation = Screen.orientation;
        lastScreenWidth = Screen.width;
    }

    void Update() {
        text.text = $"Orientation: {Input.deviceOrientation} Screen: {Screen.width} x {Screen.height}";

        interval+=Time.deltaTime;
        if (interval < 0.1f) {
            return;
        }

        if (lastScreenWidth != Screen.width) {
            OnOrientationChanged(Screen.width);
        }

        lastScreenWidth = Screen.width;
    }

    void OnOrientationChanged(int width) {
        Debug.LogError("屏幕方向已更改为 " + width);
        text1.text = "屏幕方向已更改为 " + width;
        // 在这里更新应用程序中的其他组件
    }
}