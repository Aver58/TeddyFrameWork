using Origins.Entity;
using UnityEngine;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private EntityManager entityManager;
        private GameMainLoop gameMainLoop;
        // Start is called before the first frame update
        void Awake() {
            gameMainLoop = GameMainLoop.instance;
            entityManager = EntityManager.instance;
        }

        // Update is called once per frame
        void Update()
        {
            gameMainLoop.OnUpdate();
            entityManager.OnUpdate();
        }
    }
}
