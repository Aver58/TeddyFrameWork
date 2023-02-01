using CustomPhysics.Collision;
using CustomPhysics.Collision.Shape;
using UnityEngine;

namespace CustomPhysics.Test {
    public class CollisionObjectProxy : MonoBehaviour {
        public CollisionObject target;
        public ShapeType shape;
        public bool isInControl = false;
        public int nP = 1;
        public float velocity = 7;

        private void Update() {
            transform.position = target.position;
            transform.rotation = Quaternion.Euler(target.rotation);
            transform.localScale = target.scale * Vector3.one;
            shape = target.shape.shapeType;

            if (isInControl) {
                if (nP == 1) {
                    if (Input.GetKey(KeyCode.A)) {
                        target.SetInputMoveVelocity(velocity * Vector3.left);
                    }
                    if (Input.GetKey(KeyCode.D)) {
                        target.SetInputMoveVelocity(velocity * Vector3.right);
                    }
                    if (Input.GetKey(KeyCode.W)) {
                        target.SetInputMoveVelocity(velocity * Vector3.forward);
                    }
                    if (Input.GetKey(KeyCode.S)) {
                        target.SetInputMoveVelocity(velocity * Vector3.back);
                    }
                    if (Input.GetKey(KeyCode.G)) {
                        target.Rotate(new Vector3(0, -1, 0));
                    }
                    if (Input.GetKey(KeyCode.H)) {
                        target.Rotate(new Vector3(0, 1, 0));
                    }
                    if (Input.GetKeyDown(KeyCode.B)) {
                        target.Scale(1);
                    }
                    if (Input.GetKeyDown(KeyCode.N)) {
                        target.Scale(-1);
                    }
                } else if (nP == 2) {
                    if (Input.GetKey(KeyCode.LeftArrow)) {
                        target.SetInputMoveVelocity(velocity * Vector3.left);
                    }
                    if (Input.GetKey(KeyCode.RightArrow)) {
                        target.SetInputMoveVelocity(velocity * Vector3.right);
                    }
                    if (Input.GetKey(KeyCode.UpArrow)) {
                        target.SetInputMoveVelocity(velocity * Vector3.forward);
                    }
                    if (Input.GetKey(KeyCode.DownArrow)) {
                        target.SetInputMoveVelocity(velocity * Vector3.back);
                    }
                }
            }
        }
    }
}