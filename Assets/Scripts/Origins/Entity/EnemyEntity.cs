using UnityEngine;

namespace Origins.Entity {
    public class EnemyEntity : Entity {
        public EnemyActor Actor;
        // todo 寻路
        public EnemyEntity(int characterId) : base(characterId) {
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

                go.transform.localPosition = Position;
                Actor = new EnemyActor(this, rigidBody);
            }
        }

        public override void OnUpdate() {
            Actor.OnUpdate();
        }

    }
}