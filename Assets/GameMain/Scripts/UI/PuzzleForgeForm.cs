using System.Collections.Generic;
using EventSystem;
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
        
        MessageSystem.Instance.RegisterMessage<int>(MessageTypeConst.OnGridChanged, OnGridChanged);
        
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        MinMergeCount = puzzleForgeController.MinMergeCount;
        toClearGrids = new List<PuzzleForgeGridItem>(2);
        templateItemMap = new List<PuzzleTemplateItem>(3);
        formRectTransform = transform as RectTransform;

        InitAllGrid();
        InitTemplateItem();
    }

    protected override void OnClose(bool isShutdown, object userData) {
        MessageSystem.Instance.UnregisterMessage<int>(MessageTypeConst.OnGridChanged, OnGridChanged);

        foreach (var item in gridItemMap) {
            item.Clear();
        }

        base.OnClose(isShutdown, userData);
    }

    private void OnGridChanged(Body arg1, int index) {
        if (index < gridItemMap.Count) {
            var item = gridItemMap[index];
            item.OnRefresh();
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
        var selectGridIndex = puzzleForgeController.SelectGridIndex;
        if (selectGridIndex == -1) {
            return;
        }

        var grid = gridItemMap[selectGridIndex];
        if (grid.Level > 0) {
            return;
        }

        // 检测是否消耗
        var isConsume = false;
        toClearGrids.Clear();
        var neighborGrids = puzzleForgeController.GetGridNeighbors(selectGridIndex);
        for (int i = 0; i < neighborGrids.Count; i++) {
            var neighborGridIndex = neighborGrids[i];
            var neighborGrid1 = gridItemMap[neighborGridIndex];
            
        }

        // 如果消耗了才走
        if (!isConsume) {
            return;
        }
        
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
                gridItem.Init(i);
                gridItemMap.Add(gridItem);
            }
        }
    }
}
