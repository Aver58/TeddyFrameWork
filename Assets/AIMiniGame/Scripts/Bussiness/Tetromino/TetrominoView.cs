using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TetrominoView : MonoBehaviour {
    public TetrominoModel model;
    private const int cellSize = 90;
    private const int cellSpacing = 5;
    private RectTransform rectTransform;
    private Dictionary<int, GameObject> cellGoMap = new Dictionary<int, GameObject>();

    public void Initialize(TetrominoModel model) {
        this.model = model;
        rectTransform = GetComponent<RectTransform>();
        DrawShape();
        UpdateVisuals();
    }

    private void DrawShape() {
        var cells = GetCells();
        for (int i = 0; i < cells.Count; i++) {
            var cell = cells[i];
            var prefab = ResourceManager.Instance.LoadResourceSync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/TetrominoCell.prefab");
            if (prefab != null) {
                var cellGo = Instantiate(prefab, transform);
                var cellRectTransform = cellGo.GetComponent<RectTransform>();
                UpdateAnchoredPosition(cellRectTransform, cell);
                cellGo.GetComponent<Image>().color = model.color;
                cellGoMap.Add(i, cellGo);
            }
        }
    }

    private void UpdateVisuals() {
        for (int i = 0; i < GetCells().Count; i++) {
            Transform cell = transform.GetChild(i);
            UpdateAnchoredPosition(cell.GetComponent<RectTransform>(), model.cells[i]);
        }
        UpdateAnchoredPosition(rectTransform, model.grid);
    }

    private void UpdateAnchoredPosition(RectTransform rectTrans, Vector2Int grid) {
        var column = grid.x;
        var row = grid.y;
        var posX = column * cellSize + column * cellSpacing;
        var posY = row * cellSize + row * cellSpacing;
        rectTrans.anchoredPosition = new Vector2(posX, posY);
    }

    public void Move(Vector2Int direction) {
        model.Move(direction);
        UpdateAnchoredPosition(rectTransform, model.grid);
    }

    public void Rotate() {
        model.Rotate();
        UpdateVisuals();
    }

    public List<Vector2Int> GetCells() {
        return model.cells;
    }

    public GameObject GetCellGameObject(int i) {
        return cellGoMap[i];
    }
}
