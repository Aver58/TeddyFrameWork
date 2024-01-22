using System;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UnityEngine.EventSystems;

public class PuzzleForgeGridItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    private int GridColumnCount;
    public int Level;  
    public int Index;
    public int RowIndex;
    public int ColumnIndex;

    private PuzzleForgeController puzzleForgeController;
    [SerializeField] private Image ImgBg;
    [SerializeField] private Image ImgIcon;
    [SerializeField] private Text TxtLevel;
    [SerializeField] private Button BtnClick;

    #region Public

    public void Init(int index) {
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        GridColumnCount = puzzleForgeController.GridColumnCount;
        
        Level = puzzleForgeController.GetGridLevel(index);
        Index = index;
        RowIndex = index / GridColumnCount;
        ColumnIndex = index % GridColumnCount;
#if UNITY_EDITOR
        gameObject.name = ToString();
#endif
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

    public void OnPointerEnter(PointerEventData eventData) {
        if (puzzleForgeController.IsDragTemplate()) {
            Log.Debug($"OnPointerEnter {Index}");
            puzzleForgeController.SelectGridIndex = Index;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData) {
        if (puzzleForgeController.IsDragTemplate()) {
            Log.Debug($"OnPointerExit {Index}");
        }
    }

    public void OnRefresh() {
        Level = puzzleForgeController.GetGridLevel(Index);
        RefreshLevel();
    }

    #endregion

    #region Private

    private void OnBtnClick() {
        puzzleForgeController.RequestPlaceGrid(Index);
    }

    private void RefreshLevel() {
        TxtLevel.text = Level == 0 ? "" : Level.ToString();
        puzzleForgeController.SetGridLevel(Index, Level);
        ImgIcon.enabled = Level != 0;
        if (Level != 0) {
            ImgIcon.SetSprite("res" + Level);
        }
    }

    #endregion
}
