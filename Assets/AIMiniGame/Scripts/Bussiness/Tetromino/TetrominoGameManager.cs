using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TetrominoGameManager : MonoBehaviour {
    private int rows = 20;
    private int columns = 10;
    private float dropInterval = 1.0f;
    private float dropTimer = 0.0f;
    private const int cellSize = 90;
    private const int cellSpacing = 5;
    public Transform TetrominoParnet;
    public Transform GridLayoutBG;
    private TetrominoView currentTetrominoView;
    private Vector2Int spawnPosition = new Vector2Int(5, 20);
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();
    private List<TetrominoView> activeTetrominos = new List<TetrominoView>();
    public Dictionary<Vector2Int, GameObject> gridCells = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<int, Image> gridMap = new Dictionary<int, Image>();

    void Start() {
        InitGrid();
        StartGame();
    }

    private void InitGrid() {
        ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/GirdBg.prefab", prefab => {
            if (prefab != null) {
                for (int row = 0; row < rows; row++) {
                    for (int column = 0; column < columns; column++) {
                        var cell = Instantiate(prefab, GridLayoutBG);
                        var textComponent = cell.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                        textComponent.text = $"{column},{row}";
                        gridMap.Add(row * columns + column, cell.GetComponent<Image>());
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
                    var tetromino = tetrominoGo.GetComponent<TetrominoView>();
                    var model = new TetrominoModel(config.shape, config.color, startPosition);
                    tetromino.Initialize(model);
                    activeTetrominos.Add(tetromino);

                    if (IsPositionOccupied(model.grid)) {
                        GameOver();
                    } else {
                        currentTetrominoView = tetromino;
                    }
                }
            });
        }
    }

    private void OnDownCurrentTetromino() {
        if (currentTetrominoView != null) {
            Vector2Int newPosition = currentTetrominoView.model.grid;
            while (true) {
                Vector2Int testPosition = newPosition + new Vector2Int(0, -1);
                if (IsValidPosition(testPosition, currentTetrominoView.model.cells)) {
                    newPosition = testPosition;
                } else {
                    break;
                }
            }
            MoveTetromino(currentTetrominoView, newPosition - currentTetrominoView.model.grid);
            AddOccupyCells(currentTetrominoView);
            ClearCompleteLines();
            SpawnTetromino();
        }
    }

    private void DropCurrentTetromino() {
        if (currentTetrominoView != null) {
            Vector2Int newPosition = currentTetrominoView.model.grid + new Vector2Int(0, -1);
            if (IsValidPosition(newPosition, currentTetrominoView.model.cells)) {
                MoveTetromino(currentTetrominoView, new Vector2Int(0, -1));
            } else {
                AddOccupyCells(currentTetrominoView);
                ClearCompleteLines();
                SpawnTetromino();
            }
        }
    }

    public void MoveTetromino(TetrominoView tetrominoView, Vector2Int direction) {
        tetrominoView.Move(direction);
    }

    private void AddOccupyCells(TetrominoView tetrominoView) {
        var cells = currentTetrominoView.GetCells();
        for (int i = 0; i < cells.Count; i++) {
            var cell = cells[i];
            var gridPos = currentTetrominoView.model.grid + cell;
            occupiedCells.Add(gridPos);

            var cellGo = currentTetrominoView.GetCellGameObject(i);
            cellGo.transform.SetParent(TetrominoParnet);
            cellGo.GetComponent<RectTransform>().anchoredPosition = GetCellPosition(gridPos);
            cellGo.name = gridPos.ToString();
            // gridCells[gridPos] = cellGo;
        }

        Destroy(currentTetrominoView.gameObject);
        currentTetrominoView = null;
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
        if (currentTetrominoView == null) return;

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
        Vector2Int newPosition = currentTetrominoView.model.grid + direction;
        if (IsValidPosition(newPosition, currentTetrominoView.model.cells)) {
            MoveTetromino(currentTetrominoView, direction);
        }
    }

    public void ClearTetrominos() {
        foreach (var tetromino in activeTetrominos) {
            Destroy(tetromino.gameObject);
        }

        activeTetrominos.Clear();
    }

    private void RotateCurrentTetromino() {
        currentTetrominoView.Rotate();
        if (!IsValidPosition(currentTetrominoView.model.grid, currentTetrominoView.model.cells)) {
            currentTetrominoView.Rotate(); // Undo rotation
            currentTetrominoView.Rotate();
            currentTetrominoView.Rotate();
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
                var gridPos = new Vector2Int(column, row);
                occupiedCells.Remove(gridPos);
                // gridCells.Remove(gridPos);
            }

            var newOccupiedCells = new List<Vector2Int>();
            // 所有超过完成行的单元格向下移动一行
            foreach (Vector2Int cell in occupiedCells) {
                if (cell.y > row) {
                    var newCell = new Vector2Int(cell.x, cell.y - 1);
                    newOccupiedCells.Add(newCell);
                    // todo 会存在 cell.y - 1 格也有格子，导致覆盖数据，原先格子的数据变成野指针
                    // var cellGo = gridCells[cell];
                    // gridCells.Remove(cell);
                    // gridCells[newCell] = cellGo;
                    // cellGo.name = newCell.ToString();
                    // cellGo.GetComponent<RectTransform>().anchoredPosition = GetCellPosition(newCell);
                } else {
                    newOccupiedCells.Add(cell);
                }
            }
            occupiedCells = new HashSet<Vector2Int>(newOccupiedCells);
        }
    }

    private Vector2 GetCellPosition(Vector2Int grid) {
        var column = grid.x;
        var row = grid.y;
        var posX = column * cellSize + column * cellSpacing;
        var posY = row * cellSize + row * cellSpacing;
        return new Vector2(posX, posY);
    }
}
