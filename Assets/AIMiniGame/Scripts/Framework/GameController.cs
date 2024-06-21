using UnityEngine;

public class GameController : MonoBehaviour {
    private void Start() {
        // 注册游戏状态改变事件
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDestroy() {
        // 取消注册游戏状态改变事件
        if (GameManager.Instance != null) {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState newState) {
        switch (newState) {
            case GameState.Playing:
                Debug.Log("Game is playing.");
                break;
            case GameState.Paused:
                Debug.Log("Game is paused.");
                break;
            case GameState.Ended:
                Debug.Log("Game has ended.");
                break;
        }
    }

    public void OnPauseButtonClicked() {
        GameManager.Instance.PauseGame();
    }

    public void OnResumeButtonClicked() {
        GameManager.Instance.ResumeGame();
    }

    public void OnEndGameButtonClicked() {
        GameManager.Instance.EndGame();
    }
}