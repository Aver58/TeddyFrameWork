using System.Collections.Generic;
using AIMiniGame.Scripts.Bussiness.Controller;
using AIMiniGame.Scripts.Framework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AIMiniGame.Scripts.TetrisGame {
    public class TetrisView : UIViewBase {
        [SerializeField] private GridLayoutGroup gameboardGrid;
        [SerializeField] private GridLayoutGroup nextTetrominoGrid;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI linesClearedText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Button rotateButton;
        [SerializeField] private Button hardDropButton;

        private Dictionary<int, Color> blockColors = new Dictionary<int, Color> {
            { 0, Color.clear },
            { 1, Color.cyan  }, // I形方块
            { 2, Color.cyan  }, // J形方块
            { 3, Color.cyan }, // L形方块
            { 4, Color.cyan }, // O形方块
            { 5, Color.cyan }, // S形方块
            { 6, Color.cyan }, // T形方块
            { 7, Color.cyan } // Z形方块
        };

        // private Color lightGray1 = new Color(2 / 255f, 236 / 255f, 241 / 255f);
        private Color lightGray = new Color(0.078f, 0.192f, 0.31f);
        private Image[,] boardCells;
        private Image[,] nextTetrominoCells;
        private TetrisController tetrisController => Controller as TetrisController;

        public override void Init(UILayer layer) {
            base.Init(layer);
            // 设置按钮监听
            leftButton.onClick.AddListener(OnLeftButtonClick);
            rightButton.onClick.AddListener(OnRightButtonClick);
            downButton.onClick.AddListener(OnDownButtonClick);
            rotateButton.onClick.AddListener(OnRotateButtonClick);
            hardDropButton.onClick.AddListener(OnHardDropButtonClick);
        }

        protected override void OnOpen() {
            base.OnOpen();
            gameOverPanel.SetActive(false);
            // 创建游戏板网格
            InitGameboardGrid();
            // 创建预览方块区域
            InitNextTetrominoGrid();
            tetrisController.StartGame();
        }

        private void InitGameboardGrid() {
            int rows = tetrisController.TetrisModel.Rows;
            int cols = tetrisController.TetrisModel.Cols;

            // 配置网格布局
            gameboardGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gameboardGrid.constraintCount = cols;

            // 创建单元格
            boardCells = new Image[rows, cols];

            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < cols; x++) {
                    GameObject cell = new GameObject($"Cell_{y}_{x}");
                    cell.transform.SetParent(gameboardGrid.transform, false);

                    Image image = cell.AddComponent<Image>();
                    // 棋盘格背景 - 浅灰色和深灰色交替
                    image.color = lightGray;
                    boardCells[y, x] = image;
                }
            }
        }

        private void InitNextTetrominoGrid() {
            // 配置网格布局
            nextTetrominoGrid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            nextTetrominoGrid.constraintCount = 4;

            // 创建单元格
            nextTetrominoCells = new Image[4, 4];

            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    GameObject cell = new GameObject($"NextCell_{y}_{x}");
                    cell.transform.SetParent(nextTetrominoGrid.transform, false);

                    Image image = cell.AddComponent<Image>();
                    image.color = Color.clear;

                    nextTetrominoCells[y, x] = image;
                }
            }
        }

        protected override void UpdateView() {
            if (tetrisController == null)
                return;

            // 更新游戏板
            UpdateGameboard();

            // 更新当前方块
            UpdateCurrentTetromino();

            // 更新下一个方块预览
            UpdateNextTetromino();

            // 更新UI文本
            scoreText.text = $"Score: {tetrisController.TetrisModel.Score}";
            levelText.text = $"Level: {tetrisController.TetrisModel.Level}";
            linesClearedText.text = $"Lines: {tetrisController.TetrisModel.LinesCleared}";

            // 游戏结束面板
            gameOverPanel.SetActive(tetrisController.TetrisModel.IsGameOver);
        }

        private void UpdateGameboard() {
            int rows = tetrisController.TetrisModel.Rows;
            int cols = tetrisController.TetrisModel.Cols;

            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < cols; x++) {
                    int cellValue = tetrisController.TetrisModel.GameBoard[y, x];
                    if (cellValue != 0) {
                        boardCells[y, x].color = blockColors[cellValue];
                    } else {
                        // 空白处保持棋盘格背景
                        boardCells[y, x].color = lightGray;
                    }
                }
            }
        }

        private void UpdateCurrentTetromino() {
            if (tetrisController.TetrisModel.CurrentTetromino == null)
                return;

            TetrominoData tetromino = tetrisController.TetrisModel.CurrentTetromino;
            Vector2Int position = tetrisController.TetrisModel.CurrentPosition;

            int height = tetromino.Shape.GetLength(0);
            int width = tetromino.Shape.GetLength(1);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (tetromino.Shape[y, x] != 0) {
                        int boardX = position.x + x;
                        int boardY = position.y + y;

                        // 确保在游戏板范围内
                        if (boardX >= 0 && boardX < tetrisController.TetrisModel.Cols && boardY >= 0 && boardY < tetrisController.TetrisModel.Rows) {
                            boardCells[boardY, boardX].color = blockColors[tetromino.Shape[y, x]];
                        }
                    }
                }
            }
        }

        private void UpdateNextTetromino() {
            if (tetrisController.TetrisModel.NextTetromino == null)
                return;

            // 先清除预览区
            for (int y = 0; y < 4; y++) {
                for (int x = 0; x < 4; x++) {
                    nextTetrominoCells[y, x].color = Color.clear;
                }
            }

            // 绘制下一个方块
            TetrominoData nextTetromino = tetrisController.TetrisModel.NextTetromino;
            int height = nextTetromino.Shape.GetLength(0);
            int width = nextTetromino.Shape.GetLength(1);

            for (int y = 0; y < height; y++) {
                for (int x = 0; x < width; x++) {
                    if (nextTetromino.Shape[y, x] != 0) {
                        nextTetrominoCells[y, x].color = blockColors[nextTetromino.Shape[y, x]];
                    }
                }
            }
        }

        // 按钮事件处理
        public void OnLeftButtonClick() {
            tetrisController.MoveLeft();
        }

        public void OnRightButtonClick() {
            tetrisController.MoveRight();
        }

        public void OnDownButtonClick() {
            tetrisController.MoveDown();
        }

        public void OnRotateButtonClick() {
            tetrisController.RotateTetromino();
        }

        public void OnHardDropButtonClick() {
            tetrisController.HardDrop();
        }

        // 键盘输入处理
        private void Update() {
            if (tetrisController == null) {
                return;
            }

            if (tetrisController.TetrisModel.IsGameOver)
                return;

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                tetrisController.MoveLeft();
            } else if (Input.GetKeyDown(KeyCode.RightArrow)) {
                tetrisController.MoveRight();
            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                tetrisController.MoveDown();
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                tetrisController.RotateTetromino();
            } else if (Input.GetKeyDown(KeyCode.Space)) {
                tetrisController.HardDrop();
            }
        }
    }
}