using Origins.Entity;
using UnityEngine;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private EntityManager entityManager;
        private GameMainLoop gameMainLoop;
        void Awake() {
            gameMainLoop = GameMainLoop.instance;
            entityManager = EntityManager.instance;
        }

        void Update()
        {
            gameMainLoop.OnUpdate();
            entityManager.OnUpdate();
        }
    }
}
