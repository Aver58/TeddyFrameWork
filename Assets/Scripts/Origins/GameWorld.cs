using UnityEngine;

namespace Origins {
    public class GameWorld : MonoBehaviour {
        private BattleUnitManager battleUnitManager;
        private GameMainLoop gameMainLoop;
        // Start is called before the first frame update
        void Start() {
            gameMainLoop = GameMainLoop.instance;
            battleUnitManager = BattleUnitManager.instance;
        }

        // Update is called once per frame
        void Update()
        {
            gameMainLoop.OnUpdate();
            battleUnitManager.OnUpdate();
        }
    }
}
