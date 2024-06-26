using System.Collections.Generic;
using UnityEngine;

public class TetrominoModel {
    public int[,] shape;
    public Color color;
    public Vector2Int gridPosition;
    public List<Vector2Int> cells = new List<Vector2Int>();

    public TetrominoModel(int[,] shape, Color color, Vector2Int startPosition) {
        this.shape = shape;
        this.color = color;
        gridPosition = startPosition;
        InitializeCells();
    }

    private void InitializeCells() {
        cells.Clear();
        for (int row = 0; row < shape.GetLength(0); row++) {
            for (int column = 0; column < shape.GetLength(1); column++) {
                if (shape[row, column] == 1) {
                    cells.Add(new Vector2Int(column, row));
                }
            }
        }
    }

    public void Move(Vector2Int direction) {
        gridPosition += direction;
    }

    public void SetPosition(Vector2Int newPosition) {
        gridPosition = newPosition;
    }

    public void Rotate() {
        for (int i = 0; i < cells.Count; i++) {
            var cell = cells[i];
            int x = cell.x;
            cell.x = cell.y;
            cell.y = -x;
            cells[i] = cell;
        }
    }

    public void RemoveLine(int row) {
        var newCells = new List<Vector2Int>();
        for (int i = cells.Count - 1; i >= 0; i--) {
            var cell = cells[i];
            if (cell.y == row) {
                cells.RemoveAt(i);
            } else {
                newCells.Add(cell.y > row ? new Vector2Int(cell.x, cell.y - 1) : cell);
            }
        }
        cells = newCells;
    }
}