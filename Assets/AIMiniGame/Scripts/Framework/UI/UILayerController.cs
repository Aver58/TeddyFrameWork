using UnityEngine;

namespace AIMiniGame.Scripts.Framework.UI {
    public class UILayerController {
        // 不同层级的Canvas排序设置
        public static void SetLayerOrder(Transform uiTransform, UILayer layer) {
            Canvas canvas = uiTransform.GetComponent<Canvas>();
            if (canvas == null) {
                canvas = uiTransform.gameObject.AddComponent<Canvas>();
            }

            // 每层间隔100避免冲突
            canvas.sortingOrder = (int)layer * 100;
        }

        // 设备适配（示例：异形屏安全区域）
        public static void ApplySafeArea(RectTransform rectTransform) {
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