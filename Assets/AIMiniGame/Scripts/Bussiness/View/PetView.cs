using UnityEngine;
using Debug = UnityEngine.Debug;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PetView : UIViewBase {
    public Animator animator;
    public EventTrigger moveObj;
    private RectTransform rectTransform;
    public Text TxtInputCount;
    private int inputCount = 0;
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
            animator.SetInteger(StateInt, (int)value);
        }
    }

    private bool isMusicOn = false;
    private int StateInt = Animator.StringToHash("State");
    private float scrollValue = 0f;
    protected override void OnInit() {
        if (moveObj == null) {
            return;
        }

        rectTransform = moveObj.GetComponent<RectTransform>();

        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
        moveObj.triggers.Add(entry);

        var entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerUp;
        entry1.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
        moveObj.triggers.Add(entry1);

        var entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.Drag;
        entry2.callback.AddListener((data) => { OnDrag((PointerEventData)data); });
        moveObj.triggers.Add(entry2);
    }

    protected override void OnClear() {
        base.OnClear();
    }

    private void OnPointerDown(BaseEventData eventData) {
        PointerEventData pointerEventData = (PointerEventData)eventData;
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right mouse button clicked");
            // 打开界面
        }

        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left mouse button clicked");
            // 关闭界面
        }
    }

    private void OnPointerUp(PointerEventData eventData) {
        PetState = PetState.Idle;
    }

    private void OnDrag(PointerEventData eventData) {
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out worldPosition
        );

        rectTransform.position = worldPosition;
        PetState = PetState.Drag;
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

        // 检测不同屏幕的输入，键盘输入，任意一个键被按下
        if (Input.anyKeyDown) {
            Debug.Log("任意键被按下");
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