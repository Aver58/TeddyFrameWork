using UnityEngine;

namespace Origins {
    public class EnemyEntity : RoleEntity {
        public override void OnUpdate() {
            
        }

        public override void OnInit() {
            InstanceId = EntityManager.instance.AutoIndex++;
            InitProperty(RoleId);
            
            ActorManager.instance.GetActorAsync(this);
        }

        public override void OnClear() {
            
        }

        public void SetPosition(Vector2 value) {
            Position = value;
            ActorManager.instance.SetActorPosition(InstanceId, value);
        }

        private void InitProperty(int roleId) {
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