using UnityEngine;

namespace Origins.Entity {
    public class HeroEntity : Entity {
        private const string HERO_ACTOR_NAME = "HeroActor";
        public HeroActor HeroActor;

        public HeroEntity() {
            InitProperty();
            AddActor();
        }

        private void InitProperty() {
            var config = NPCPropertyTable.Instance.GetTableItem(103);
            Hp = config.maxHp;
            Mana = config.magic;
            MoveSpeed = config.moveSpeed;
        }

        private void AddActor() {
            var asset = LoadModule.Instance.LoadPrefab(HERO_ACTOR_NAME);
            var go = Object.Instantiate(asset, UIModule.Instance.GetParentTransform(ViewType.MAIN));
            var rigidBody = go.GetComponent<Rigidbody2D>();
            HeroActor = new HeroActor();
            HeroActor.Init(this, rigidBody);
        }

        public override void OnUpdate() {
            HeroActor.OnUpdate();
        }
    }
}