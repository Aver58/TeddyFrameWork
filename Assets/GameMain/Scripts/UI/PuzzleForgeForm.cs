using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityGameFramework.Runtime;

public class PuzzleForgeForm : FullScreenForm {
    [SerializeField] private Transform GridParent;
    [SerializeField] private GameObject GridItemTemplate;

    [SerializeField] private GameObject TemplateItemTemplate;
    [SerializeField] private List<Transform> TemplateParents;

    private int MinMergeCount = 3;
    private RectTransform formRectTransform;

    private PuzzleForgeController puzzleForgeController;
    private List<PuzzleForgeGridItem> gridItemMap;
    private List<PuzzleTemplateItem> templateItemMap;
    private List<PuzzleForgeGridItem> toClearGrids;
    private List<PuzzleTemplateItem> templateItems;

    protected override void OnInit(object userData) {
        base.OnInit(userData);
        
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        MinMergeCount = puzzleForgeController.MinMergeCount;
        toClearGrids = new List<PuzzleForgeGridItem>(2);
        templateItemMap = new List<PuzzleTemplateItem>(3);
        formRectTransform = transform as RectTransform;

        InitAllGrid();
        InitTemplateItem();
    }

    protected override void OnClose(bool isShutdown, object userData) {
        base.OnClose(isShutdown, userData);

        foreach (var item in gridItemMap) {
            item.Clear();
        }
    }

    private void InitTemplateItem() {
        var data = puzzleForgeController.GetTemplate();
        for (int i = 0; i < data.Count; i++) {
            var index = data[i];
            if (index < TemplateParents.Count) {
                var parent = TemplateParents[i];
                var go = Instantiate(TemplateItemTemplate, parent);
                var item = go.GetComponent<PuzzleTemplateItem>();
                if (item != null) {
                    item.Init(i, index, formRectTransform, OnEndDragTemplateItem);
                    templateItemMap.Add(item);
                }
            } else {
                Log.Error($"【输入模具】索引越界 index {index} TemplateParents.Count {TemplateParents.Count}");
            }
        }
    }

    private void OnEndDragTemplateItem() {
        Log.Debug($"OnEndDragTemplateItem");
        return;
        // 如果消耗了才走
        puzzleForgeController.UseOneTemplate();
        var data = puzzleForgeController.GetTemplate();
        for (int i = 0; i < templateItemMap.Count; i++) {
            var item = templateItemMap[i];
            var index = data[i];
            item.Init(i, index, formRectTransform, OnEndDragTemplateItem);
        }
    }

    private void InitAllGrid() {
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
                var neighborGrids2 = puzzleForgeController.GetGridNeighbors(gridIndex);
                for (int j = 0; j < neighborGrids2.Count; j++) {
                    var grid2Index = neighborGrids2[j];
                    var grid2 = gridItemMap[grid2Index];
                    if (grid2Index != index &&
                        grid2Index != grid.Index && 
                        grid2.Level == level) {
                        count++;
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
