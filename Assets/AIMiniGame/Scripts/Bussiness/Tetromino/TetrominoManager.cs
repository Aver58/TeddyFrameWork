using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TetrominoManager : Singleton<TetrominoManager> {
    private List<GameObject> activeTetrominos = new List<GameObject>();
    public Transform CanvasTransform;

    public void SpawnTetromino(string id, Vector2Int startPosition, System.Action<GameObject> callback) {
        TetrominoConfig config = TetrominoConfig.Get(id);

        if (config != null) {
            ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/Tetromino.prefab", prefab => {
                if (prefab != null) {
                    GameObject tetromino = Instantiate(prefab, CanvasTransform, true);
                    tetromino.GetComponent<Tetromino>().Initialize(config.shape, config.color, startPosition);
                    activeTetrominos.Add(tetromino);
                    callback?.Invoke(tetromino);
                } else {
                    callback?.Invoke(null);
                }
            });
        } else {
            callback?.Invoke(null);
        }
    }

    public void ClearTetrominos() {
        foreach (var tetromino in activeTetrominos) {
            Destroy(tetromino);
        }
        activeTetrominos.Clear();
    }

    public void MoveTetromino(GameObject tetromino, Vector2Int direction) {
        tetromino.GetComponent<Tetromino>().Move(direction);
    }

    public void RotateTetromino(GameObject tetromino) {
        tetromino.GetComponent<Tetromino>().Rotate();
    }

    public void SetTetrominoPosition(GameObject tetromino, Vector2Int newPosition) {
        tetromino.GetComponent<Tetromino>().SetPosition(newPosition);
    }
}