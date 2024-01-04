using UnityEngine;

public class TestCameraManager : MonoBehaviour {
    private TestCamera.CameraManager cameraManager;
    void Start() {
        cameraManager = new TestCamera.CameraManager();
        Camera.main.depthTextureMode |= DepthTextureMode.Depth;
    }

    void Update() {
        cameraManager.OnUpdate();
    }
}