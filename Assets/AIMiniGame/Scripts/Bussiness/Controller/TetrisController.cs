using System.Collections;
using UnityEngine;
using AIMiniGame.Scripts.Framework.UI;
using AIMiniGame.Scripts.TetrisGame;

namespace AIMiniGame.Scripts.Bussiness.Controller {
    public class TetrisController : ControllerBase {
        public TetrisModel TetrisModel { get; private set; }
        private float _fallTime;
        private float _fallTimeDefault = 1.0f;
        private Coroutine _gameLoopCoroutine;
        private float timer = 0;

        public TetrisController() {
            TetrisModel = new TetrisModel();
            Model = TetrisModel;
            _fallTime = _fallTimeDefault;

            // 订阅Model属性变化事件
            TetrisModel.PropertyChanged += (sender, args) => NotifyModelChanged();

            UpdateRegister.Instance.RegisterUpdate(OnUpdate);
        }

        private void OnUpdate(float delta) {
            GameLoop(delta);
        }

        public void StartGame() {
            Debug.Log("Game Started!");
            TetrisModel.ResetGame();
            SpawnNewTetromino();
            SpawnNextTetromino();
        }

        private void GameLoop(float delta) {
            if (TetrisModel.IsGameOver) {
                return;
            }

            timer += delta;
            if (timer < _fallTime) {
                return;
            }

            timer = 0;
            // 向下移动
            if (!MoveTetromino(0, 1)) {
                // 不能下落了，固定当前方块
                PlaceTetromino();
                // 检查是否有可以消除的行
                int linesCleared = ClearLines();
                if (linesCleared > 0) {
                    UpdateScore(linesCleared);
                }

                // 生成新方块
                if (!SpawnNewTetromino()) {
                    TetrisModel.IsGameOver = true;
                }
            }
        }

        public void MoveLeft() {
            MoveTetromino(-1, 0);
        }

        public void MoveRight() {
            MoveTetromino(1, 0);
        }

        public void MoveDown() {
            if (!MoveTetromino(0, 1)) {
                PlaceTetromino();
                // 检查是否有可以消除的行
                int linesCleared = ClearLines();
                if (linesCleared > 0) {
                    UpdateScore(linesCleared);
                }

                // 生成新方块
                if (!SpawnNewTetromino()) {
                    TetrisModel.IsGameOver = true;
                }
            }
        }

        public void HardDrop() {
            while (MoveTetromino(0, 1)) {
            }

            PlaceTetromino();

            // 检查是否有可以消除的行
            int linesCleared = ClearLines();
            if (linesCleared > 0) {
                UpdateScore(linesCleared);
            }

            // 生成新方块
            if (!SpawnNewTetromino()) {
                TetrisModel.IsGameOver = true;
            }
        }

        public void RotateTetromino() {
            TetrominoData currentTetromino = TetrisModel.CurrentTetromino;
            int[,] rotatedShape = TetrominoFactory.RotateMatrix(currentTetromino.Shape);

            // 临时备份当前方块数据
            int[,] originalShape = currentTetromino.Shape;

            // 尝试旋转
            currentTetromino.Shape = rotatedShape;

            // 检查旋转后是否有效
            if (!IsValidPosition(TetrisModel.CurrentPosition)) {
                // 回退到原始状态
                currentTetromino.Shape = originalShape;
                return;
            }

            // 应用旋转并通知视图更新
            TetrisModel.CurrentTetromino = new TetrominoData(currentTetromino.Type, rotatedShape);
        }

        private bool MoveTetromino(int offsetX, int offsetY) {
            Vector2Int newPosition = new Vector2Int(TetrisModel.CurrentPosition.x + offsetX, TetrisModel.CurrentPosition.y + offsetY);

            // 检查新位置是否有效
            if (IsValidPosition(newPosition)) {
                TetrisModel.CurrentPosition = newPosition;
                return true;
            }

            return false;
        }

        private void PlaceTetromino() {
            TetrominoData tetromino = TetrisModel.CurrentTetromino;
            Vector2Int position = TetrisModel.CurrentPosition;

            int height = tetromino.Shape.GetLength(0);
            int width = tetromino.Shape.GetLength(1);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (tetromino.Shape[y, x] != 0) {
                        int boardX = position.x + x;
                        int boardY = position.y + y;

                        // 确保在游戏板范围内
                        if (IsInBounds(boardX, boardY)) {
                            TetrisModel.GameBoard[boardY, boardX] = tetromino.Shape[y, x];
                        }
                    }
                }
            }
        }

        private int ClearLines() {
            int linesCleared = 0;

            for (int y = 0; y < TetrisModel.Rows; y++) {
                bool isLineFull = true;

                for (int x = 0; x < TetrisModel.Cols; x++) {
                    if (TetrisModel.GameBoard[y, x] == 0) {
                        isLineFull = false;
                        break;
                    }
                }

                if (isLineFull) {
                    // 消除该行并将上面的所有行下移
                    for (int yy = y; yy > 0; yy--) {
                        for (int x = 0; x < TetrisModel.Cols; x++) {
                            TetrisModel.GameBoard[yy, x] = TetrisModel.GameBoard[yy - 1, x];
                        }
                    }

                    // 顶行置为空
                    for (int x = 0; x < TetrisModel.Cols; x++) {
                        TetrisModel.GameBoard[0, x] = 0;
                    }

                    linesCleared++;
                }
            }

            if (linesCleared > 0) {
                TetrisModel.LinesCleared += linesCleared;
                // 触发GameBoard的属性变更通知
                TetrisModel.GameBoard = TetrisModel.GameBoard;
            }

            return linesCleared;
        }

        private void UpdateScore(int linesCleared) {
            // 分数计算逻辑
            int[] scoreMultiplier = { 0, 40, 100, 300, 1200 }; // 0,1,2,3,4行的分数
            TetrisModel.Score += scoreMultiplier[linesCleared] * TetrisModel.Level;

            // 每清除10行提升一级
            if (TetrisModel.LinesCleared / 10 > (TetrisModel.LinesCleared - linesCleared) / 10) {
                TetrisModel.Level++;
                _fallTime = _fallTimeDefault / (1 + (TetrisModel.Level - 1) * 0.1f);
            }
        }

        private bool SpawnNewTetromino() {
            if (TetrisModel.NextTetromino != null) {
                TetrisModel.CurrentTetromino = TetrisModel.NextTetromino;
            } else {
                TetrisModel.CurrentTetromino = TetrominoFactory.GetRandomTetromino();
            }

            SpawnNextTetromino();

            // 设置初始位置（居中，顶部）
            int startX = TetrisModel.Cols / 2 - TetrisModel.CurrentTetromino.Shape.GetLength(1) / 2;
            TetrisModel.CurrentPosition = new Vector2Int(startX, 0);

            // 检查新方块位置是否有效，如果无效则游戏结束
            return IsValidPosition(TetrisModel.CurrentPosition);
        }

        private void SpawnNextTetromino() {
            TetrisModel.NextTetromino = TetrominoFactory.GetRandomTetromino();
        }

        private bool IsValidPosition(Vector2Int position) {
            TetrominoData tetromino = TetrisModel.CurrentTetromino;
            int height = tetromino.Shape.GetLength(0);
            int width = tetromino.Shape.GetLength(1);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (tetromino.Shape[y, x] != 0) {
                        int boardX = position.x + x;
                        int boardY = position.y + y;

                        // 检查是否超出边界
                        if (!IsInBounds(boardX, boardY)) {
                            return false;
                        }

                        // 检查是否与已有方块重叠
                        if (TetrisModel.GameBoard[boardY, boardX] != 0) {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool IsInBounds(int x, int y) {
            return x >= 0 && x < TetrisModel.Cols && y >= 0 && y < TetrisModel.Rows;
        }
    }
}