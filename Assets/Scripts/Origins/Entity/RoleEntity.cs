namespace Origins {
    public class RoleEntity : AbsEntity {
        public int RoleId;

        private float hp;
        public float Hp {
            get => hp;
            set {
                hp = value;
            }
        }

        public float Mp;
        public int MaxHp;
        public int MaxMp;
        public int PhysicsAttack;
        public int Defense;
        public float MoveSpeed;
        public float AttackCooldown;

        public override void OnInit() {
            
        }

        public override void OnUpdate() {
            
        }

        public override void OnClear() {
            
        }

        public virtual void TakeDamage(float value) {}
    }
}