using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetromino : MonoBehaviour {
    public TetrominoModel model;

    private const int cellSize = 90;
    private const int cellSpacing = 5;
    private RectTransform rectTransform;

    public void Initialize(TetrominoModel model) {
        this.model = model;
        rectTransform = GetComponent<RectTransform>();
        DrawShape();
        UpdateVisuals();
    }

    private void DrawShape() {
        foreach (Vector2Int cell in model.cells) {
            var prefab = ResourceManager.Instance.LoadResourceSync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/TetrominoCell.prefab");
            if (prefab != null) {
                var cellGo = Instantiate(prefab, transform);
                var cellRectTransform = cellGo.GetComponent<RectTransform>();
                UpdateAnchoredPosition(cellRectTransform, cell);
                cellGo.GetComponent<Image>().color = model.color;
            }
        }
    }

    private void UpdateVisuals() {
        for (int i = 0; i < model.cells.Count; i++) {
            Transform cell = transform.GetChild(i);
            UpdateAnchoredPosition(cell.GetComponent<RectTransform>(), model.cells[i]);
        }
        UpdateAnchoredPosition(rectTransform, model.gridPosition);
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
        UpdateAnchoredPosition(rectTransform, model.gridPosition);
    }

    public void SetPosition(Vector2Int newPosition) {
        model.SetPosition(newPosition);
        UpdateAnchoredPosition(rectTransform, model.gridPosition);
    }

    public void Rotate() {
        model.Rotate();
        UpdateVisuals();
    }

    public void RemoveLine(int row) {
        //??
        model.RemoveLine(row);
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        DrawShape();
        UpdateVisuals();
    }
}
