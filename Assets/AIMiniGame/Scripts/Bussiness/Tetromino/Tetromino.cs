using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetromino : MonoBehaviour {
    public TetrominoModel model;
    private const int cellSize = 90;
    private const int cellSpacing = 5;
    private RectTransform rectTransform;
    private Dictionary<Vector2Int, GameObject> cellGoMap = new Dictionary<Vector2Int, GameObject>();

    public void Initialize(TetrominoModel model) {
        this.model = model;
        rectTransform = GetComponent<RectTransform>();
        DrawShape();
        UpdateVisuals();
    }

    private void DrawShape() {
        foreach (Vector2Int cell in model.cells) {
            ResourceManager.Instance.LoadAssetAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/TetrominoCell.prefab", obj => {
                if (obj != null) {
                    var cellGo = Instantiate(obj, transform);
                    var cellRectTransform = cellGo.GetComponent<RectTransform>();
                    UpdateAnchoredPosition(cellRectTransform, cell);
                    cellGo.GetComponent<Image>().color = model.color;
                    cellGo.name = cell.ToString();
                    cellGoMap.Add(cell, cellGo);
                }
            });
        }
    }

    private void UpdateVisuals() {
        for (int i = 0; i < model.cells.Count; i++) {
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

    public void RemoveRow(int row) {
        model.RemoveRow(row);
        for (int i = 0; i < model.cells.Count; i++) {
            var cell = model.cells[i];
            if (cellGoMap.ContainsKey(cell)) {
                var cellGo = cellGoMap[cell];
                Destroy(cellGo);
                cellGoMap.Remove(cell);
                Debug.LogError($" Destroy Cell {cell} {cellGo}");
            }
        }

        UpdateVisuals();
    }
}
