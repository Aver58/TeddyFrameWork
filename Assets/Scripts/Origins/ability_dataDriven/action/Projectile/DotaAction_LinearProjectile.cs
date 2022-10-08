using System;
using JetBrains.Annotations;
using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    // 目标,效果名称,移动速度,开始范围,结束范围,固定距离,开始位置,目标队伍,目标类型,目标标签,前方锥形,提供视野,视觉范围
    // Target, EffectName, MoveSpeed, StartRadius, EndRadius, FixedDistance, StartPosition, TargetTeams, TargetTypes, TargetFlags, HasFrontalCone, ProvidesVision, VisionRadius
    public sealed class DotaAction_LinearProjectile : DotaAction {
        // Require 必须参数
        [NotNull] private readonly string effectName;
        [NotNull] private readonly string sourceAttachment;
        private readonly bool dodgeable;
        private readonly float moveSpeed;

        // Optional 可选参数
        private bool providesVision;//提供视野
        private float visionRadius;//视野范围
        private bool hasFrontalCone;//前方锥形
        private float startRadius;
        private float endRadius;

        private Vector2 startPosition;
        private float fixedDistance;//固定距离

        public DotaAction_LinearProjectile(AbilityTarget abilityTarget, [NotNull] string effectName,
            [NotNull] string sourceAttachment, bool dodgeable, float moveSpeed) : base(abilityTarget) {
            this.effectName = effectName ?? throw new ArgumentNullException(nameof(effectName));
            this.sourceAttachment = sourceAttachment ?? throw new ArgumentNullException(nameof(sourceAttachment));
            this.dodgeable = dodgeable;
            this.moveSpeed = moveSpeed;
        }

        public void SetVisionParam(bool providesVision, float visionRadius) {
            this.providesVision = providesVision;
            this.visionRadius = visionRadius;
        }

        protected override void ExecuteByUnit(AbsEntity entity, AbsEntity targetEntity, AbilityRequestContext abilityRequestContext) {
            if (ProjectileManager.instance.GetProjectile(ProjectileType.Linear) is LinearProjectile linearProjectile) {
                abilityRequestContext.SetRequestUnit(targetEntity);
                linearProjectile.OnInit(entity, targetEntity, entity.Position, entity.Forward, abilityRequestContext, effectName, moveSpeed, dodgeable);
            }        
        }
    }
}