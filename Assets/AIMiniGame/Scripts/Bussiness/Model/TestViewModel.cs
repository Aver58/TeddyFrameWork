using AIMiniGame.Scripts.Framework.UI;

namespace AIMiniGame.Scripts.Bussiness.Model {
    public class TestViewModel : ModelBase {
        private int _health;
        public int Health {
            get => _health;
            set {
                if (_health != value) {
                    _health = value;
                    RaisePropertyChanged(); // 触发数据变更事件
                }
            }
        }
    }
}