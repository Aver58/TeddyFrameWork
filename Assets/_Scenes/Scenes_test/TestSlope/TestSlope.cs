using System;
using UnityEditor;
using UnityEngine;

public class TestSlope : MonoBehaviour {
    CharacterController cc;
    float speed = 3.0f;　// 移動速度
    float rotate = 180.0f;　// 旋轉速度
    float jumpH = 12.0f;　// 跳躍高度
    float gravity = 10.0f;　// 重力
    private float slideFriction = 0.3f;
    private float slideSpeed = 6f; // slide speed
    Vector3 moveDirection = Vector3.zero, vDir = Vector3.zero;
    private bool isGrounded = false;
    void Start() {
        cc = GetComponent<CharacterController>();
    }

    private Action OnNextDrawGizmos;

    private void OnDrawGizmos() {
        OnNextDrawGizmos?.Invoke();
        OnNextDrawGizmos = null;
    }

    private bool isHit;
    private float angle;
    private bool isSliding;

    private Vector3 hitNormal;
    void OnControllerColliderHit (ControllerColliderHit hit) {
        hitNormal = hit.normal;
        // Debug.DrawLine(transform.position + hitNormal, transform.position + hitNormal * 2, Color.green);
    }
    
    private float ratio = 1;//0.5f;
    private RaycastHit[] groundHits = new RaycastHit[10];

    private void Update() {
        GroundCheck();
        
        var inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection = transform.TransformDirection(inputDirection);
        moveDirection *= speed;

        // var origin = cc.transform.position;
        // origin.y += cc.height / 2;
        // ratio += 0.01f;
        // if (ratio>=1) {
        //     ratio = 0.5f;
        // }
        // var radius = cc.radius;
        // var maxDistance = cc.height - cc.radius + 0.01f;
        // Array.Clear(groundHits, 0, groundHits.Length);
        // var point1 = cc.transform.position;
        // point1.y += cc.height / 2;
        // var point2 = cc.transform.position;
        // point2.y -= cc.height / 2;
        // 注意，胶囊投射不检测胶囊重叠的碰撞器。
        // Physics.CapsuleCastNonAlloc(point1, point2, radius,Vector3.down, groundHits, maxDistance, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        // Physics.SphereCastNonAlloc(origin, radius,Vector3.down, groundHits, maxDistance , LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        // int len = groundHits.Length;
        // for (int i = 0; i < len; i++) {
        //     var hit = groundHits[i];
        //     if (hit.transform != null) {
        //         var capsuleCenterPoint = cc.transform.position;
        //         capsuleCenterPoint.y -= cc.height / 2 - cc.radius;
        //
        //         OnNextDrawGizmos += () => {
        //             Handles.Label(transform.position + new Vector3(0 , 2,0), "hits:" + hit.transform.name);
        //             Handles.Label(transform.position + new Vector3(0 , 1.9f,0), "hitNormal:" + hitNormal);
        //         };
        //         if (hitNormal != Vector3.up) {
        //             moveDirection += new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z) * slideSpeed;
        //             break;
        //         }
        //     }
        // }
      
        var isLimitAngle = (Vector3.Angle (Vector3.up, hitNormal) <= cc.slopeLimit);
        // 斜坡并且落地了，才滑动
        if (!isLimitAngle) {
        }

        // var factor = (1 - slideFriction) * (1 - hitNormal.y) * slideSpeed;
        // moveDirection.x += hitNormal.x * factor;
        // moveDirection.z += hitNormal.z * factor;
        // moveDirection += new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z) * slideSpeed;
        
        Array.Clear(groundHits, 0, groundHits.Length);
        var origin = cc.transform.position;
        origin.y -= cc.height / 2 - cc.radius;
        var radius = cc.radius * 1.2f;
        var maxDistance = cc.height/2;
        Physics.SphereCastNonAlloc(origin, radius,Vector3.down, groundHits, maxDistance, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore);
        int len = groundHits.Length;
        for (int i = 0; i < len; i++) {
            var hit = groundHits[i];
            if (hit.transform != null) {
                OnNextDrawGizmos += () => {
                    var position = transform.position;
                    Handles.Label(position + new Vector3(0 , 2,0), "hits:" + hit.transform.name);
                    Handles.Label(position + new Vector3(0 , 1.9f,0), "hitNormal:" + hit.normal);
                    Debug.DrawLine(position + hit.normal, position + hit.normal * 2, Color.green);
                    
                    Gizmos.DrawWireSphere(origin, radius);
                    origin.y -= maxDistance;
                    Gizmos.DrawWireSphere(origin, radius);
                };
            }
        }
    
        vDir.y -= gravity * Time.deltaTime;
        if (Input.GetKeyDown("space")) {
            vDir.y = jumpH;
        }

        moveDirection += vDir;
        
        OnNextDrawGizmos += () => {
            Handles.Label(transform.position + new Vector3(0 , 1f,0), "isGrounded:" +isGrounded);
            Handles.Label(transform.position + new Vector3(0 , 1.5f,0), "moveDirection:" +moveDirection);
        };
        
        cc.Move(moveDirection * Time.deltaTime);
    }
    
    private void GroundCheck() {
        isGrounded = cc.isGrounded;
        
        // if (isGrounded && moveDirection.y < 0.0f) {
        //     moveDirection.y = -2.0f;
        // }
    } 
}
