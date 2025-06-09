using AIMiniGame.Scripts.Bussiness.Model;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

namespace AIMiniGame.Scripts.Bussiness.Controller {
    public class TestController : ControllerBase {
        public TestViewModel Model { get; private set; }

        public TestController() {
            Model = new TestViewModel { Health = 100 };
            Model.PropertyChanged += (sender, args) => NotifyModelChanged();
        }

        public void TakeDamage(int damage) {
            Model.Health -= damage;
            // 业务逻辑（如死亡检测）
            if (Model.Health <= 0) {
                Debug.Log("Player Died!");
            }
        }
    }
}