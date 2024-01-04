using UnityEngine;

public class TestCameraRequest : MonoBehaviour {
    void Start() {
        
    }

    void Update() {
        // 点击右键就会触发相机请求，请求一个开镜相机请求，测个开镜，关镜
        if (Input.GetMouseButtonDown(1)) {
            TestCamera.CameraManager.Instance.RequestCodeDrivenCamera(Camera.main, 1, 1f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        Debug.LogError("OnTriggerEnter");
        // 相机请求
        TestCamera.CameraManager.Instance.RequestCodeDrivenCamera(Camera.main, 1, 1f);
    }
}
