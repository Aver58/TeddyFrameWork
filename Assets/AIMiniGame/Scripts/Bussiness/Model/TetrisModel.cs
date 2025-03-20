using System;
using System.Collections;
using System.Collections.Generic;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

public enum TetrominoType { I, O, T, S, Z, J, L }
[Serializable]
public class TetrominoData
{
    public TetrominoType type;
    public Vector2Int[,] cells; // 存放各旋转状态下的块坐标

    public TetrominoData(TetrominoType type, Vector2Int[,] cells)
    {
        this.type = type;
        this.cells = cells;
    }
}

public class TetrisModel : MonoBehaviour {
    public static TetrisModel Instance;

    // 游戏网格大小
    public int gridWidth = 10;
    public int gridHeight = 20;
    public int[,] grid; // 用于记录已固定的块

    // 定义俄罗斯方块形状字典
    public Dictionary<TetrominoType, TetrominoData> tetrominoDict;

    // 当前活跃的方块数据
    public TetrominoType currentType;
    public Vector2Int currentPosition; // 当前块的网格位置
    public int currentRotation = 0;

    public TetrisModel()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGrid();
            InitializeTetrominoes();
        }
    }

    void InitializeGrid()
    {
        grid = new int[gridWidth, gridHeight];
        // 初始化网格，0 表示空，非0 表示占用（可以存储颜色或ID）
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = 0;
            }
        }
    }

    void InitializeTetrominoes() {
        tetrominoDict = new Dictionary<TetrominoType, TetrominoData>();

        // 示例：添加一个简单的“I”形方块，定义两种旋转状态（水平、垂直）
        // 注意：实际项目中需定义所有旋转状态，以下仅为示例
        Vector2Int[,] cellsI = new Vector2Int[2, 4] {
            { new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0), new Vector2Int(3,0) },
            { new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(0,2), new Vector2Int(0,3) }
        };
        tetrominoDict.Add(TetrominoType.I, new TetrominoData(TetrominoType.I, cellsI));

        // 同理，其他类型可添加O, T, S, Z, J, L等

    }

    // 根据当前类型和旋转状态获取当前方块的相对单元格坐标数组
    public Vector2Int[] GetCurrentCells()
    {
        TetrominoData data = tetrominoDict[currentType];
        int rotationState = currentRotation % data.cells.GetLength(0);
        Vector2Int[] cells = new Vector2Int[data.cells.GetLength(1)];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = data.cells[rotationState, i];
        }
        return cells;
    }

    // 重置当前方块（随机生成）
    public void SpawnNewTetromino()
    {
        Array values = Enum.GetValues(typeof(TetrominoType));
        currentType = (TetrominoType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
        currentRotation = 0;
        // 初始位置通常在网格顶部中间
        currentPosition = new Vector2Int(gridWidth / 2 - 2, gridHeight - 1);
    }
}
