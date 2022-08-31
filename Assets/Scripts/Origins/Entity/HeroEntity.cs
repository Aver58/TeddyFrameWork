using UnityEngine;

namespace Origins.Entity {
    public class HeroEntity : Entity {
        public HeroActor Actor;

        public HeroEntity(int characterId) : base(characterId) {
            AddActor();
        }

        private void AddActor() {
            var characterItem = CharacterTable.Instance.Get(CharacterId);
            if (characterItem == null) {
                return;
            }

            var asset = LoadModule.Instance.LoadPrefab(characterItem.modelPath);
            if (asset != null) {
                var go = Object.Instantiate(asset, UIModule.Instance.GetParentTransform(ViewType.MAIN));
                var rigidBody = go.GetComponent<Rigidbody2D>();
                Actor = new HeroActor();
                Actor.Init(this, rigidBody);
            }
        }

        public override void OnUpdate() {
            Actor.OnUpdate();
        }
    }
}