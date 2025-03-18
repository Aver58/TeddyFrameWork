using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AIMiniGame.Scripts.Framework.UI {
    // Model基类（实现属性变更通知）
    public abstract class ModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // // 示例Model（玩家数据）
    // public class PlayerModel : ModelBase {
    //     private int _health;
    //     public int Health {
    //         get => _health;
    //         set {
    //             if (_health != value) {
    //                 _health = value;
    //                 RaisePropertyChanged(); // 触发数据变更事件
    //             }
    //         }
    //     }
    // }
}