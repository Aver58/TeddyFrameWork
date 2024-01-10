using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PuzzleForgeItem : MonoBehaviour {
    private const int MinMergeCount = 3;
    public int RowIndex;
    public int ColumnIndex;
    public int Index;
    public int Level;  // 当前格子存储的资源等级
    public List<PuzzleForgeItem> NeighborGrids;
    private List<PuzzleForgeItem> toClearGrids;
    
    
    [SerializeField] private Image ImgBg;
    [SerializeField] private Image ImgIcon;
    [SerializeField] private Text TxtLevel;
    [SerializeField] private Button BtnClick;

    #region Public

    public void Init(int rowIndex, int columnIndex, int index) {
        Level = 0;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
        Index = index;
        NeighborGrids = new List<PuzzleForgeItem>();
        toClearGrids = new List<PuzzleForgeItem>(2);
        
        BtnClick.onClick.AddListener(OnBtnClick);
    }

    public void Clear() {
        BtnClick.onClick.RemoveListener(OnBtnClick);
        
    }
    
    public void AddNeighbor(PuzzleForgeItem item) {
        NeighborGrids.Add(item);
    }

    public void ClearLevel() {
        Level = 0;
        TxtLevel.text = Level.ToString();
    }
    
    #endregion

    #region Private

    private void OnBtnClick() {
        if (Level > 0) {
            return;
        }

        LevelUp();
        RequestMerge();
    }

    private void RequestMerge() {
        var count = 1;
        toClearGrids.Clear();
        for (int i = 0; i < NeighborGrids.Count; i++) {
            var grid = NeighborGrids[i];
            if (grid.Level == Level) {
                count++;
                toClearGrids.Add(grid);
                // 遍历第二层
                for (int j = 0; j < grid.NeighborGrids.Count; j++) {
                    var grid2 = grid.NeighborGrids[j];
                    if (grid2.Index != Index && 
                        grid2.Index != grid.Index && 
                        grid2.Level == Level) {
                        count++;
                        toClearGrids.Add(grid2);
                    }
                }
            }
        }

        if (count >= MinMergeCount) {
            LevelUp();
            // 死循环预警
            RequestMerge();
        }
    }

    private void LevelUp() {
        Level++;
        // 先用数字代替吧
        TxtLevel.text = Level.ToString();
        for (int i = 0; i < toClearGrids.Count; i++) {
            var grid = toClearGrids[i];
            grid.ClearLevel();
        }

        toClearGrids.Clear();
        // 设置图片
        // ImgIcon.sprite = "";
    }

    #endregion
}
