using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AIMiniGame.Scripts.Framework.Resource {
    public class AddressableLoader : IResourceLoader {
        public void Initialize() {
            Addressables.InitializeAsync();
        }

        public void LoadAssetAsync<T>(string key, System.Action<T> onComplete) where T : Object {
            Addressables.LoadAssetAsync<T>(key).Completed += handle => {
                if (handle.Status == AsyncOperationStatus.Succeeded) {
                    onComplete?.Invoke(handle.Result);
                } else {
                    Debug.LogError($"【LoadAssetAsync】Failed to load asset: {key}");
                }
            };
        }

        public void UnloadAsset(string key) {
            Addressables.Release(key);
        }

        public void UnloadAll() {
            // Addressables 自动管理资源释放
        }
    }
}