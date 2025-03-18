using System;

namespace AIMiniGame.Scripts.Framework.UI {
    // Controller基类
    public abstract class ControllerBase {
        public event Action OnModelChanged;
        protected ModelBase Model;
        protected void NotifyModelChanged() => OnModelChanged?.Invoke();
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
}