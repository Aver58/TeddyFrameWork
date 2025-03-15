using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

public class TestView : BaseUI {
    public override void Init(UILayer layer) {
        Layer = layer;
    }

    public override void OnShow() {
        Debug.Log("TestView OnShow");
    }

    public override void OnHide() {
        Debug.Log("TestView OnHide");
    }
}
