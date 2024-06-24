using UnityEngine;

public class GameEngine : MonoBehaviour {
    public GameObject Canvas;
    private void Start() {
        // 加载资源示例
        ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AIMiniGame/ToBundle/Prefabs/MyPrefab.prefab", obj => {
            if (obj != null) {
                var go = Instantiate(obj, Canvas.transform, true);
                go.transform.localPosition = Vector3.zero;
            }
        });

        // 注册事件示例
        EventManager.Instance.Register<string>(EventConstantId.OnTestEvent, OnTestEvent);

        // 触发事件示例
        EventManager.Instance.Trigger(EventConstantId.OnTestEvent, "Hello, World!");

        // 加载配置示例
        // TestConfig();
    }

    void OnDestroy() {
        // 注销事件示例
        EventManager.Instance.Unregister<string>(EventConstantId.OnTestEvent, OnTestEvent);
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