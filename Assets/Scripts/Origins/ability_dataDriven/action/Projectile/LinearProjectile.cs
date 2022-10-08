using Origins;
using UnityEngine;

namespace Battle.logic.ability_dataDriven {
    public class LinearProjectile : ProjectileEntity {
        private bool dodgeable;
        private float moveSpeed;
        private float durning = 3f;
        private float flyTime;
        private Vector3 flyToward;

        public void OnInit(AbsEntity casterEntity, AbsEntity targetEntity, Vector3 sourcePosition, Vector3 sourceForward,
            AbilityRequestContext abilityRequestContext, string effectName, float moveSpeed, bool dodgeable) {
            
        }

        public override void OnUpdate() {
            base.OnUpdate();

            // if (moveSpeed > 0) {
            //     LocalPosition += flyToward * moveSpeed * Time.deltaTime;
            // }
            //
            // durning += Time.deltaTime;
            // if (flyTime >= durning) {
            //     FlyEnd();
            // }
        }

        private void FlyEnd() {
            OnClear();
        }
    }
}