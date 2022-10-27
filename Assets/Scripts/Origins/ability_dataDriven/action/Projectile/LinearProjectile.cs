using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class LinearProjectile : ProjectileEntity {
        private bool dodgeable;
        private float moveSpeed;
        private float durning = 3f;
        private float flyTime;
        private Vector2 forward;

        public void OnInit(AbsEntity casterEntity, AbsEntity targetEntity, Vector2 sourcePosition, Vector2 sourceForward,
            AbilityRequestContext abilityRequestContext, string effectName, float moveSpeed, bool dodgeable) {
            base.OnInit(casterEntity, targetEntity, sourcePosition, sourceForward, abilityRequestContext, effectName);

            this.moveSpeed = moveSpeed;
            this.dodgeable = dodgeable;
            forward = sourceForward;
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (moveSpeed > 0) {
                LocalPosition += forward * moveSpeed * Time.deltaTime;
                GameMsg.instance.DispatchEvent(GameMsgDef.OnProjectileActorMoveTo, InstanceId, LocalPosition, LocalForward);
            }
            
            // 碰撞 矩形与圆的相交
            // Physics2D.BoxCastNonAlloc();
            
            durning += Time.deltaTime;
            if (flyTime >= durning) {
                FlyEnd();
            }
        }

        private void FlyEnd() {
            OnClear();
            
            GameMsg.instance.DispatchEvent(GameMsgDef.OnProjectileActorDestroy, InstanceId);
        }
    }
}