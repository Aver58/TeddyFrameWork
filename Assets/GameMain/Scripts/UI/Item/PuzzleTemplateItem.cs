using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PuzzleTemplateItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
    [SerializeField] private Image ImgIcon;

    private int index;
    private int dataIndex;
    private RectTransform parentTransform;
    private RectTransform imgIconTransform;
    private Action onEndDrag;

    private PuzzleForgeController puzzleForgeController;


    #region Public

    public void Init(int index, int dataIndex, RectTransform parentTransform, Action onEndDrag) {
        puzzleForgeController = GameEntry.Controller.PuzzleForgeController;
        this.index = index;
        this.dataIndex = dataIndex;
        this.onEndDrag = onEndDrag;
        this.parentTransform = parentTransform;

        ImgIcon.enabled = true;
        ImgIcon.SetSprite("template" + index);
        imgIconTransform = ImgIcon.rectTransform;
        imgIconTransform.localPosition = Vector3.zero;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (index != 0) {
            return;
        }

        puzzleForgeController.DragTemplateIndex = dataIndex;
    }

    public void OnDrag(PointerEventData eventData) {
        if (index != 0) {
            return;
        }

        RectTransformUtility.ScreenPointToWorldPointInRectangle(parentTransform, eventData.position, eventData.enterEventCamera, out var globalMousePos);
        imgIconTransform.position  = globalMousePos;
    }

    public void OnEndDrag(PointerEventData eventData) {
        if (index != 0) {
            return;
        }

        imgIconTransform.localPosition = Vector3.zero;
        onEndDrag?.Invoke();
    }

    #endregion
    
    #region Private



    #endregion
}
