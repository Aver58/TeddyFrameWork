using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // [MMORPG技能管线设计经验总结](https://baijiahao.baidu.com/s?id=1740830613356473547&wfr=spider&for=pc)
    // [MOBA 类游戏技能系统设计](https://nashnie.github.io/gameplay/2018/11/13/moba-like-skill-system-design.html)
    // [Dota2 创意工坊](http://maps.dota2.com.cn/custommap/cbook)
    // 良好的数据结构驱动
    // 逻辑以帧为单位
    // 可预览的技能编辑
    // DOTA2 技能系统，按着对配置表的理解，自己进行梳理逻辑
    public class Ability {
        public int abilitylevel;// 技能等级
        
        private readonly AbilityConfig abilityConfig;
        
        private string description;
        private int fps;
        private float fixedDeltaTimeOfFps;
        private float currentTick;
        // todo 怎么以帧为单位跑逻辑
        private int currentFrame;
        private float cooldown;

        private float backSwingPoint;
        //技能表现部分，动作以及融合
        private AnimationClip animationClip;
        private bool applyRootMotion;
        private bool cancelable;
        private AbilityTarget abilityTarget;
        private AbilityState abilityState;
        private AbsEntity caster; 

        private float baseDamage;
        public float BaseDamage {
            get {
                if (abilityConfig.AbilityDamage != null) {
                    baseDamage = abilityConfig.AbilityDamage[abilitylevel - 1];
                }

                return baseDamage;
            }
        }

        #region LifeCycle
        
        public Ability(AbilityConfig abilityConfig) {
            this.abilityConfig = abilityConfig;
        }

        public void OnInit(int targetFrameRate) {
            fps = targetFrameRate;
            currentTick = 0;
            abilitylevel = 1;
            abilityState = AbilityState.None;
            cooldown = abilityConfig.AbilityCooldowns[abilitylevel];

            backSwingPoint = abilityConfig.AbilityCastPoint + abilityConfig.AbilityChannelTime;
        }
        
        public void OnFixedUpdate(float deltaTime) {
            cooldown -= deltaTime;
            currentTick += deltaTime;

            if (currentTick > 0 && currentTick < abilityConfig.AbilityCastPoint) {
                EnterCastPoint();
            }

            if (currentTick > abilityConfig.AbilityCastPoint && currentTick < backSwingPoint) {
                EnterChannel();
            }

            if (currentTick > backSwingPoint && currentTick < abilityConfig.AbilityDuration) {
                EnterBackSwing();
            }
        }

        #endregion

        #region Public

        public void SetCaster(AbsEntity entity) {
            caster = entity;
        }

        #endregion
        
        #region Private

        // 前摇时间：
        //      技能开始，但是技能真正的结算流程还没开始
        //      技能开始以后，机能相关的特效和动作就开始播放
        private void EnterCastPoint() {
            if (abilityState == AbilityState.None) {
                abilityState = AbilityState.CastPoint;
                ExecuteEvent(AbilityEvent.OnAbilityPhaseStart);
            }
        }

        // 前摇时间结束：
        //      技能前摇结束时技能开始真正的释放以及结算，等技能前摇结束以后，技能真正的释放并结算
        //      释放包括创建相应的弹道／法术场和buff
        private void EnterChannel() {
            if (abilityState == AbilityState.CastPoint) {
                abilityState = AbilityState.Channeling;
                ExecuteEvent(AbilityEvent.OnSpellStart);
            }
        }

        // 技能后摇点：
        //      技能播放到后摇点时间时，技能真正的结束。这时，技能对应的特效以及人物动作可能还会继续播放，但是技能流程已经正式结束了
        //      也就是说，下一个技能可以执行
        private void EnterBackSwing() {
            if (abilityState == AbilityState.Channeling) {
                abilityState = AbilityState.CastBackSwing;
                ExecuteEvent(AbilityEvent.OnChannelFinish);
            }
        }

        // 驱动事件
        private void ExecuteEvent(AbilityEvent abilityEvent)
        {
            if (abilityConfig.AbilityEventMap.ContainsKey(abilityEvent)) {
                var d2Event = abilityConfig.AbilityEventMap[abilityEvent];
                d2Event.Execute(caster);
            }
        }

        #endregion
    }
}
