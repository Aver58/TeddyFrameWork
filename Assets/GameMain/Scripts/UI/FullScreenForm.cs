using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public class FullScreenForm : UIFormLogic {
    protected override void OnInit(object userData) {
        base.OnInit(userData);

        RectTransform transform = GetComponent<RectTransform>();
        transform.anchorMin = Vector2.zero;
        transform.anchorMax = Vector2.one;
        transform.anchoredPosition = Vector2.zero;
        transform.sizeDelta = Vector2.zero;
    }
}
