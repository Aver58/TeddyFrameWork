using System.Collections.Generic;
using System.Linq;
using EventSystem;
using StarForce;
using UnityEngine;
using UnityGameFramework.Runtime;

public class PuzzleForgeController : Controller {
    public int GridRowCount = 5;
    public int GridColumnCount = 6;
    public int MinMergeCount = 3;           // 资源最小合并数量
    public int MinMergeTemplateCount = 2;   // 模具最小合并数量
    public int TemplateCount = 3;
    public int DragTemplateIndex = -1;
    public int SelectGridIndex = -1;
    
    private List<int> gridLevelMap;
    private List<List<int>> gridNeighborMap;
    private List<int> templateList;
    private List<int> toClearGridIndexs;
    private HashSet<int> changedGrids;

    #region Public

    public void OnInit() {
        var count = GridRowCount * GridColumnCount;
        toClearGridIndexs = new List<int>(2);
        gridLevelMap = new List<int>(count);
        gridNeighborMap = new List<List<int>>(count);
        templateList = new List<int>(TemplateCount);
        changedGrids = new HashSet<int>();
        
        for (int i = 0; i < count; i++) {
            gridLevelMap.Add(0);
        }

        for (int i = 0; i < count; i++) {
            var neighbors = InitGridNeighbors(i);
            gridNeighborMap.Add(neighbors);
        }

        for (int i = 0; i < TemplateCount; i++) {
            var index = Random.Range(0, 3);
            templateList.Add(index);
        }
    }

    public void Clear() {
        gridLevelMap = null;
    }

    public int GetGridCount() {
        return gridLevelMap.Count;
    }

    public int GetGridLevel(int gridIndex) {
        if (gridIndex < gridLevelMap.Count) {
            return gridLevelMap[gridIndex];
        }

        return -1;
    }

    public void SetGridLevel(int gridIndex, int level) {
        if (gridIndex < gridLevelMap.Count) {
            gridLevelMap[gridIndex] = level;
        }
    }

    public void AddGridLevel(int gridIndex) {
        if (gridIndex < gridLevelMap.Count) {
            gridLevelMap[gridIndex]++;
        }
    }
    
    public List<int> GetGridNeighbors(int gridIndex) {
        if (gridIndex < gridNeighborMap.Count) {
            return gridNeighborMap[gridIndex];
        }

        return null;
    }

    public List<int> GetTemplate() {
        return templateList;
    }

    public void UseOneTemplate() {
        templateList.Remove(0);
        var index = Random.Range(0, 2);
        templateList.Add(index);
    }

    public bool IsDragTemplate() {
        return DragTemplateIndex != -1; 
    }

    // 请求放置材料
    public void RequestPlaceGrid(int gridIndex) {
        var gridLevel = GetGridLevel(gridIndex);
        if (gridLevel > 0) {
            return;
        }

        changedGrids.Clear();
        AddGridLevel(gridIndex);
        changedGrids.Add(gridIndex);
        RequestLevelUpGrid(gridIndex);
    }

    // 请求放置模具
    public void RequestPlaceTemplate(int gridIndex, int templateIndex) {
        if (templateIndex == -1) {
            return;
        }

        var level = GetGridLevel(gridIndex);
        if (level > 0) {
            return;
        }

        // 检测是否消耗
        var isConsume = false;
        toClearGridIndexs.Clear();
        var neighborGrids = GetGridNeighbors(gridIndex);
        // 遍历树？
        for (int i = 0; i < neighborGrids.Count; i++) {
            var neighborGridIndex = neighborGrids[i];
            var neighborGrid1 = GetGridNeighbors(neighborGridIndex);

        }

        // 如果消耗了才走
        if (!isConsume) {
            return;
        }

        // UseOneTemplate();
        // var data = GetTemplate();
        // for (int i = 0; i < templateItemMap.Count; i++) {
        //     var item = templateItemMap[i];
        //     var index = data[i];
        //     item.Init(i, index, formRectTransform, OnEndDragTemplateItem);
        // }
    }

    #endregion

    #region Private

    private List<int> InitGridNeighbors(int gridIndex) {
        var neighbors = new List<int>();
        var rowIndex = gridIndex / GridColumnCount;
        var columnIndex = gridIndex % GridColumnCount;
        
        // 上
        if (rowIndex - 1 >= 0) {
            neighbors.Add(gridIndex - GridColumnCount);
        }
        
        // 下
        if (rowIndex + 1 < GridRowCount) {
            neighbors.Add(gridIndex + GridColumnCount);
        }
        
        // 左
        if (columnIndex - 1 >= 0) {
            neighbors.Add(gridIndex - 1);
        }
        
        // 右
        if (columnIndex + 1 < GridColumnCount) {
            neighbors.Add(gridIndex + 1);
        }

        return neighbors;
    }

    private void RequestForgeGridTemplate(int gridIndex) {

    }

    private void RequestLevelUpGrid(int gridIndex) {
        var gridLevel = GetGridLevel(gridIndex);
        var count = 1;
        toClearGridIndexs.Clear();
        var neighborGrids = GetGridNeighbors(gridIndex);
        for (int i = 0; i < neighborGrids.Count; i++) {
            var neighborGridIndex = neighborGrids[i];
            var neighborGridLevel = GetGridLevel(neighborGridIndex);
            if (neighborGridLevel == gridLevel) {
                count++;
                toClearGridIndexs.Add(neighborGridIndex);
                Log.Debug($"【合并】新增 {neighborGridIndex}");
                // 遍历邻居的邻居
                var neighborGrids2 = GetGridNeighbors(neighborGridIndex);
                for (int j = 0; j < neighborGrids2.Count; j++) {
                    var neighborGridIndex2 = neighborGrids2[j];
                    var neighborGridLevel2 = GetGridLevel(neighborGridIndex2);
                    if (neighborGridIndex2 != gridIndex &&
                        neighborGridIndex2 != neighborGridIndex && 
                        neighborGridLevel2 == gridLevel) {
                        count++;
                        toClearGridIndexs.Add(neighborGridIndex2);
                        Log.Debug($"【合并】新增 {neighborGridIndex2}");
                    }
                }
            }
        }
        
        if (count >= MinMergeCount) {
            AddGridLevel(gridIndex);
            changedGrids.Add(gridIndex);
            // 清理其他格子
            for (int i = 0; i < toClearGridIndexs.Count; i++) {
                var index = toClearGridIndexs[i];
                SetGridLevel(index, 0);
                changedGrids.Add(index);
            }

            toClearGridIndexs.Clear();
            // 死循环预警
            RequestLevelUpGrid(gridIndex);
        }
        
        // 同步view层
        foreach (var index in changedGrids) {
            MessageSystem.Instance.DispatchMessage(MessageTypeConst.OnGridChanged, index);
        }
    }
    
    #endregion
}