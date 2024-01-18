using System.Collections.Generic;
using StarForce;
using UnityEngine;
using UnityGameFramework.Runtime;

public class PuzzleForgeController : Controller {
    public int GridRowCount = 5;
    public int GridColumnCount = 6;
    public int MinMergeCount = 3;
    public int TemplateCount = 3;
    public int DragTemplateIndex = -1;
    
    private List<int> gridLevelMap;
    private List<List<int>> gridNeighborMap;
    private List<int> templateList;

    #region Public

    public void OnInit() {
        var count = GridRowCount * GridColumnCount;
        gridLevelMap = new List<int>(count);
        gridNeighborMap = new List<List<int>>(count);
        templateList = new List<int>(TemplateCount);
        for (int i = 0; i < count; i++) {
            gridLevelMap.Add(0);
        }

        for (int i = 0; i < count; i++) {
            var neighbors = InitGridNeighbors(i);
            gridNeighborMap.Add(neighbors);
        }

        for (int i = 0; i < TemplateCount; i++) {
            var index = Random.Range(0, 2);
            Log.Debug(index);
            templateList.Add(index);
        }
    }

    public void Clear() {
        gridLevelMap = null;
    }

    public int GetGridCount() {
        return gridLevelMap.Count;
    }

    public int GetGridLevel(int index) {
        if (index < gridLevelMap.Count) {
            return gridLevelMap[index];
        }

        return -1;
    }

    public void SetGridLevel(int index, int level) {
        if (index < gridLevelMap.Count) {
            gridLevelMap[index] = level;
        }
    }

    public List<int> GetGridNeighbors(int index) {
        if (index < gridNeighborMap.Count) {
            return gridNeighborMap[index];
        }

        return null;
    }

    public List<int> GetTemplate() {
        return templateList;
    }

    public void UseOneTemplate() {
        templateList.Remove(0);
        var index = Random.Range(0, 2);
        Log.Debug(index);
        templateList.Add(index);
    }

    #endregion

    #region Private

    private List<int> InitGridNeighbors(int index) {
        var neighbors = new List<int>();
        var rowIndex = index / GridColumnCount;
        var columnIndex = index % GridColumnCount;
        
        // 上
        if (rowIndex - 1 >= 0) {
            neighbors.Add(index - GridColumnCount);
        }
        
        // 下
        if (rowIndex + 1 < GridRowCount) {
            neighbors.Add(index + GridColumnCount);
        }
        
        // 左
        if (columnIndex - 1 >= 0) {
            neighbors.Add(index - 1);
        }
        
        // 右
        if (columnIndex + 1 < GridColumnCount) {
            neighbors.Add(index + 1);
        }

        return neighbors;
    }

    #endregion
}