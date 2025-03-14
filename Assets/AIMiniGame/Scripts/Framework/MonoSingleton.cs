using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
    private static T instance;
    private static readonly object lockObject = new object();
    private static bool isApplicationQuitting = false;

    public static T Instance {
        get {
            if (isApplicationQuitting) {
                Debug.LogWarning($"[Singleton] Instance {typeof(T)} already destroyed on application quit. Won't create again - returning null.");
                return null;
            }

            lock (lockObject) {
                if (instance == null) {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null) {
                        var singletonObject = new GameObject();
                        instance = singletonObject.AddComponent<T>();
                        singletonObject.name = typeof(T).ToString() + " (Singleton)";
                        DontDestroyOnLoad(singletonObject);
                    }
                }

                return instance;
            }
        }
    }

    protected virtual void OnDestroy() {
        if (instance == this) {
            isApplicationQuitting = true;
        }
    }
}