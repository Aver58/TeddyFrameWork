using AIMiniGame.Scripts.Bussiness.Model;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

namespace AIMiniGame.Scripts.Bussiness.Controller {
    public class SettingController : ControllerBase {
        public SettingModel Model { get; private set; }
        public SettingController() {
            Model = new SettingModel { };
            Model.PropertyChanged += (sender, args) => NotifyModelChanged();
        }
    }
}