using System;
using UnityEngine;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private GameMainLoop gameMainLoop;
        private EntityManager entityManager;
        private ActorManager actorManager;

        private void Awake() {
            gameMainLoop = GameMainLoop.instance;
            entityManager = EntityManager.instance;
            actorManager = ActorManager.instance;
        }

        private void Update() {
            gameMainLoop.OnUpdate();
            entityManager.OnUpdate();
            actorManager.OnUpdate();
        }

        private void FixedUpdate() {
            gameMainLoop.OnFixedUpdate();
        }
    }
}
