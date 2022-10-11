using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class LinearProjectile : ProjectileEntity {
        private bool dodgeable;
        private float moveSpeed;
        private float durning = 3f;
        private float flyTime;
        private Vector3 forward;

        public void OnInit(AbsEntity casterEntity, AbsEntity targetEntity, Vector3 sourcePosition, Vector3 sourceForward,
            AbilityRequestContext abilityRequestContext, string effectName, float moveSpeed, bool dodgeable) {
            base.OnInit(casterEntity, targetEntity, sourcePosition, sourceForward, abilityRequestContext, effectName);

            this.moveSpeed = moveSpeed;
            this.dodgeable = dodgeable;
            forward = sourceForward;
        }

        public override void OnUpdate() {
            base.OnUpdate();

            if (moveSpeed > 0) {
                Position += forward * moveSpeed * Time.deltaTime;
                GameMsg.instance.DispatchEvent(GameMsgDef.OnProjectileActorMoveTo, InstanceId, Position, LocalForward);
            }
            
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