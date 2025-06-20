using UnityEngine;
using Debug = UnityEngine.Debug;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AIMiniGame.Scripts.Bussiness.Controller;
using System;
using Kirurobo;

public class PetView : UIViewBase {
    public Image ImgMain;
    public Image ImgLeft;
    public Image ImgRight;
    public Image ImgHead;
    public EventTrigger moveObj;
    public Text TxtInputCount;
    private RectTransform rectTransform;
    private int inputCount = 0;
    private PetController petController;

    public int InputCount {
        get {
            return inputCount;
        } set {
            inputCount = value;
            TxtInputCount.text = inputCount.ToString();
        }
    }

    private PetState petState = PetState.Idle;
    private PetState PetState {
        get {
            return petState;
        } set {
            petState = value;
        }
    }

    private int StateInt = Animator.StringToHash("State");
    private float scrollValue = 0f;
    private UniWindowController uniWindowController;

    protected override void OnInit() {
        if (moveObj == null) {
            return;
        }

        rectTransform = moveObj.GetComponent<RectTransform>();
        petController = Controller as PetController;

        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        moveObj.triggers.Add(entry);

        var entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerUp;
        entry1.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        moveObj.triggers.Add(entry1);

        InitSetting();
    }

    private void InitSetting() {
        uniWindowController = UniWindowController.current;
        uniWindowController.isTopmost = PlayerPrefs.GetInt(SettingController.IsWindowsTopMost, 0) == 1;
        uniWindowController.isClickThrough = PlayerPrefs.GetInt(SettingController.IsClickThrough, 0) == 1;
        // 透明 
        uniWindowController.isTransparent = true;
        uniWindowController.alphaValue = 1;
    }

    protected override void OnClear() {
        base.OnClear();
    }

    private void OnPointerDown(BaseEventData eventData) {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if (pointerEventData.button == PointerEventData.InputButton.Right){
            ControllerManager.Instance.OpenAsync<SettingController>();
        }

        if (pointerEventData.button == PointerEventData.InputButton.Left){
            ControllerManager.Instance.Close<SettingController>();
        }
    }

    private void OnPointerUp(PointerEventData eventData) {
    }

    private void Update() {
        // 检测滚轮滚动
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0) {
            scrollValue += scroll;
            if (scrollValue > 1) {
                InputCount++;
                scrollValue = 0f;
            }
        }

        if (Input.anyKeyDown) {
            InputCount++;
        }
    }

    protected override void UpdateView() {
        // if (Controller is TestViewController testViewController) {
        //     _testText.text = $"Health: {testViewController.Model.Health}";
        // }
    }
}

public enum PetState {
    Idle,
    Drag,
    Music,
    Sleep,
    Walk,
}