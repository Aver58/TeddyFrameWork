using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public class PuzzleForgeGridItem : MonoBehaviour {
    private int GridRowCount;
    private int GridColumnCount;
    public int Level;  
    public int Index;
    public int RowIndex;
    public int ColumnIndex;
    public List<int> NeighborGrids;

    private PuzzleForgeController puzzleForgeController;
    private Action<PuzzleForgeGridItem> onBtnClickGrid;
    [SerializeField] private Image ImgBg;
    [SerializeField] private Image ImgIcon;
    [SerializeField] private Text TxtLevel;
    [SerializeField] private Button BtnClick;

    #region Public

    public void Init(int index, Action<PuzzleForgeGridItem> onBtnClickGrid) {
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        GridRowCount = puzzleForgeController.GridRowCount;
        GridColumnCount = puzzleForgeController.GridColumnCount;
        
        Level = puzzleForgeController.GetGridLevel(index);
        Index = index;
        RowIndex = index / GridColumnCount;
        ColumnIndex = index % GridColumnCount;
        this.onBtnClickGrid = onBtnClickGrid;
#if UNITY_EDITOR
        gameObject.name = ToString();
#endif
        NeighborGrids = puzzleForgeController.GetGridNeighbors(index);
        ImgIcon.enabled = false;
        TxtLevel.text = "";
        BtnClick.onClick.AddListener(OnBtnClick);
    }

    public void Clear() {
        BtnClick.onClick.RemoveListener(OnBtnClick);
    }

    public override string ToString() {
        var s = $"index:{Index} 行:{RowIndex + 1} 列:{ColumnIndex + 1}";
        return s;
    }

    public void LevelUp() {
        Level++;
        TxtLevel.text = Level.ToString();
        puzzleForgeController.SetGridLevel(Index, Level);
        ImgIcon.enabled = true;
        ImgIcon.SetSprite("res" + Level);
    }

    public void ClearLevel() {
        Level = 0;
        puzzleForgeController.SetGridLevel(Index, Level);

        TxtLevel.text = "";
        ImgIcon.enabled = false;
        Log.Debug($"清理 {ToString()}");
    }
    
    #endregion

    #region Private

    private void OnBtnClick() {
        if (Level > 0) {
            return;
        }

        LevelUp();
        onBtnClickGrid?.Invoke(this);
        // RequestMerge();
    }

    private void RequestMerge() {
        // var count = 1;
        // toClearGrids.Clear();
        // // 遍历邻居
        // for (int i = 0; i < NeighborGrids.Count; i++) {
        //     var gridIndex = NeighborGrids[i];
        //     var grid = gridIndex;
        //     if (grid.Level == Level) {
        //         count++;
        //         toClearGrids.Add(grid);
        //         Log.Debug($"【合并】新增 {grid}");
        //         // 遍历邻居的邻居
        //         for (int j = 0; j < grid.NeighborGrids.Count; j++) {
        //             var grid2 = grid.NeighborGrids[j];
        //             if (grid2.Index != Index && 
        //                 grid2.Index != grid.Index && 
        //                 grid2.Level == Level) {
        //                 count++;
        //                 toClearGrids.Add(grid2);
        //                 Log.Debug($"【合并】新增 {grid2}");
        //             }
        //         }
        //     }
        // }
        //
        // if (count >= MinMergeCount) {
        //     LevelUp();
        //     // 清理其他格子
        //     for (int i = 0; i < toClearGrids.Count; i++) {
        //         var grid = toClearGrids[i];
        //         grid.ClearLevel();
        //     }
        //
        //     toClearGrids.Clear();
        //
        //     // 死循环预警
        //     RequestMerge();
        // }
    }

    #endregion
}
