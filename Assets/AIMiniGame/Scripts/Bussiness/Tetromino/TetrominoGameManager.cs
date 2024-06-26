using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TetrominoGameManager : MonoBehaviour {
    private int rows = 20;
    private int columns = 10;
    private float dropInterval = 1.0f;
    private float dropTimer = 0.0f;
    public Transform TetrominoParnet;
    public Transform GridLayoutBG;
    private Tetromino currentTetromino;
    private Vector2Int spawnPosition = new Vector2Int(5, 20);
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    private List<GameObject> activeTetrominos = new List<GameObject>();

    private Dictionary<int, TextMeshProUGUI> textMap = new Dictionary<int, TextMeshProUGUI>();

    void Start() {
#if UNITY_EDITOR
        InitBG();
#endif
        StartGame();
    }

    private void InitBG() {
        ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/GirdBg.prefab", prefab => {
            if (prefab != null) {
                for (int row = 0; row < rows; row++) {
                    for (int column = 0; column < columns; column++) {
                        var cell = Instantiate(prefab, GridLayoutBG);
                        var textComponent = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        textComponent.text = $"{column},{row}";

                        textMap.Add(row * columns + column, textComponent);
                    }
                }
            }
        });
    }

    void Update() {
        dropTimer += Time.deltaTime;
        if (dropTimer >= dropInterval) {
            dropTimer = 0.0f;
            DropCurrentTetromino();
        }

        HandleInput();
    }

    void OnDestroy() {
        // Cleanup if necessary
    }

    private void StartGame() {
        SpawnTetromino();
    }

    private void GameOver() {
        ClearTetrominos();
        occupiedCells.Clear();
    }

    private void SpawnTetromino() {
        var tetrominoIds = TetrominoConfig.GetKeys();
        var randomId = tetrominoIds[Random.Range(0, tetrominoIds.Count)];
        SpawnTetromino(randomId, spawnPosition);
    }

    public void SpawnTetromino(string id, Vector2Int startPosition) {
        TetrominoConfig config = TetrominoConfig.Get(id);
        if (config != null) {
            ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/Tetromino.prefab", prefab => {
                if (prefab != null) {
                    var tetrominoGo = Instantiate(prefab, TetrominoParnet);
                    var tetromino = tetrominoGo.GetComponent<Tetromino>();
                    var model = new TetrominoModel(config.shape, config.color, startPosition);
                    tetromino.Initialize(model);
                    activeTetrominos.Add(tetrominoGo);

                    if (IsPositionOccupied(model.gridPosition)) {
                        GameOver();
                    } else {
                        currentTetromino = tetromino;
                    }
                }
            });
        }
    }

    private void OnDownCurrentTetromino() {
        if (currentTetromino != null) {
            Vector2Int newPosition = currentTetromino.model.gridPosition;
            while (true) {
                Vector2Int testPosition = newPosition + new Vector2Int(0, -1);
                if (IsValidPosition(testPosition, currentTetromino.model.cells)) {
                    newPosition = testPosition;
                } else {
                    break;
                }
            }
            MoveTetromino(currentTetromino, newPosition - currentTetromino.model.gridPosition);
            AddOccupyCells(currentTetromino);
            ClearCompleteLines();
            SpawnTetromino();
        }
    }

    private void DropCurrentTetromino() {
        if (currentTetromino != null) {
            Vector2Int newPosition = currentTetromino.model.gridPosition + new Vector2Int(0, -1);
            if (IsValidPosition(newPosition, currentTetromino.model.cells)) {
                MoveTetromino(currentTetromino, new Vector2Int(0, -1));
            } else {
                AddOccupyCells(currentTetromino);
                ClearCompleteLines();
                SpawnTetromino();
            }
        }
    }

    public void MoveTetromino(Tetromino tetromino, Vector2Int direction) {
        tetromino.Move(direction);
    }

    private void AddOccupyCells(Tetromino tetromino) {
        foreach (Vector2Int cell in tetromino.model.cells) {
            var gridVector = tetromino.model.gridPosition + cell;
            occupiedCells.Add(gridVector);

            var row = gridVector.y;
            var column = gridVector.x;
            textMap[row * columns + column].color = Color.red;
        }
    }

    private bool IsValidPosition(Vector2Int position, List<Vector2Int> cells) {
        foreach (Vector2Int cell in cells) {
            Vector2Int newPos = position + cell;
            if (newPos.x < 0 ||
                newPos.x >= columns ||
                newPos.y < 0 ||
                occupiedCells.Contains(newPos)) {
                return false;
            }
        }
        return true;
    }

    private bool IsPositionOccupied(Vector2Int position) {
        return occupiedCells.Contains(position);
    }

    private void HandleInput() {
        if (currentTetromino == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            TryMoveCurrentTetromino(new Vector2Int(-1, 0));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            TryMoveCurrentTetromino(new Vector2Int(1, 0));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            OnDownCurrentTetromino();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            RotateCurrentTetromino();
        }
    }

    private void TryMoveCurrentTetromino(Vector2Int direction) {
        Vector2Int newPosition = currentTetromino.model.gridPosition + direction;
        if (IsValidPosition(newPosition, currentTetromino.model.cells)) {
            MoveTetromino(currentTetromino, direction);
        }
    }

    public void ClearTetrominos() {
        foreach (var tetromino in activeTetrominos) {
            Destroy(tetromino);
        }

        activeTetrominos.Clear();
    }

    private void RotateCurrentTetromino() {
        currentTetromino.Rotate();
        if (!IsValidPosition(currentTetromino.model.gridPosition, currentTetromino.model.cells)) {
            currentTetromino.Rotate(); // Undo rotation
            currentTetromino.Rotate();
            currentTetromino.Rotate();
        }
    }

    private void ClearCompleteLines() {
        var completeLines = new List<int>();
        for (int y = 0; y < rows; y++) {
            bool isComplete = true;
            for (int x = 0; x < columns; x++) {
                if (!occupiedCells.Contains(new Vector2Int(x, y))) {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete) {
                completeLines.Add(y);
            }
        }

        foreach (int row in completeLines) {
            for (int column = 0; column < columns; column++) {
                occupiedCells.Remove(new Vector2Int(column, row));
                textMap[row * columns + column].color = Color.white;
            }

            foreach (var tetromino in activeTetrominos) {
                tetromino.GetComponent<Tetromino>().RemoveLine(row);
            }

            var newOccupiedCells = new List<Vector2Int>();
            foreach (Vector2Int cell in occupiedCells) {
                if (cell.y > row) {
                    newOccupiedCells.Add(new Vector2Int(cell.x, cell.y - 1));
                    textMap[(cell.y - 1) * columns + cell.x].color = Color.red;
                } else {
                    newOccupiedCells.Add(cell);
                    textMap[cell.y * columns + cell.x].color = Color.red;
                }
            }
            occupiedCells = new HashSet<Vector2Int>(newOccupiedCells);
        }
    }
}
