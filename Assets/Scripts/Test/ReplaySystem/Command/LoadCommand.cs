using UnityEngine;

namespace Test.ReplaySystem.Command {
    public class LoadCommand : ICommand {
        private int actorId;
        private string assetName;
        private Vector3 position;
        private Quaternion rotation;

        public string Serialize() {

            return "";
        }

        public void Deserialize(string data) {
            // var data = JsonUtility.FromJson<CubeSerializer>(dataString);
            //
            this.actorId = actorId;
            this.assetName = assetName;
            this.position = position;
            this.rotation = rotation;
        }

        public void Execute() {
            LoadModule.LoadTestModel(assetName, (assetRequest) => {
                var asset = assetRequest.asset as UnityEngine.GameObject;
                var gameObject = UnityEngine.Object.Instantiate(asset);
                var actor = gameObject.GetComponent<CubeActor>();
                actor.ActorId = actorId;
                actor.AssetName = assetName;

                if (ReplayHelper.IsRecording) {
                    ReplaySystem.Instance.RegisterPacket(MessageTypeConst.LoadCubeActor, actor.Serialize());
                }
            });
        }

        public void Undo() {

        }
    }
}