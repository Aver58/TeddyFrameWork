using AIMiniGame.Scripts.Bussiness.Controller;
using AIMiniGame.Scripts.TetrisGame;
using UnityEngine;

public class GameEngine : MonoBehaviour {
    private void Start() {
        // 加载资源示例
        // ResourceManager.Instance.LoadAssetAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/MyPrefab.prefab", obj => {
        //     if (obj != null) {
        //         var go = Instantiate(obj);
        //         go.transform.localPosition = Vector3.zero;
        //     }
        // });

        // 卸载资源示例
        // ResourceManager.Instance.UnloadAsset("Assets/AIMiniGame/ToBundle/Prefabs/MyPrefab.prefab");

        // 注册事件示例
        // EventManager.Instance.Register<string>(EventConstantId.OnTestEvent, OnTestEvent);
        // EventManager.Instance.Register<string>(EventConstantId.OnTestEvent, OnTestEvent2);

        // 触发事件示例
        // EventManager.Instance.Dispatch(EventConstantId.OnTestEvent, "Hello, World!");
        // EventManager.Instance.Unregister<string>(EventConstantId.OnTestEvent, OnTestEvent);
        // EventManager.Instance.Dispatch(EventConstantId.OnTestEvent, "Hello, World!2");

        // 加载配置示例
        // TestConfig();

        // 加载界面示例
        // UIManager.Instance.OpenUI<TestView>();

        GameWorld gameWorld = new GameWorld();
        gameWorld.Init();
        ControllerManager.Instance.OpenAsync<PetController>();
    }

    private void OnTestEvent2(string message) {
        Debug.Log($"Received event message2: {message}");
    }

    void OnDestroy() {
        // 注销事件示例
        // if (EventManager.Instance == null) {
        //     return;
        // }
        //
        // EventManager.Instance.Unregister<string>(EventConstantId.OnTestEvent, OnTestEvent);
    }

    private void OnTestEvent(string message) {
        Debug.Log("Received event message: " + message);
    }

    private void TestConfig() {
        var keys = TestFileConfig.GetKeys();
        for (int i = 0; i < keys.Count; i++) {
            var key = keys[i];
            Debug.LogError(key);
            var config = TestFileConfig.Get(key);

            Debug.LogError(config.id);
            Debug.LogError(config.gameTitles);
            Debug.LogError(config.maxScores);
            Debug.LogError(config.playerSpeeds);
        }
    }
}