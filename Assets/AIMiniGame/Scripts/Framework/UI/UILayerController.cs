using UnityEngine;

namespace AIMiniGame.Scripts.Framework.UI {
    public class UILayerController: MonoBehaviour {
        // 不同层级的Canvas排序设置
        public void SetLayerOrder(Transform uiTransform, UILayer layer) {
            Canvas canvas = uiTransform.GetComponent<Canvas>();
            canvas.sortingOrder = (int)layer * 100; // 每层间隔100避免冲突
        }

        // 设备适配（示例：异形屏安全区域）
        public void ApplySafeArea(RectTransform rectTransform) {
            Rect safeArea = Screen.safeArea;
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}