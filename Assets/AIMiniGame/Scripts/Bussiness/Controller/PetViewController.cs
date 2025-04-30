using AIMiniGame.Scripts.Bussiness.Model;
using AIMiniGame.Scripts.Framework.UI;

namespace AIMiniGame.Scripts.Bussiness.Controller {
    public class PetViewController : ControllerBase {
        public PetViewModel Model { get; private set; }

        public PetViewController() {
            Model = new PetViewModel { Health = 100 };
            Model.PropertyChanged += (sender, args) => NotifyModelChanged();
        }
    }
}