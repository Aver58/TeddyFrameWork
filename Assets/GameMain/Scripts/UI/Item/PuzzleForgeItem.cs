using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public class PuzzleForgeItem : MonoBehaviour {
    private const int MinMergeCount = 3;
    public int Level;  // 当前格子存储的资源等级
    public int Index;
    public int RowIndex;
    public int ColumnIndex;

    public List<PuzzleForgeItem> NeighborGrids;
    private List<PuzzleForgeItem> toClearGrids;

    [SerializeField] private Image ImgBg;
    [SerializeField] private Image ImgIcon;
    [SerializeField] private Text TxtLevel;
    [SerializeField] private Button BtnClick;

    #region Public

    public void Init(int rowIndex, int columnIndex, int index) {
        Level = 0;
        Index = index;
        RowIndex = rowIndex;
        ColumnIndex = columnIndex;
#if UNITY_EDITOR
        gameObject.name = ToString();
#endif
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

    public override string ToString() {
        var s = $"index:{Index} 行:{RowIndex + 1} 列:{ColumnIndex + 1}";
        return s;
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
        // 遍历邻居
        for (int i = 0; i < NeighborGrids.Count; i++) {
            var grid = NeighborGrids[i];
            if (grid.Level == Level) {
                count++;
                toClearGrids.Add(grid);
                Log.Debug($"【合并】新增 {grid}");
                // 遍历邻居的邻居
                for (int j = 0; j < grid.NeighborGrids.Count; j++) {
                    var grid2 = grid.NeighborGrids[j];
                    if (grid2.Index != Index && 
                        grid2.Index != grid.Index && 
                        grid2.Level == Level) {
                        count++;
                        toClearGrids.Add(grid2);
                        Log.Debug($"【合并】新增 {grid2}");
                    }
                }
            }
        }

        if (count >= MinMergeCount) {
            LevelUp();
            // 清理其他格子
            for (int i = 0; i < toClearGrids.Count; i++) {
                var grid = toClearGrids[i];
                grid.ClearLevel();
            }

            toClearGrids.Clear();

            // 死循环预警
            RequestMerge();
        }
    }

    private void LevelUp() {
        Level++;
        // 先用数字代替吧
        TxtLevel.text = Level.ToString();
        // 设置图片
        // ImgIcon.sprite = "";
    }

    private void ClearLevel() {
        Level = 0;
        TxtLevel.text = Level.ToString();
        Log.Debug($"清理 {ToString()}");
    }

    #endregion
}
