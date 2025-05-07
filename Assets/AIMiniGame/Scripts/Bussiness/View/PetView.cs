using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine.EventSystems;

public class PetView : UIViewBase {
    public EventTrigger moveObj;
    private RectTransform rectTransform;

    private void Start() {
        if (moveObj == null) {
            return;
        }

        rectTransform = moveObj.GetComponent<RectTransform>();

        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        moveObj.triggers.Add(entry);

        var entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.Drag;
        entry2.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        moveObj.triggers.Add(entry2);

        var entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.Drop;
        entry3.callback.AddListener((data) => { OnDrop((PointerEventData)data); });
        moveObj.triggers.Add(entry3);
    }

    private void OnPointerDown(BaseEventData eventData) {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if (pointerEventData.button == PointerEventData.InputButton.Right) {
            Debug.Log("Right mouse button clicked");
            // 打开界面
        }

        if (pointerEventData.button == PointerEventData.InputButton.Left) {
            Debug.Log("Left mouse button clicked");
            // 关闭界面
        }
    }

    private void OnDrag(PointerEventData eventData) {
        // 移动 moveObj 对象的位置
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPosition
        );

        rectTransform.position = worldPosition;
    }

    // todo 无效 EndDrag
    private void OnDrop(PointerEventData eventData) {
        Debug.Log("Dropped on: " + eventData.pointerDrag.name);
        throw new NotImplementedException();
    }

    private void Update() {

    }

    protected override void UpdateView() {
        // if (Controller is TestViewController testViewController) {
        //     _testText.text = $"Health: {testViewController.Model.Health}";
        // }
    }
}
