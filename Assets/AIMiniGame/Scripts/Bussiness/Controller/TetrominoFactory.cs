using System.Collections.Generic;
using UnityEngine;

namespace AIMiniGame.Scripts.TetrisGame {
    public static class TetrominoFactory {
        // 方块形状定义
        private static readonly Dictionary<int, int[,]> TetrominoShapes = new Dictionary<int, int[,]> {
            // I形方块
            { 0, new int[,] {
                {0, 0, 0, 0},
                {1, 1, 1, 1},
                {0, 0, 0, 0},
                {0, 0, 0, 0}
            } },
            // J形方块
            { 1, new int[,] {
                {2, 0, 0},
                {2, 2, 2},
                {0, 0, 0}
            } },
            // L形方块
            { 2, new int[,] {
                {0, 0, 3},
                {3, 3, 3},
                {0, 0, 0}
            } },
            // O形方块
            { 3, new int[,] {
                {4, 4},
                {4, 4}
            } },
            // S形方块
            { 4, new int[,] {
                {0, 5, 5},
                {5, 5, 0},
                {0, 0, 0}
            } },
            // T形方块
            { 5, new int[,] {
                {0, 6, 0},
                {6, 6, 6},
                {0, 0, 0}
            } },
            // Z形方块
            { 6, new int[,] {
                {7, 7, 0},
                {0, 7, 7},
                {0, 0, 0}
            } }
        };

        public static TetrominoData GetRandomTetromino() {
            int type = Random.Range(0, TetrominoShapes.Count);
            return new TetrominoData(type, TetrominoShapes[type]);
        }

        public static int[,] RotateMatrix(int[,] matrix) {
            int n = matrix.GetLength(0);
            int[,] result = new int[n, n];

            // 转置
            for (int i = 0; i < n; i++) {
                for (int j = 0; j < n; j++) {
                    result[i, j] = matrix[n - j - 1, i];
                }
            }

            return result;
        }
    }
}