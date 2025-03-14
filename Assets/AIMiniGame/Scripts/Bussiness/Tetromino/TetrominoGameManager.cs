using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private List<Tetromino> activeTetrominos = new List<Tetromino>();

    private Dictionary<int, TextMeshProUGUI> textMap = new Dictionary<int, TextMeshProUGUI>();

    void Start() {
#if UNITY_EDITOR
        InitBG();
#endif
        StartGame();
    }

    private void InitBG() {
        ResourceManager.Instance.LoadAssetAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/GirdBg.prefab", prefab => {
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
            ResourceManager.Instance.LoadAssetAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/Tetromino.prefab", prefab => {
                if (prefab != null) {
                    var tetrominoGo = Instantiate(prefab, TetrominoParnet);
                    var tetromino = tetrominoGo.GetComponent<Tetromino>();
                    var model = new TetrominoModel(config.shape, config.color, startPosition);
                    tetromino.Initialize(model);
                    activeTetrominos.Add(tetromino);

                    if (IsPositionOccupied(model.grid)) {
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
            Vector2Int newPosition = currentTetromino.model.grid;
            while (true) {
                Vector2Int testPosition = newPosition + new Vector2Int(0, -1);
                if (IsValidPosition(testPosition, currentTetromino.model.cells)) {
                    newPosition = testPosition;
                } else {
                    break;
                }
            }
            MoveTetromino(currentTetromino, newPosition - currentTetromino.model.grid);
            AddOccupyCells(currentTetromino);
            ClearCompleteLines();
            SpawnTetromino();
        }
    }

    private void DropCurrentTetromino() {
        if (currentTetromino != null) {
            Vector2Int newPosition = currentTetromino.model.grid + new Vector2Int(0, -1);
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
            var grid = tetromino.model.grid + cell;
            occupiedCells.Add(grid);

            var column = grid.x;
            var row = grid.y;
            textMap[row * columns + column].color = Color.red;
        }
    }

    private bool IsValidPosition(Vector2Int position, List<Vector2Int> cells) {
        foreach (Vector2Int cell in cells) {
            Vector2Int newPos = position + cell;
            if (newPos.x < 0 ||
                newPos.x >= columns ||
                newPos.y < 0 ||
                IsPositionOccupied(newPos)) {
                return false;
            }
        }
        return true;
    }

    private bool IsPositionOccupied(Vector2Int grid) {
        return occupiedCells.Contains(grid);
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
        Vector2Int newPosition = currentTetromino.model.grid + direction;
        if (IsValidPosition(newPosition, currentTetromino.model.cells)) {
            MoveTetromino(currentTetromino, direction);
        }
    }

    public void ClearTetrominos() {
        foreach (var tetromino in activeTetrominos) {
            Destroy(tetromino.gameObject);
        }

        activeTetrominos.Clear();
    }

    private void RotateCurrentTetromino() {
        currentTetromino.Rotate();
        if (!IsValidPosition(currentTetromino.model.grid, currentTetromino.model.cells)) {
            currentTetromino.Rotate(); // Undo rotation
            currentTetromino.Rotate();
            currentTetromino.Rotate();
        }
    }

    private void ClearCompleteLines() {
        var completeRows = new List<int>();
        for (int row = 0; row < rows; row++) {
            bool isComplete = true;
            for (int column = 0; column < columns; column++) {
                if (!IsPositionOccupied(new Vector2Int(column, row))) {
                    isComplete = false;
                    break;
                }
            }
            if (isComplete) {
                completeRows.Add(row);
            }
        }

        foreach (int row in completeRows) {
            for (int column = 0; column < columns; column++) {
                occupiedCells.Remove(new Vector2Int(column, row));
                textMap[row * columns + column].color = Color.white;
            }

            var newOccupiedCells = new List<Vector2Int>();
            foreach (Vector2Int cell in occupiedCells) {
                if (cell.y > row) {
                    // 所有超过完成行的单元格向下移动一行
                    newOccupiedCells.Add(new Vector2Int(cell.x, cell.y - 1));
                    textMap[(cell.y - 1) * columns + cell.x].color = Color.red;
                } else {
                    newOccupiedCells.Add(cell);
                    textMap[cell.y * columns + cell.x].color = Color.red;
                }
            }
            occupiedCells = new HashSet<Vector2Int>(newOccupiedCells);

            foreach (var tetromino in activeTetrominos) {
                tetromino.RemoveRow(row);
            }
        }
    }
}
