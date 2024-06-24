using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tetromino : MonoBehaviour {
    public int[,] shape;
    public Color color;
    public Vector2Int position;
    public Vector2Int[] cells;
    private const int cellSize = 90;

    public void Initialize(int[,] shape, Color color, Vector2Int startPosition) {
        this.shape = shape;
        this.color = color;
        this.position = startPosition;
        DrawShape();
        UpdateVisuals();
    }

    private void DrawShape() {
        var cellList = new List<Vector2Int>();
        for (int y = 0; y < shape.GetLength(0); y++) {
            for (int x = 0; x < shape.GetLength(1); x++) {
                if (shape[y, x] == 1) {
                    ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/Tetromino/TetrominoCell.prefab", prefab => {
                        if (prefab != null) {
                            var cell = Instantiate(prefab, transform, true);
                            cell.transform.localPosition = new Vector3(x, -y, 0) * cellSize;
                            cell.GetComponent<Image>().color = color;
                            cellList.Add(new Vector2Int(x, -y));
                        }
                    });
                }
            }
        }

        cells = cellList.ToArray();
    }

    private void UpdateVisuals() {
        for (int i = 0; i < cells.Length; i++) {
            Transform cell = transform.GetChild(i);
            cell.localPosition = (Vector2)cells[i] * cellSize;
        }

        transform.localPosition = (Vector2)position * cellSize;
    }

    public void Move(Vector2Int direction) {
        position += direction;
        transform.localPosition = new Vector3(position.x, position.y, 0) * cellSize;
    }

    public void SetPosition(Vector2Int newPosition) {
        position = newPosition;
        transform.localPosition = (Vector2)position * cellSize;
    }

    public void Rotate() {
        for (int i = 0; i < cells.Length; i++) {
            int x = cells[i].x;
            cells[i].x = cells[i].y;
            cells[i].y = -x;
        }

        UpdateVisuals();
    }
}