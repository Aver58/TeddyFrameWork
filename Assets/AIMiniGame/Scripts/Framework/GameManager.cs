using System;
using UnityEngine;

public enum GameState {
    Playing,
    Paused,
    Ended
}

public class GameManager : Singleton<GameManager> {
    private GameState currentState;

    public GameState CurrentState {
        get { return currentState; }
        private set {
            if (currentState != value) {
                currentState = value;
                OnGameStateChanged?.Invoke(currentState);
            }
        }
    }

    public event Action<GameState> OnGameStateChanged;

    private void Start() {
        CurrentState = GameState.Playing; // 默认状态为Playing
    }

    public void PauseGame() {
        if (CurrentState == GameState.Playing) {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f; // 暂停游戏
        }
    }

    public void ResumeGame() {
        if (CurrentState == GameState.Paused) {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f; // 恢复游戏
        }
    }

    public void EndGame() {
        if (CurrentState != GameState.Ended) {
            CurrentState = GameState.Ended;
            Time.timeScale = 0f; // 停止游戏
        }
    }
}