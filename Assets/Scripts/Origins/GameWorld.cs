using System;
using UnityEngine;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private EntityManager entityManager;
        private GameMainLoop gameMainLoop;

        private void Awake() {
            gameMainLoop = GameMainLoop.instance;
            entityManager = EntityManager.instance;
        }

        private void Update() {
            gameMainLoop.OnUpdate();
            entityManager.OnUpdate();
        }

        private void FixedUpdate() {
            gameMainLoop.OnFixedUpdate();
        }
    }
}
