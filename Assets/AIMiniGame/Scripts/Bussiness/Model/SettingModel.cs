using AIMiniGame.Scripts.Framework.UI;

namespace AIMiniGame.Scripts.Bussiness.Model {
    public class SettingModel : ModelBase {
        // 窗口置顶

        // 系统托盘 隐藏主窗口 创建系统托盘图标
        private bool isWindowsTop;
        public bool IsWindowsTop {
            get => isWindowsTop;
            set {
                if (isWindowsTop != value) {
                    isWindowsTop = value;
                    RaisePropertyChanged();
                }
            }
        }

        // 

    }
}