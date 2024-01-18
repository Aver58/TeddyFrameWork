using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityGameFramework.Runtime;

public class PuzzleTemplateItem : MonoBehaviour, IBeginDragHandler {
    private int index;
    [SerializeField] private Image ImgIcon;
    private PuzzleForgeController puzzleForgeController;

    #region Public

    public void Init(int index) {
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        this.index = index;
        
        ImgIcon.SetSprite("template" + index);
        ImgIcon.enabled = true;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        puzzleForgeController.DragTemplateIndex = index;
        ImgIcon.enabled = false;
    }
    
    #endregion
    
    #region Private

    
    
    #endregion
}
