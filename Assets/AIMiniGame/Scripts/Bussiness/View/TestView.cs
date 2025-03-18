using AIMiniGame.Scripts.Bussiness.Controller;
using AIMiniGame.Scripts.Framework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestView : UIViewBase {
    [SerializeField] private Button _testBtn;
    [SerializeField] private TextMeshProUGUI _testText;

    public override void Init(UILayer layer) {
        base.Init(layer);
        _testBtn.onClick.AddListener(OnTakeDamageButtonClick);
    }

    public override void OnOpen() {
        Debug.Log("TestView OnShow");
    }

    public override void OnClose() {
        Debug.Log("TestView OnHide");
    }

    protected override void UpdateView() {
        if (Controller is TestViewController testViewController) {
            _testText.text = $"Health: {testViewController.Model.Health}";
        }
    }

    private void OnTakeDamageButtonClick() {
        (Controller as TestViewController)?.TakeDamage(10);
    }
}
