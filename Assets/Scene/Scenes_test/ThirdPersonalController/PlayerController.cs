using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace ThirdPersonalController {
    public class PlayerController : MonoBehaviour {
        private CharacterController cc;
        private Transform camaraTransform;

        [Header("Debug")]
        [FieldLabel("是否在地面上")] public bool isGrounded = false;
        [FieldLabel("垂直方向")] public Vector3 verticalSpeed = Vector3.zero;
        [FormerlySerializedAs("iceMoveDirection")] [FormerlySerializedAs("moveDirection")] [FieldLabel("移动方向")] public Vector3 iceMoveVelocity = Vector3.zero;
        [FieldLabel("是否在冰面")] public bool isInIceZone;
        [Header("基础配置")]
        [FieldLabel("相机")] public Camera camera;　
        [FieldLabel("重力")] public float gravity = -9.8f;　
        [FieldLabel("跳跃高度")] public float jumpSpeed = 5.0f;　
        [FieldLabel("普通移动速度大小")] public float normalMoveSpeed = 5.0f;
        [Header("冰面配置")]
        // [FieldLabel("摩擦力系数(0 ~ 1, 越接近1滑动越久)")] public float slideFriction = 0.98f;
        [FieldLabel("加速因子")] public float accelerationFactor = 5f;
        [FieldLabel("减速因子")] public float decelerationFactor = 2f;
        [FieldLabel("冰面斜坡滑行速度")] public float IceSlopeAccelerationSpeed = 2;
        [FieldLabel("冰面斜坡滑行最大速度")] public float maxSlopeSpeed = 10f;
        [FieldLabel("μ摩擦系数")] public float frictionCoefficient = 0.3f;
        private void Start() {
            cc = GetComponent<CharacterController>();
            camaraTransform = camera.transform;

        }

        private void FixedUpdate() {
             // 检测是否在地面上
            isGrounded = cc.isGrounded;

            UpdateRotation();
            UpdateVerticalSpeed();
            UpdateHorizontalSpeed();
            // 处理斜坡滑动逻辑
            // if (isGrounded) {
            //     Vector3 slopeDirection = GetSlopeDirection();
            //     if (slopeDirection != Vector3.zero) {
            //         moveDirection += slopeDirection * gravity * Time.deltaTime;
            //     }
            // }

            // moveDirection
            cc.Move(iceMoveVelocity * Time.deltaTime);
        }

        private void UpdateHorizontalSpeed() {
            // var ySpeed = moveDirection.y;
            if (isInIceZone) {
                SlideMovement();
            } else {
                NormalMovement();
            }

            // moveDirection.y = ySpeed;
        }

        private void SlideMovement() {
            if (!isGrounded) {
                Debug.LogError("Not on the ground!");
            }
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 inputForce = transform.forward * vertical + transform.right * horizontal;
            // 需要计算的力：玩家输入 + 重力在斜坡方向上的力 + 摩擦力
            var inputVelocity = inputForce.normalized * accelerationFactor * Time.deltaTime;
            // iceMoveDirection += inputVelocity;
            // 应用摩擦力（减速）
            if (isGrounded) {
                var position = transform.position;
                var horizontalVelocity = new Vector3(iceMoveVelocity.x, 0, iceMoveVelocity.z);
                var frictionForce = -horizontalVelocity.normalized * Time.deltaTime;

                var slopeAngel = Vector3.Angle(slopeNormal, Vector3.up);
                if (slopeNormal != Vector3.up && slopeAngel > 10) {
                    var slopeVelocity = Vector3.ProjectOnPlane(Vector3.down, slopeNormal);
                    // 重力沿斜坡平行方向的分力
                    var gravityParallel = slopeVelocity * Mathf.Abs(gravity);
                    // 重力沿斜坡垂直方向的分力
                    var gravityVertical = gravity * Vector3.down - gravityParallel;
                    // 计算摩擦力 f = μ * m * g 垂直于斜坡的重力分量
                    // 速度在斜坡方向的投影分量
                    var velocityOnSlope  = Vector3.ProjectOnPlane(horizontalVelocity, slopeNormal); // 速度在斜坡方向的分量
                    frictionForce = -velocityOnSlope.normalized * frictionCoefficient * gravityVertical.magnitude * Time.deltaTime;
                    Debug.DrawLine(position, position + slopeVelocity * 2, Color.green);
                    Debug.DrawLine(position, position + velocityOnSlope * 2, Color.blue);
                    Debug.DrawLine(position, position + gravityParallel * 2, Color.white);
                    // 重力在斜坡平行方向的分力
                    iceMoveVelocity += slopeVelocity.normalized * IceSlopeAccelerationSpeed * Time.deltaTime;
                    // Debug.LogError($"slopeAngel {slopeAngel} slopeVelocity {slopeVelocity} iceMoveDirection {iceMoveDirection}");
                    var inputVelocityOnSlope = Vector3.ProjectOnPlane(inputForce, slopeNormal);
                    inputVelocity = inputVelocityOnSlope.normalized * accelerationFactor * Time.deltaTime;
                }

                iceMoveVelocity = Vector3.ClampMagnitude(iceMoveVelocity, maxSlopeSpeed);
                // iceMoveDirection += inputVelocity;
                // iceMoveDirection += frictionForce;
                Debug.DrawLine(position, position + frictionForce * 400, Color.yellow);
                Debug.DrawLine(position, position + iceMoveVelocity * 2, Color.red);

                // Debug.LogError($"moveDirection {iceMoveDirection.magnitude}");
                // 限制滑行速度
                // moveDirection = Vector3.ClampMagnitude(moveDirection, maxSlopeSpeed);
            }
        }

        // private void SlideMovement() {
        //     float horizontal = Input.GetAxis("Horizontal");
        //     float vertical = Input.GetAxis("Vertical");
        //     Vector3 inputDirection = transform.forward * vertical + transform.right * horizontal;
        //     if (inputDirection.magnitude > 0.1f) {
        //         // 增加加速度，使得在冰面上滑行的方向感更明显
        //         moveDirection += inputDirection.normalized * accelerationFactor * Time.deltaTime;
        //     } else {
        //         // 没有输入时逐渐减速
        //         var sliderFactor = Mathf.Pow(slideFriction, decelerationFactor * Time.deltaTime);
        //         moveDirection *= sliderFactor;
        //     }
        //
        //     if (hitNormal != Vector3.up) {
        //         var slideVelocity = hitNormal;
        //         slideVelocity.y = 0;
        //         moveDirection += slideVelocity * IceSlopeSlideSpeed * Time.deltaTime;
        //         moveDirection = Vector3.ClampMagnitude(moveDirection, maxSlopeSpeed);
        //     }
        // }

        // 相机朝向
        private void UpdateRotation() {
            var rot = transform.eulerAngles;
            rot.y = camaraTransform.eulerAngles.y;
            transform.eulerAngles = rot;
        }

        private void NormalMovement() {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            iceMoveVelocity = transform.forward * vertical + transform.right * horizontal;
            iceMoveVelocity *= normalMoveSpeed;
        }

        private void UpdateVerticalSpeed() {
            // if (isGrounded) {
            //     // if (verticalSpeed.y < 0) {
            //     //     verticalSpeed.y = gravity;
            //     // }
            //
            //     if (Input.GetKeyDown("space")) {
            //         verticalSpeed.y = jumpSpeed;
            //     }
            // } else {
            //     verticalSpeed.y += gravity * Time.deltaTime;
            // }
            // 重力持续增加版本
            verticalSpeed.y += gravity * Time.deltaTime;
            if (Input.GetKeyDown("space")) {
                verticalSpeed.y = jumpSpeed;
            }
            if (verticalSpeed.y < -20f) {
                verticalSpeed.y = -20f;
            }

            iceMoveVelocity.y = verticalSpeed.y;
        }

        private void OnTriggerEnter(Collider other) {
            isInIceZone = true;
        }

        private void OnTriggerExit(Collider other) {
            isInIceZone = false;
            iceMoveVelocity = Vector3.zero; // 离开冰面时停止滑动
        }

        private Vector3 slopeNormal;
        private void OnControllerColliderHit(ControllerColliderHit hit) {
            slopeNormal = hit.normal;
        }
    }
}
