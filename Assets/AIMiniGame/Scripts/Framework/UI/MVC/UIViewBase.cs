using UnityEngine;
using UnityEngine.UI;

namespace AIMiniGame.Scripts.Framework.UI {
    public abstract class UIViewBase : MonoBehaviour {
        protected ControllerBase Controller;
        public UILayer Layer = UILayer.Normal;// 所属层级

        public virtual void Init(UILayer layer) {
            UILayerController.SetLayerOrder(this.transform, layer);
            var graphicRaycaster = transform.GetComponent<GraphicRaycaster>();
            if (graphicRaycaster == null) {
                graphicRaycaster = transform.gameObject.AddComponent<GraphicRaycaster>();
            }

            OnInit();
        }

        public void Open() {
            OnOpen();
        }

        public void Close() {
            OnClose();
        }

        public void Clear() {
            OnClear();
        }

        protected virtual void OnInit(){}
        protected virtual void OnOpen() {}     // 界面显示回调
        protected virtual void OnClose() {}    // 界面隐藏回调
        protected virtual void OnClear() { }

        // 绑定Controller（手动或通过依赖注入）
        public void BindController(ControllerBase controller) {
            Controller = controller;
            Controller.OnModelChanged += UpdateView;
        }

        // 抽象方法：子类实现具体UI更新逻辑
        protected virtual void UpdateView(){}
    }

    // // 示例View（玩家血条UI）
    // public class PlayerHealthView : UIView {
    //     [SerializeField] private Text _healthText; // Unity Inspector中绑定
    //     protected override void UpdateView() {
    //         if (Controller is PlayerController playerController) {
    //             _healthText.text = $"Health: {playerController.Model.Health}";
    //         }
    //     }
    //
    //     // 按钮点击事件（由Unity Event触发）
    //     public void OnTakeDamageButtonClick() {
    //         (Controller as PlayerController)?.TakeDamage(10);
    //     }
    // }
}