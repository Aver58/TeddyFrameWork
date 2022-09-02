using UnityEngine;

namespace Origins {
    public class HeroEntity : AbsEntity {
        public HeroEntity(int roleId) {
            RoleId = roleId;
            InstanceId = EntityManager.instance.AutoIndex++;
        }

        public override void OnUpdate() {
            
        }

        public sealed override void OnInit() {
            InitProperty(RoleId);
            
            var actor = ActorManager.instance.AddHeroActor(this);
        }

        public override void OnClear() {
            
        }

        #region Public

        public void SetPosition(Vector2 value) {
            Position = value;
        }

        #endregion

        #region Private

        protected override void InitProperty(int roleId) {
            var config = CharacterTable.Instance.Get(roleId);
            Hp = config.maxHp;
            Mana = config.magic;
            MoveSpeed = config.moveSpeed;
        }

        #endregion
    }
}