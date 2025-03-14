using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    public class LocalMultiplayer : MonoBehaviour
    {
        private Rigidbody playerRigidbody;

        public KeyCode moveToForward;
        public KeyCode moveToBack;
        public KeyCode moveToLeft;
        public KeyCode moveToRight;
        public float movementSpeed = 6.0f;
        public float rotationSpeed = 6.0f;
        public bool invertRotation = false;

        public void Start()
        {
            playerRigidbody = this.gameObject.GetComponent<Rigidbody>();
        }

        public void Update()
        {
            //GEt the movement
            Vector3 movementAxis = Vector3.zero;
            if (Input.GetKey(moveToForward) == true)
                movementAxis.z = 1.0f;
            if (Input.GetKey(moveToBack) == true)
                movementAxis.z = -1.0f;
            if (Input.GetKey(moveToForward) == false && Input.GetKey(moveToBack) == false)
                movementAxis.z = 0.0f;
            Vector3 rotationAxis = Vector3.zero;
            if (Input.GetKey(moveToLeft) == true)
                rotationAxis.y = 1.0f;
            if (Input.GetKey(moveToRight) == true)
                rotationAxis.y = -1.0f;
            if (Input.GetKey(moveToLeft) == false && Input.GetKey(moveToRight) == false)
                rotationAxis.y = 0.0f;

            //Set the movement
            playerRigidbody.velocity = transform.TransformVector(new Vector3(movementAxis.x * movementSpeed, playerRigidbody.velocity.y, movementAxis.z * movementSpeed));
            playerRigidbody.transform.Rotate(new Vector3(0, rotationAxis.y * rotationSpeed * ((invertRotation == true) ? -1 : 1), 0));
        }
    }
}