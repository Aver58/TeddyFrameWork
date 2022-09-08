using UnityEngine;

namespace Origins {
    public class EnemyEntity : AbsEntity {
        public EnemyEntity(int roleId) {
            RoleId = roleId;
            InstanceId = EntityManager.instance.AutoIndex++;
        }
        
        public override void OnUpdate() {
            
        }

        public override void OnInit() {
            InitProperty(RoleId);
            
            ActorManager.instance.GetActorFromPool(this);
        }

        public override void OnClear() {
            
        }

        public void SetPosition(Vector2 value) {
            Position = value;
            ActorManager.instance.SetActorPosition(InstanceId, value);
        }
        
        protected override void InitProperty(int roleId) {
            var config = EnemyConfigTable.Instance.Get(roleId);
            MaxHp = config.maxHp;
            MaxMp = config.maxMp;
            Hp = MaxHp;
            Mp = MaxMp;
            PhysicsAttack = config.physicAttack;
            MoveSpeed = config.moveSpeed;
            AttackCooldown = config.attackCooldown;
        }
    }
}