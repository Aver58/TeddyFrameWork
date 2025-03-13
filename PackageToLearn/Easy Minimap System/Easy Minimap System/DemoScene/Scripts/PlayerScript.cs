using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MTAssets.EasyMinimapSystem
{
    public class PlayerScript : MonoBehaviour
    {
        //Cache variables
        private Rigidbody playerRigidbody;
        private bool isMovementKeysCurrentlyPressed = false;
        private float verticalAxisRotation = 0;

        //Enums of script
        public enum CameraMode
        {
            FirstPerson,
            ThirdPerson
        }

        //Public variables
        public bool canHideCursor = true;
        public CameraMode cameraMode;
        public Transform player3dModelPivot;
        public Transform cameraPivot;
        public float movementSpeed = 6;
        public float mouseSensibility = 12;
        public GameObject firstPersonCameraObj;
        public GameObject thirdPersonCameraObj;
        public GameObject player3dModelObj;
        public Animator playerAnimator;
        public Animator cameraAnimator;
        public MinimapItem playerItem;

        void Start()
        {
            //Get components
            playerRigidbody = this.gameObject.GetComponent<Rigidbody>();

            //Set cursor as invisible
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            //Check if some movement key is pressed
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                isMovementKeysCurrentlyPressed = true;
            else
                isMovementKeysCurrentlyPressed = false;

            //Get values from each axis
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            //Player Movement
            if (isMovementKeysCurrentlyPressed == true && Cursor.lockState == CursorLockMode.Locked)
            {
                player3dModelPivot.localRotation = Quaternion.Lerp(player3dModelPivot.localRotation, Quaternion.LookRotation(new Vector3(horizontal, 0, vertical), Vector3.up), 20 * Time.deltaTime);
                playerRigidbody.velocity = transform.TransformVector(new Vector3(horizontal * movementSpeed, playerRigidbody.velocity.y, vertical * movementSpeed));
            }
            if (isMovementKeysCurrentlyPressed == false || Cursor.lockState != CursorLockMode.Locked)
            {
                player3dModelPivot.localRotation = Quaternion.Lerp(player3dModelPivot.localRotation, Quaternion.Euler(0, 0, 0), 26 * Time.deltaTime);
                playerRigidbody.velocity = new Vector3(0, playerRigidbody.velocity.y, 0);
            }

            //Get values from each axis
            float mouseHorizontal = Input.GetAxis("Mouse X");
            float mouseVertical = Input.GetAxis("Mouse Y");

            //Camera movement
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                this.gameObject.transform.eulerAngles += new Vector3(0, mouseHorizontal * mouseSensibility, 0);
                verticalAxisRotation += (mouseVertical * (mouseSensibility * 0.7f * -1));
                verticalAxisRotation = Mathf.Clamp(verticalAxisRotation, -15.0f, 25.0f);
                cameraPivot.localEulerAngles = new Vector3(verticalAxisRotation, 0, 0);
            }

            //Hide/Show of cursor
            if (Input.GetKeyDown(KeyCode.Escape) == true && Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            if (Input.GetMouseButtonDown(0) == true && canHideCursor == true)
                Cursor.lockState = CursorLockMode.Locked;

            //Change the camera mode
            if (Input.GetKeyDown(KeyCode.C) == true && cameraMode == CameraMode.FirstPerson)
            {
                cameraMode = CameraMode.ThirdPerson;
                goto ContinueScriptAfterChangeCameraMode;
            }
            if (Input.GetKeyDown(KeyCode.C) == true && cameraMode == CameraMode.ThirdPerson)
            {
                cameraMode = CameraMode.FirstPerson;
                goto ContinueScriptAfterChangeCameraMode;
            }
        ContinueScriptAfterChangeCameraMode:;

            //Apply configurations of camera
            if (cameraMode == CameraMode.FirstPerson)
            {
                firstPersonCameraObj.SetActive(true);
                thirdPersonCameraObj.SetActive(false);
                player3dModelObj.SetActive(false);
                if (playerItem != null)
                    playerItem.followRotationOf = MinimapItem.FollowRotationOf.ThisGameObject;
            }
            if (cameraMode == CameraMode.ThirdPerson)
            {
                firstPersonCameraObj.SetActive(false);
                thirdPersonCameraObj.SetActive(true);
                player3dModelObj.SetActive(true);
                if (playerItem != null)
                    playerItem.followRotationOf = MinimapItem.FollowRotationOf.CustomGameObject;
            }

            //Apply animation
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                playerAnimator.SetBool("run", isMovementKeysCurrentlyPressed);
                playerAnimator.SetFloat("runSpeed", movementSpeed / 6.0f);
                if (firstPersonCameraObj.activeSelf == true)
                {
                    cameraAnimator.SetBool("run", isMovementKeysCurrentlyPressed);
                    cameraAnimator.SetFloat("runSpeed", movementSpeed / 6.0f);
                }
            }
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                playerAnimator.SetBool("run", false);
                if (firstPersonCameraObj.activeSelf == true)
                {
                    cameraAnimator.SetBool("run", false);
                }
            }

            //Apply the force of weight
            if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, 0.3f) == false)
                playerRigidbody.AddForce(new Vector3(0, -100, 0), ForceMode.Force);
        }
    }
}