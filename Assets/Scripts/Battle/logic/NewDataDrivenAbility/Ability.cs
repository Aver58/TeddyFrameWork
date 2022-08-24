using UnityEngine;

namespace Battle.logic.NewDataDrivenAbility {
    // [MOBA 类游戏技能系统设计](https://nashnie.github.io/gameplay/2018/11/13/moba-like-skill-system-design.html)
    // 良好的数据结构驱动;
    // 逻辑以帧为单位;
    // 可预览的技能编辑;
    public class Ability {
        private string description;
        //技能帧率
        private int fps;
        private float fixedDeltaTimeOfFps;
        private float currentTick;
        private int currentFrame;
        //技能表现部分，动作以及融合
        private AnimationClip animationClip;
        private bool applyRootMotion;
        private bool cancelable;
        private AbilityConfig abilityConfig;

        private AbilityState abilityState;

        #region LifeCycle

        private void OnInit() {
            currentTick = 0;
            currentFrame = 0;
            abilityState = AbilityState.None;
            fixedDeltaTimeOfFps = 1 / fps;
            
            
        }
        
        private void OnFixedUpdate() {
            currentTick += fixedDeltaTimeOfFps;
            while (currentFrame < currentTick) {
                currentFrame++;
            }

            // 施法状态切换
            if (abilityState == AbilityState.None) {
                 
            }
        }

        #endregion

        #region Private

        private void EnterCastPoint() {
            
        }

        
        
        #endregion
    }
}
