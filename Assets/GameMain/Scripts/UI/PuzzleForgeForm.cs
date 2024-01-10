using System.Collections.Generic;
using UnityEngine;

public class PuzzleForgeForm : FullScreenForm {
    private const int GridRowCount = 5;
    private const int GridColumnCount = 6;
    [SerializeField] private Transform GridParent;
    [SerializeField] private GameObject GridItemTemplate;
    
    // todo 数据层
    // PuzzleForgeController
    private List<PuzzleForgeItem> gridItemMap;

    protected override void OnInit(object userData) {
        base.OnInit(userData);
        LoadAllGrid();
    }

    private void LoadAllGrid() {
        gridItemMap = new List<PuzzleForgeItem>(GridRowCount * GridColumnCount);
        
        for (int row = 0; row < GridRowCount; row++) {
            for (int column = 0; column < GridColumnCount; column++) {
                var go = Instantiate(GridItemTemplate, GridParent);
                var gridItem = go.GetComponent<PuzzleForgeItem>();
                if (gridItem != null) {
                    gridItem.Init(row, column, GetIndex(row, column));
                    gridItemMap.Add(gridItem);
                }
            }
        }
        
        // 初始化格子邻居
        for (int i = 0; i < gridItemMap.Count; i++) {
            var item = gridItemMap[i];
            InitAllNeighbor(item);
        }
    }

    private int GetIndex(int rowIndex, int columnIndex) {
        return rowIndex * GridColumnCount + columnIndex;
    }
    
    private void InitAllNeighbor(PuzzleForgeItem item) {
        var rowIndex = item.RowIndex;
        var columnIndex = item.ColumnIndex;
        // 上
        if (rowIndex - 1 >= 0) {
            var upItem = gridItemMap[GetIndex(rowIndex - 1, columnIndex)];
            item.AddNeighbor(upItem);
        }
        
        // 下
        if (rowIndex + 1 < GridRowCount) {
            var downItem = gridItemMap[GetIndex(rowIndex + 1, columnIndex)];
            item.AddNeighbor(downItem);
        }
        
        // 左
        if (columnIndex - 1 >= 0) {
            var leftItem = gridItemMap[GetIndex(rowIndex, columnIndex - 1)];
            item.AddNeighbor(leftItem);
        }
        
        // 右
        if (columnIndex + 1 < GridColumnCount) {
            var rightItem = gridItemMap[GetIndex(rowIndex, columnIndex + 1)];
            item.AddNeighbor(rightItem);
        }
    }
}
