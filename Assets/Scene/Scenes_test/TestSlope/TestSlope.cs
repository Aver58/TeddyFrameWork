#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class TestSlope : MonoBehaviour {
    CharacterController cc;
    float speed = 3.0f;　// 移動速度
    float rotate = 180.0f;　// 旋轉速度
    [Header("移动方向")] public Vector3 moveDirection = Vector3.zero;
    [Header("垂直方向")] public Vector3 vDir = Vector3.zero;
    [Header("是否落地")] public bool isGrounded = false;
    private float ratio = 1;//0.5f;
    private RaycastHit[] groundHits = new RaycastHit[10];
    private Action OnNextDrawGizmos;
    [Header("跳跃高度")] public float jumpH = 12.0f;　// 跳躍高度
    [Header("重力")] public float gravity = 10.0f;　// 重力
    public Vector3 slideDirection;//(0 ~ 1, 越接近1滑动越久)
    public bool isSliding = true;
    [Header("普通移动速度大小")] public float normalSpeed = 5.0f;
    [Header("滑行速度大小")] public float slideSpeed = 6f; // slide speed
    [Header("摩擦力系数")] public float slideFriction = 0.3f;
    [Header("最小滑行速度阈值")] public float stopThreshold = 0.1f;
    [Header("加速因子")] public float accelerationFactor = 5f;
    [Header("减速因子")] public float decelerationFactor = 2f;
    void Start() {
        cc = GetComponent<CharacterController>();

        // for (int i = 0; i < 10; i++) {
        //     Debug.LogError(GetRandomRoleAIWeaponType().ToString());
        // }
        // Debug.LogError(GetSchange(1200,1200, false, 4, 0, 32, 10, 1100));

        GetChestId(101010, out int parentLevel, out int subLevel, out int chestIndex);
        Debug.LogError(parentLevel);
        Debug.LogError(subLevel);
        Debug.LogError(chestIndex);

        // 创建一个 HashSet 存储 ChestID
        HashSet<ChestID> chestSet = new HashSet<ChestID>();

        // 添加一些示例数据
        chestSet.Add(new ChestID(1, 1, 1));
        chestSet.Add(new ChestID(1, 2, 3));
        chestSet.Add(new ChestID(2, 1, 4));

        // 传入一个 long 类型的 chestId
        long inputChestId = 10102;

        // 将 long chestId 转换为 ChestID 对象
        ChestID target = new ChestID(inputChestId);

        // 判断 HashSet 是否包含该对象
        bool contains = chestSet.Contains(target);
        Debug.LogError(contains);
    }

    private void Update() {
        SlopeUpdate();

        if (isSliding) {
            SlideMovement();
        } else {
            NormalMovement();
        }
    }

    private void OnDrawGizmos() {
        OnNextDrawGizmos?.Invoke();
        OnNextDrawGizmos = null;
    }

    public struct ChestID {
        public int ParentLevelIndex { get; set; }
        public int SubLevelIndex { get; set; }
        public int ChestIndex { get; set; }

        public ChestID(int parentLevelIndex, int subLevelIndex, int chestIndex) {
            ParentLevelIndex = parentLevelIndex;
            SubLevelIndex = subLevelIndex;
            ChestIndex = chestIndex;
        }

        public ChestID(long chestId) {
            var (parentLevelIndex, subLevelIndex, chestIndex) = ChestID.ParseChestId(chestId);
            ParentLevelIndex = parentLevelIndex;
            SubLevelIndex = subLevelIndex;
            ChestIndex = chestIndex;
        }

        public int GetChestId() {
            return ParentLevelIndex * 10000 + SubLevelIndex * 100 + ChestIndex;
        }

        // 比如：10101 ==》 1,1,1
        // chestID 转换规则：大关id * 10000 + 小关id * 100 + 宝箱id
        // chestId 转 大关id，小关id，宝箱id
        public static(int, int, int) ParseChestId(long chestId) {
            var parentLevel = (int) (chestId / 10000);
            var subLevel = (int) ((chestId % 10000) / 100);
            var chestIndex = (int) (chestId % 100);
            return (parentLevel, subLevel, chestIndex);
        }

        public override int GetHashCode() {
            return ParentLevelIndex * 10000 + SubLevelIndex * 100 + ChestIndex;
        }

        public override bool Equals(object obj) {
            return obj is ChestID key &&
                   ParentLevelIndex == key.ParentLevelIndex &&
                   SubLevelIndex == key.SubLevelIndex &&
                   ChestIndex == key.ChestIndex;
        }
    }

    public void GetChestId(long chestId, out int parentLevel, out int subLevel, out int chestIndex) {
        parentLevel = (int) (chestId / 10000);
        subLevel = (int) ((chestId % 10000) / 100);
        chestIndex = (int) (chestId % 100);
    }

    private bool isHit;
    private float angle;
    private Vector3 hitNormal;
    void OnControllerColliderHit (ControllerColliderHit hit) {
        hitNormal = hit.normal;
        // Debug.DrawLine(transform.position + hitNormal, transform.position + hitNormal * 2, Color.green);
    }

    // RA = A 玩家段位分
    // RB = B 玩家段位分
    // EA = 预期 A 玩家胜率
    // EB = 预期 B 玩家胜率
    // R'A = A 玩家段位变化
    // R'B = B 玩家段位变化
    // K = 系数极限值控制加减分峰值
    // EA = 1/(1+10^(RB-RA/400))
    // EB = 1/(1+10^(RA-RB/400))
    // E(A)+E(B)=1
    // 分数计算公式，Q 等于当局总回合数，不足 4 补为 4
    // A 赢：R'A = RA+K(1-EA)(1-Q/50)
    // A 输：R'A = RA-K(1-EB)(1-Q/50)
    public int GetSchange(int RA, int RB, bool AIsWin, int round, int totalTimes, float MatchFactorK, int MatchTimes, int MatchLowValue) {
        var k = MatchFactorK;
        var EA = 1 / (1 + Mathf.Pow(10, (RB - RA) / 400f));
        var EB = 1 / (1 + Mathf.Pow(10, (RA - RB) / 400f));
        if (round < 4) {
            round = 4;
        }

        var schange = 0;
        var newBeeFactor = 1f;
        if (totalTimes < MatchTimes) {
            newBeeFactor = 1.6f;
        }

        if (AIsWin) {
            if (RA < MatchLowValue) {
                k *= 1.2f;
            }
            schange = Mathf.RoundToInt(RA + newBeeFactor * k * (1 - EA) * (1 - round / 50f));
        } else {
            if (RA < MatchLowValue) {
                k *= 0.9f;
            }
            schange = Mathf.RoundToInt(RA - newBeeFactor * k * (1 - EB) * (1 - round / 50f));
        }

        Debug.LogError($"schange: {schange} RA:{RA} RB:{RB} AIsWin:{AIsWin} EA:{EA} EB:{EB} totalTimes {totalTimes}");
        return schange;
    }

    public static RoleAIWeaponType GetRandomRoleAIWeaponType() {
        var randomValue = GetRandomEnumExcluding<RoleAIWeaponType>(RoleAIWeaponType.ZiZiBeng);
        return randomValue;
    }

    public static T GetRandomEnumExcluding<T>(params T[] excludedValues) where T : Enum {
        var type = typeof(T);
        var enumValues = Enum.GetValues(type).Cast<T>();
        // 过滤掉被排除的值
        var filteredValues = enumValues.Except(excludedValues).ToArray();
        if (filteredValues.Length == 0) {
            throw new InvalidOperationException("AI武器没有剩余可供随机选择的枚举值！");
        }

        var random = new System.Random();
        int randomIndex = random.Next(filteredValues.Length);
        return (T)filteredValues.GetValue(randomIndex);
    }

    public enum RoleAIWeaponType : byte {
        // 突击步枪
        M416 = 1,
        AKM = 2,
        SCARL = 3,
        ParticleCannon = 41,    //只支持背着，暂时不支持使用
        ZiZiBeng = 42,          //只支持背着，暂时不支持使用,不能删除，有配置掉落
    }

    private void NormalMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(moveX,0,moveZ);
        cc.Move(moveDirection * normalSpeed * Time.deltaTime);
    }

    private void SlideMovement() {
         float horizontal = Input.GetAxis("Horizontal");
         float vertical = Input.GetAxis("Vertical");
         Vector3 inputDirection = transform.forward * vertical + transform.right * horizontal;
         if (inputDirection.magnitude > 0.1f) {
             // 增加加速度，使得在冰面上滑行的方向感更明显
             moveDirection += inputDirection.normalized * accelerationFactor * Time.deltaTime;
         } else {
             // 没有输入时逐渐减速
             moveDirection *= Mathf.Pow(slideFriction, Time.deltaTime * decelerationFactor);
         }

        Debug.LogError($"moveDirection: {moveDirection}");
        // slideDirection += moveDirection * slideSpeed * Time.deltaTime;
        // slideDirection = Vector3.Lerp(slideDirection, Vector3.zero, slideFriction * Time.deltaTime);
        // Debug.LogError($"Horizontal {moveX} Vertical {moveZ} slideDirection: {slideDirection} stopThreshold {stopThreshold}");
        cc.Move(moveDirection * Time.deltaTime);
    }

    // 斜坡功能检测
    private void SlopeUpdate() {
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
#endif