using System;

namespace AIMiniGame.Scripts.Framework.UI {
    // Controller基类
    public abstract class ControllerBase {
        public bool IsClear { get; private set; } = false;
        public bool IsOpen { get; private set; }
        public string FunctionName { get; private set; }
        public UIViewBase Window { get; set; }

        public event Action OnModelChanged;
        protected ModelBase Model;
        protected void NotifyModelChanged() => OnModelChanged?.Invoke();

        public void OpenAsync() {
            if (IsOpen) {
                return;
            }

            IsOpen = true;
            Window = UIManager.Instance.Open(this);
            if (Window) {
                Window.gameObject.SetActive(true);
            }
            OnOpen();
        }

        public void Close(bool immediately = false) {
            if (!IsOpen) {
                return;
            }

            IsOpen = false;
            IsClear = true;
            OnClose();
            if (Window) {
                Window.gameObject.SetActive(false);
            }
        }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        public void Init(string functionName) {
            FunctionName = functionName;
        }
    }
}

// // 示例Controller（玩家逻辑）
// public class PlayerController : ControllerBase {
//     public PlayerModel Model { get; private set; }
//
//     public PlayerController() {
//         Model = new PlayerModel { Health = 100 };
//         Model.PropertyChanged += (sender, args) => NotifyModelChanged();
//     }
//
//     public void TakeDamage(int damage) {
//         Model.Health -= damage;
//         // 业务逻辑（如死亡检测）
//         if (Model.Health <= 0) {
//             Debug.Log("Player Died!");
//         }
//     }
// }