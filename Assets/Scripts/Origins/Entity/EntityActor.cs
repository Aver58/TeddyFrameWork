using UnityEngine;

namespace Origins.Entity {
    public class EnemyActor : Actor {
        public Entity Entity;

        private Transform mTransform;
        private Rigidbody2D rigidbody2D;

        public EnemyActor(EnemyEntity enemyEntity, Rigidbody2D rigidbody) {
            rigidbody2D = rigidbody;
            mTransform = rigidbody.transform;

            Entity = enemyEntity;
        }

        public override void OnUpdate() {

        }
    }
}