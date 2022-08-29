using UnityEngine;

namespace Origins.Entity {
    public class HeroEntity : Entity {
        private const string HERO_ACTOR_NAME = "HeroActor";
        public HeroActor HeroActor;

        public HeroEntity() {
            AddActor();
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