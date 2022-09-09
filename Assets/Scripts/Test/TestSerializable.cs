using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "TestSerializable")]
public class TestSerializable : ScriptableObject {
    [Serializable]
    public struct MyStruct {
        public string test;
        // public SoWepEquipData.ModifierType test1;
        // public SoWepEquipData.ParamType test2;
        // public SoWepEquipData.Modifier test3;
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
    
    public SoWepEquipData[] testStruct;
    // public MyStruct testStruct1;
}
