using System.Collections.Generic;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

namespace AIMiniGame.Scripts.TetrisGame {
    public class TetrisModel : ModelBase {
        private readonly int _rows = 20;
        private readonly int _cols = 10;

        private int[,] _gameBoard;
        private int _score;
        private int _level;
        private int _linesCleared;
        private bool _isGameOver;
        private TetrominoData _currentTetromino;
        private TetrominoData _nextTetromino;
        private Vector2Int _currentPosition;

        public int[,] GameBoard {
            get => _gameBoard;
            set {
                _gameBoard = value;
                RaisePropertyChanged();
            }
        }

        public int Score {
            get => _score;
            set {
                _score = value;
                RaisePropertyChanged();
            }
        }

        public int Level {
            get => _level;
            set {
                _level = value;
                RaisePropertyChanged();
            }
        }

        public int LinesCleared {
            get => _linesCleared;
            set {
                _linesCleared = value;
                RaisePropertyChanged();
            }
        }

        public bool IsGameOver {
            get => _isGameOver;
            set {
                _isGameOver = value;
                RaisePropertyChanged();
            }
        }

        public TetrominoData CurrentTetromino {
            get => _currentTetromino;
            set {
                _currentTetromino = value;
                RaisePropertyChanged();
            }
        }

        public TetrominoData NextTetromino {
            get => _nextTetromino;
            set {
                _nextTetromino = value;
                RaisePropertyChanged();
            }
        }

        public Vector2Int CurrentPosition {
            get => _currentPosition;
            set {
                _currentPosition = value;
                RaisePropertyChanged();
            }
        }

        public int Rows => _rows;
        public int Cols => _cols;

        public TetrisModel() {
            ResetGame();
        }

        public void ResetGame() {
            GameBoard = new int[_rows, _cols];
            Score = 0;
            Level = 1;
            LinesCleared = 0;
            IsGameOver = false;
        }
    }

    // 方块数据结构
    public class TetrominoData {
        public int[,] Shape { get; set; }
        public int Type { get; set; }

        public TetrominoData(int type, int[,] shape) {
            Type = type;
            Shape = shape;
        }
    }
}