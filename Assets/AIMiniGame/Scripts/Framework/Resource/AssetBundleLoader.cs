using System.Collections.Generic;
using UnityEngine;

namespace AIMiniGame.Scripts.Framework.Resource {
    public class AssetBundleLoader : IResourceLoader {
        private ResourceConfig m_resourceConfig;
        private Dictionary<string, AssetBundle> m_loadedBundles = new Dictionary<string, AssetBundle>();

        public void Initialize() {
            // 初始化本地或远程AssetBundle路径
            m_resourceConfig = ResourceConfig.Instance;
        }

        public void LoadAssetAsync<T>(string key, System.Action<T> onComplete) where T : Object {
            string bundleName = GetBundleName(key);
            string assetName = GetAssetName(key);

            if (m_loadedBundles.TryGetValue(bundleName, out AssetBundle bundle)) {
                LoadAssetFromBundle(bundle, assetName, onComplete);
            } else {
                LoadBundleAsync(bundleName, bundle => {
                    LoadAssetFromBundle(bundle, assetName, onComplete);
                });
            }
        }

        private void LoadBundleAsync(string bundleName, System.Action<AssetBundle> onComplete) {
            var request = AssetBundle.LoadFromFileAsync($"{m_resourceConfig.LocalAssetBundlePath}/{bundleName}");
            request.completed += operation => {
                var bundle = request.assetBundle;
                m_loadedBundles[bundleName] = bundle;
                onComplete?.Invoke(bundle);
            };
        }

        private void LoadAssetFromBundle<T>(AssetBundle bundle, string assetName, System.Action<T> onComplete) where T : Object {
            var request = bundle.LoadAssetAsync<T>(assetName);
            request.completed += operation => {
                onComplete?.Invoke(request.asset as T);
            };
        }

        public void UnloadAsset(string key) {
            string bundleName = GetBundleName(key);
            if (m_loadedBundles.TryGetValue(bundleName, out AssetBundle bundle)) {
                bundle.Unload(false);
                m_loadedBundles.Remove(bundleName);
            }
        }

        public void UnloadAll() {
            foreach (var bundle in m_loadedBundles.Values) {
                bundle.Unload(true);
            }

            m_loadedBundles.Clear();
        }

        private string GetBundleName(string key) => key.Split('/')[0];
        private string GetAssetName(string key) => key.Split('/')[1];
    }
}