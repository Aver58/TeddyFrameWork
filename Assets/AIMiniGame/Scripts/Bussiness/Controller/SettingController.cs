using AIMiniGame.Scripts.Bussiness.Model;
using AIMiniGame.Scripts.Framework.UI;
using UnityEngine;

namespace AIMiniGame.Scripts.Bussiness.Controller {
    public class SettingController : ControllerBase {
        public static string IsWindowsTopMost = "IsWindowsTopMost";
        public static string IsClickThrough = "isClickThrough";

        public static string MainPetId = "MainPetId";
        // public static string LeftPetId = "LeftPetId";
        // public static string RightPetId = "RightPetId";
        public static string HeadDecorateSprite = "HeadDecorateSprite";

        public SettingModel Model { get; private set; }
        public SettingController() {
            Model = new SettingModel { };
            Model.PropertyChanged += (sender, args) => NotifyModelChanged();
        }
    }
}