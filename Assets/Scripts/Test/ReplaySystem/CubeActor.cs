using System.Collections.Generic;
using UnityEngine;

public class CubeActor : Actor {
    public float moveSpeed = 5f;

    private CubeSerializer lastSerializer;

    private struct CubeSerializer : IEqualityComparer<CubeSerializer> {
        public ReplayType replayType;
        public Vector3 position;
        public Quaternion rotation;

        public CubeSerializer(Vector3 position, Quaternion rotation) {
            replayType = ReplayType.CubeActor;
            this.position = position;
            this.rotation = rotation;
        }

        public bool Equals(CubeSerializer x, CubeSerializer y) {
            return x.position.Equals(y.position) && x.rotation.Equals(y.rotation);
        }

        public int GetHashCode(CubeSerializer obj) {
            unchecked {
                return (obj.position.GetHashCode() * 397) ^ obj.rotation.GetHashCode();
            }
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
        var newSerializer = new CubeSerializer(transform1.position, transform1.rotation);
        // if (!lastSerializer.Equals(default) && lastSerializer.Equals(newSerializer)) {
        //     return null;
        // }

        // lastSerializer = newSerializer;
        var data = JsonUtility.ToJson(newSerializer);
        return data;
    }

    public override void Deserialize(string jsonData) {
        var data = JsonUtility.FromJson<CubeSerializer>(jsonData);
        var transform1 = transform;
        transform1.position = data.position;
        transform1.rotation = data.rotation;
    }
}