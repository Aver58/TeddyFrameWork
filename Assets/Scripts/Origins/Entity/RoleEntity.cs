using UnityEngine;

namespace Origins {
    public class RoleEntity : Entity {
        public RoleActor RoleActor;
        public GameDefine.RoleType RoleType;
        
        public RoleEntity(int roleId) {
            RoleId = roleId;
            InstanceId = EntityManager.instance.AutoIndex++;
        }

        public override void OnUpdate() {
            RoleActor.OnUpdate();
        }

        public sealed override void OnInit() {
            InitProperty(RoleId);
            InitActor();
        }

        public override void OnClear() {
            
        }

        #region Public

        public void SetPosition(Vector2 value) {
            Position = value;
            RoleActor.SetPosition(value);
        }

        public void SetRoleType(GameDefine.RoleType value) {
            RoleType = value;
        }

        #endregion

        #region Private

        protected sealed override void InitProperty(int roleId) {
            var config = CharacterTable.Instance.Get(roleId);
            Hp = config.maxHp;
            Mana = config.magic;
            MoveSpeed = config.moveSpeed;
        }
        
        protected sealed override void InitActor() {
            var characterItem = CharacterTable.Instance.Get(RoleId);
            if (characterItem == null) {
                return;
            }

            var asset = LoadModule.Instance.LoadPrefab(characterItem.modelPath);
            if (asset != null) {
                var go = Object.Instantiate(asset, UIModule.Instance.GetParentTransform(ViewType.MAIN));
                go.transform.localPosition = Position;
                RoleActor = go.GetComponent<RoleActor>();
                if (RoleActor != null) {
                    RoleActor.OnInit();
                    RoleActor.SetEntity(this);
                }
            }
        }

        #endregion
    }
}