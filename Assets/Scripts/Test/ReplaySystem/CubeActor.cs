using UnityEngine;

namespace Test.ReplaySystem {
    public class CubeActor : Actor {
        private CubeSerializer lastSerializer;

        public float moveSpeed = 5f;
        public string AssetName;
        public int MessageType => MessageTypeConst.CubeActor;

        private struct CubeSerializer {
            public int ActorId;
            public string AssetName;
            public Vector3 Position;
            public Quaternion Rotation;

            public CubeSerializer(int actorId, string assetName, Vector3 position, Quaternion rotation) {
                ActorId = actorId;
                AssetName = assetName;
                Position = position;
                Rotation = rotation;
            }
        }

        private void Update() {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontalInput, verticalInput, 0f) * moveSpeed * Time.deltaTime;
            transform.Translate(movement);
        }

        public override string Serialize() {
            var transform1 = transform;
            var newSerializer = new CubeSerializer(ActorId, AssetName, transform1.position, transform1.rotation);
            // if (!lastSerializer.Equals(default) && lastSerializer.Equals(newSerializer)) {
            //     return null;
            // }

            // lastSerializer = newSerializer;
            var data = JsonUtility.ToJson(newSerializer);
            return data;
        }

        public override void Deserialize(string dataString) {
            var data = JsonUtility.FromJson<CubeSerializer>(dataString);
            var transform1 = transform;
            transform1.position = data.Position;
            transform1.rotation = data.Rotation;
        }
    }
}