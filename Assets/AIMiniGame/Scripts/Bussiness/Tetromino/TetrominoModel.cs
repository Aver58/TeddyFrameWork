using System.Collections.Generic;
using UnityEngine;

public class TetrominoModel {
    public int[,] shape;
    public Color color;
    public Vector2Int grid;
    public List<Vector2Int> cells = new List<Vector2Int>();

    public TetrominoModel(int[,] shape, Color color, Vector2Int start) {
        this.shape = shape;
        this.color = color;
        grid = start;
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
        grid += direction;
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
}