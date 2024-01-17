using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class PuzzleForgeForm : FullScreenForm {
    public int GridRowCount;
    public int GridColumnCount;
    private int MinMergeCount = 3;

    [SerializeField] private Transform GridParent;
    [SerializeField] private GameObject GridItemTemplate;
    [SerializeField] private List<Transform> TempleteParents;
    private List<PuzzleForgeGridItem> gridItemMap;
    private PuzzleForgeController puzzleForgeController;

    protected override void OnInit(object userData) {
        base.OnInit(userData);
        
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        MinMergeCount = puzzleForgeController.MinMergeCount;
        GridRowCount = puzzleForgeController.GridRowCount;
        GridColumnCount = puzzleForgeController.GridColumnCount;
        toClearGrids = new List<PuzzleForgeGridItem>(2);
        LoadAllGrid();
    }

    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);

        foreach (var item in gridItemMap) {
            item.Clear();
        }
    }

    private void LoadAllGrid() {
        var count = puzzleForgeController.GetGridCount();
        gridItemMap = new List<PuzzleForgeGridItem>(count);
        for (int i = 0; i < count; i++) {
            var go = Instantiate(GridItemTemplate, GridParent);
            var gridItem = go.GetComponent<PuzzleForgeGridItem>();
            if (gridItem != null) {
                gridItem.Init(i, OnBtnClickGrid);
                gridItemMap.Add(gridItem);
            }
        }
    }

    private List<PuzzleForgeGridItem> toClearGrids;
    private void OnBtnClickGrid(PuzzleForgeGridItem item) {
        var count = 1;
        var index = item.Index;
        var level = item.Level;
        toClearGrids.Clear();
        // 遍历邻居
        var neighborGrids = puzzleForgeController.GetGridNeighbors(index);
        for (int i = 0; i < neighborGrids.Count; i++) {
            var gridIndex = neighborGrids[i];
            var grid = gridItemMap[gridIndex];
            if (grid.Level == level) {
                count++;
                toClearGrids.Add(grid);
                Log.Debug($"【合并】新增 {grid}");
                // 遍历邻居的邻居
                for (int j = 0; j < grid.NeighborGrids.Count; j++) {
                    var grid2Index = grid.NeighborGrids[j];
                    var grid2Level = puzzleForgeController.GetGridLevel(index);
                    if (grid2Index != index && 
                        grid2Index != grid.Index && 
                        grid2Level == level) {
                        count++;
                        var grid2 = gridItemMap[gridIndex];
                        toClearGrids.Add(grid2);
                        Log.Debug($"【合并】新增 {grid2}");
                    }
                }
            }
        }
        
        if (count >= MinMergeCount) {
            item.LevelUp();
            // 清理其他格子
            for (int i = 0; i < toClearGrids.Count; i++) {
                var grid = toClearGrids[i];
                grid.ClearLevel();
            }

            toClearGrids.Clear();
            // 死循环预警
            OnBtnClickGrid(item);
        }
    }
}
