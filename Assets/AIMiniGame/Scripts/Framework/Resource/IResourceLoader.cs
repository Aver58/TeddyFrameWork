using UnityEngine;

namespace AIMiniGame.Scripts.Framework.Resource {
    public interface IResourceLoader {
        void Initialize();
        void LoadAssetAsync<T>(string key, System.Action<T> onComplete) where T : Object;
        void UnloadAsset(string key);
        void UnloadAll();
    }
}