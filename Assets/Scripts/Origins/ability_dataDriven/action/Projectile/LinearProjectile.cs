namespace Battle.logic.ability_dataDriven {
    public class LinearProjectile : ProjectileEntity {
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