using UnityEngine;

namespace Origins {
    public class EnemyEntity : RoleEntity {
        public override void OnInit() {
            BattleCamp = BattleCamp.ENEMY;
            InitProperty(RoleId);
            
            ActorManager.instance.GetActorAsync(this);
        }

        public void SetLocalPosition(Vector2 value) {
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