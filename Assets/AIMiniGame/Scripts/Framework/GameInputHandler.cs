using UnityEngine;

public class GameInputHandler : MonoBehaviour {
    private void OnEnable() {
        InputManager.Instance.Register(InputEvents.SpaceKeyDown, OnSpaceKeyDown);
        InputManager.Instance.Register<Vector2>(InputEvents.TouchStart, OnTouchStart);
    }

    private void OnDisable() {
        InputManager.Instance.Unregister(InputEvents.SpaceKeyDown, OnSpaceKeyDown);
        InputManager.Instance.Unregister<Vector2>(InputEvents.TouchStart, OnTouchStart);
    }

    private void OnSpaceKeyDown() {
        Debug.Log("Space key pressed");
    }

    private void OnTouchStart(Vector2 touchPosition) {
        Debug.Log($"Touch started at: {touchPosition}");
    }
}