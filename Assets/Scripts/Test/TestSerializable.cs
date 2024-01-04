using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TestSerializable")]
public class TestSerializable : ScriptableObject {
    [Serializable]
    public struct MyStruct {
        public string test;
        public SoWepEquipData.ModifierType test1;
        public SoWepEquipData.ParamType test2;
        public SoWepEquipData.Modifier test3;
        // public SoWepEquipData.EquipEffect test4;
    }

    [Serializable]
    public struct SoWepEquipData {
        public enum ModifierType {
            BulletNum, // 子弹数量
            BulletSpeed, // 子弹速度
            BulletPowerUpRatio, // 子弹伤害提高百分比
            ReloadRatio, // 换弹时间
            ShootRangeRatio, // 扩散范围 (最远处距离的扩散范围 像素)
            RecoilRatio, // 减少上升角度百分比(垂直后坐力)
            MaxLeftRightRecoilRatio, // 最大水平向左
            CameraRatioLevel, // 镜头瞄准默认等级
            MinCameraRatioLevel, // 可调节倍镜的最低等级
            AimDownRatio, // 开镜倍率权重
            SoundRatio, // 声音大小 0 - 1
            ShowFireEffect, // 火花显示
            IsSilencerAudio, // 开枪声音
            DownRecoilRatio, // 增加回落百分比
            PressWeaponLength, // 收枪检测长度
            FireDeployTimeUpRatio, // 射击间隔提高比例
            FireBulletNum, // 散弹子弹数量
            MaxDelayTimeBulletNum, // 射击间隔达到最小值需要的子弹数(加速机枪前摇时间降低百分比)
            LocalTuoWeiEffect, // 拖尾特效
            FireEffectMove, // 其他人显示的拖尾特效
            ZiZiZi, // 滋滋泵只滋不泵
        }

        public enum ParamType {
            Int,
            Float,
            String,
            Bool,
        }

        [Serializable]
        public struct Modifier {
            public ModifierType ModifierType;
            public ParamType ParamType;
            public string Param;
        }

        public string WeaponEquipId;
        public string PI_Data;
        public string SkinSign;
        public int PackValue;
        public Modifier[] Modifiers;
    }

    [Serializable]
    public struct MyStruct1 {
        public List<Vector3> t;
    }

    // public SoWepEquipData[] testStruct;
    // public List<Vector3> ItemTransforms;
    // public List<MyStruct1> ItemTransformsMap;
    // public MyStruct testStruct1;

    #region Serialize Field

    [Serializable]
    public struct OverrideClip {
        public string OldClipName;
        public string NewClipName;
    }

    public class SkinBase {
        public string Sign;
        public OverrideClip[] OverrideClips;
    }

    [Serializable]
    public class GuanYuSkin : SkinBase {
        [Tooltip("冲刺跟随特效")] public string SprintFollowEffect;
        [Tooltip("冲刺时留在原地的特效")] public string SprintOriginEffect;
        [Tooltip("冲刺过程中镜头的特效")] public string SprintCameraEffect;
        [Tooltip("进入音效")] public string EnterSound;
        [Tooltip("进入特效")] public string SpinEnterEffect;
        [Tooltip("出生特效")] public string SpinStartEffect;
        [Tooltip("持续特效")] public string SpinContinueEffect;
        [Tooltip("武器特效")] public string SpinWeaponEffect;
        [Tooltip("结束特效")] public string SpinEndEffect;
        [Tooltip("进入音效")] public string GuanyuSpinEnterSound;
    }

    [Serializable]
    public class NinjaSkin : SkinBase {

    }

    [Serializable]
    public class AidMeiSkin : SkinBase {
        [Tooltip("添加特效")] public string RangeEffect;
        [Tooltip("爆炸特效")] public string BombEffect;
        // [Tooltip("消失特效")] public BuffSOBase ClearEffect;
        [Tooltip("持续特效")] public string AidMeiBotLoop;
        [Tooltip("机器人")] public string RobtEffect;
        [Tooltip("治疗机器人预制")] public string PrefebRobt;
        [Tooltip("治疗机器人落地音效")] public string Sound;
    }

    public GuanYuSkin[] GuanYuSkins;
    // public List<NinjaSkin> NinjaSkins;
    public AidMeiSkin[] AidMeiSkins;

    // itemID==》实例字段名称
    private static Dictionary<long, string> SOIdCardSkinMap = new Dictionary<long, string> {
        {1207959552, "GuanYuSkins"},
        {1107296291, "NinjaSkins"},
        {436207619, "AidMeiSkins"},
    };

    #endregion

    private static readonly string SOIdCardSkinConfigPath = "Assets/Scripts/Test/TestSerializable.asset";
    private static TestSerializable sOIdCardSkinConfig;
    public static TestSerializable SOIdCardSkinConfigInstance {
        get {
            if (sOIdCardSkinConfig == null) {
#if UNITY_EDITOR
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TestSerializable>(SOIdCardSkinConfigPath);
                sOIdCardSkinConfig = asset;
#endif
            }

            return sOIdCardSkinConfig;
        }
    }

    private const string Sign = "Sign";
    private const string OverrideClips = "OverrideClips";
    private static Type sOIdCardSkinConfigType = typeof(TestSerializable);
    // 传入 itemId 和 idCardSign、Key（字段名），拿到 string
    public static OverrideClip[] GetClips<T>(long itemId, string idCardSign) {
        if (!SOIdCardSkinMap.ContainsKey(itemId)) {
            Debug.LogError("【SOIdCardSkinConfig】没有配置指定道具的信息：itemId：" + itemId);
            return null;
        }

        string fieldName = SOIdCardSkinMap[itemId];
        var fieldInfo = sOIdCardSkinConfigType.GetField(fieldName);
        if (fieldInfo == null) {
            Debug.LogError("【SOIdCardSkinConfig】没有指定字段名：fieldName：" + fieldName);
            return null;
        }

        var skins = fieldInfo.GetValue(SOIdCardSkinConfigInstance);
        if (skins != null) {
            T[] skinArray = (T[])skins;
            var type = typeof(T);
            for (int i = 0; i < skinArray.Length; i++) {
                var skin = skinArray[i];
                var skinSignFieldInfo = type.GetField(Sign);
                if (skinSignFieldInfo == null) {
                    Debug.LogError("【SOIdCardSkinConfig】没有指定字段名：Sign：" + Sign);
                    continue;
                }

                var skinSign = (string)skinSignFieldInfo.GetValue(skin);
                if (skinSign == idCardSign) {
                    var skinKeyFieldInfo = type.GetField(OverrideClips);
                    if (skinKeyFieldInfo == null) {
                        Debug.LogError("【SOIdCardSkinConfig】没有指定字段名：OverrideClips：" + OverrideClips);
                        continue;
                    }

                    var clipValue = skinKeyFieldInfo.GetValue(skin);
                    if (clipValue.GetType() == typeof(OverrideClip[])) {
                        Debug.LogError(clipValue);
                        return (OverrideClip[])clipValue;
                    }

                    Debug.LogError("【SOIdCardSkinConfig】找到的字段不是 OverrideClip[] 类型！");
                }
            }
        }

        return null;
    }


    [ContextMenu("MyItem")]
    void OnClickMenuItem() {
        GetClips<GuanYuSkin>(1207959552, "GuanYu");
    }
}
