﻿using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // [MMORPG技能管线设计经验总结](https://baijiahao.baidu.com/s?id=1740830613356473547&wfr=spider&for=pc)
    // [MOBA 类游戏技能系统设计](https://nashnie.github.io/gameplay/2018/11/13/moba-like-skill-system-design.html)
    // [Dota2 创意工坊](http://maps.dota2.com.cn/custommap/cbook)
    // 良好的数据结构驱动
    // 逻辑以帧为单位
    // 可预览的技能编辑
    // DOTA2 技能系统，按着对配置表的理解，自己进行梳理逻辑
    public class Ability : ILifeCycle {
        public int AbilityLevel; // 技能等级
        public AbilityRequestContext AbilityRequestContext;

        private readonly AbilityConfig abilityConfig;
        
        private string description;
        private int fps;
        private float fixedDeltaTimeOfFps;
        private float currentTick;
        // todo 怎么以帧为单位跑逻辑
        private int currentFrame;
        private float cooldown;
        private bool isStartCd;

        private float backSwingPoint;
        // 技能表现部分，动作以及融合
        private AnimationClip animationClip;
        private bool applyRootMotion;
        private bool cancelable;
        private AbilityState abilityState;
        private AbsEntity caster; 

        private float baseDamage;
        public float BaseDamage {
            get {
                if (abilityConfig.AbilityDamage != null) {
                    baseDamage = abilityConfig.AbilityDamage[AbilityLevel - 1];
                }

                return baseDamage;
            }
        }

        #region LifeCycle
        
        public Ability(AbsEntity entity, AbilityConfig abilityConfig) {
            caster = entity;
            this.abilityConfig = abilityConfig;
        }

        public void OnInit() {
            // fps = targetFrameRate;
            AbilityLevel = 1;
            backSwingPoint = abilityConfig.AbilityCastPoint + abilityConfig.AbilityChannelTime;

            AbilityRequestContext = new AbilityRequestContext();
        }

        public void OnUpdate() {

        }

        public void OnClear() {

        }

        public void OnUpdate(float deltaTime) {
            // Debug.LogError($"cooldown{cooldown} currentTick {currentTick} {abilityConfig.AbilityCastPoint}  {backSwingPoint}  {abilityConfig.AbilityDuration}");
            if (abilityState == AbilityState.None && currentTick <= abilityConfig.AbilityCastPoint) {
                OnAbilityPhaseStart();
            }

            if (abilityState == AbilityState.CastPoint && currentTick >= abilityConfig.AbilityCastPoint && currentTick <= backSwingPoint) {
                OnSpellStart();
            }

            if (abilityState == AbilityState.Channeling && currentTick >= backSwingPoint && currentTick <= abilityConfig.AbilityDuration) {
                OnChannelFinish();
            }
            
            currentTick += deltaTime;
            if (cooldown > 0) {
                cooldown -= deltaTime;
            }
        }

        #endregion

        #region Public

        #region Get

        public bool IsCastable() {
            return cooldown <= 0;
        }

        public void CastAbility() {
            currentTick = 0;
            abilityState = AbilityState.None;

            OnAbilityPhaseStart();
        }
        
        public int GetAbilityDamage() {
            return -1;
        }

        public float GetAbilityCastRange() {
            return abilityConfig.AbilityCastRange;
        }

        public AbilityUnitTargetTeam GetTargetTeams() {
            return abilityConfig.AbilityUnitTargetTeam;
        }
        
        public AbilityUnitTargetType GetTargetType() {
            return abilityConfig.AbilityUnitTargetType;
        }

        public FindOrder GetFindOrder() {
            return abilityConfig.FindOrder;
        }
        
        #endregion
        
        public void SetRequestTarget(AbsEntity entity) {
            AbilityRequestContext.IsUnitRequest = true;
            AbilityRequestContext.RequestUnit = entity;
        }
        
        public void SetRequestTarget(Vector3 position) {
            AbilityRequestContext.IsUnitRequest = false;
            AbilityRequestContext.RequestPosition = position;
        }
        
        public void EndChannel(bool interrupted){ }
        public void EndCooldown(){ }
        
        #endregion
        
        #region Private

        private void OnAbilityPhaseStart() {
            // 前摇时间：
            //      技能开始，但是技能真正的结算流程还没开始
            //      技能开始以后，机能相关的特效和动作就开始播放
            abilityState = AbilityState.CastPoint;
            ExecuteEvent(AbilityEvent.OnAbilityPhaseStart);
        }

        private void OnSpellStart() {
            // 前摇时间结束：
            //      技能前摇结束时技能开始真正的释放以及结算，等技能前摇结束以后，技能真正的释放并结算
            //      释放包括创建相应的弹道／法术场和buff
            abilityState = AbilityState.Channeling;
            ExecuteEvent(AbilityEvent.OnSpellStart);
        }

        private void OnChannelFinish() {
            // 技能后摇点：
            //      技能播放到后摇点时间时，技能真正的结束。这时，技能对应的特效以及人物动作可能还会继续播放，但是技能流程已经正式结束了
            //      也就是说，下一个技能可以执行
            abilityState = AbilityState.CastBackSwing;
            ExecuteEvent(AbilityEvent.OnChannelFinish);
            
            cooldown = abilityConfig.AbilityCooldowns[AbilityLevel];
        }
        
        // 驱动事件
        private void ExecuteEvent(AbilityEvent abilityEvent) {
            if (abilityConfig.AbilityEventMap.ContainsKey(abilityEvent)) {
                var d2Event = abilityConfig.AbilityEventMap[abilityEvent];
                BattleLog.Log($"【ExecuteEvent】 技能名称：{abilityConfig.Name} 事件：{abilityEvent.ToString()}");
                d2Event.Execute(caster, AbilityRequestContext);
            }
        }

        #endregion
    }
}
