using AIMiniGame.Scripts.Framework.Resource;
public class ResourceManager : Singleton<ResourceManager> {
    private IResourceLoader m_loader;
    private ResourceCache m_cache = new ResourceCache();

    public ResourceManager() {
        switch (ResourceConfig.Instance.LoadMode) {
            case ResourceConfig.LoadModeEnum.Addressables:
                m_loader = new AddressableLoader();
                break;
            case ResourceConfig.LoadModeEnum.AssetBundle:
                m_loader = new AssetBundleLoader();
                break;
        }

        m_loader.Initialize();
    }

    public void LoadAssetAsync<T>(string key, System.Action<T> onComplete) where T : UnityEngine.Object {
        var cachedAsset = m_cache.Get<T>(key);
        if (cachedAsset != null) {
            onComplete?.Invoke(cachedAsset);
            return;
        }

        m_loader.LoadAssetAsync<T>(key, asset => {
            m_cache.Add(key, asset);
            onComplete?.Invoke(asset);
        });
    }

    public void UnloadAsset(string key) {
        m_loader.UnloadAsset(key);
        m_cache.Remove(key);
    }

    public void UnloadAll() {
        m_loader.UnloadAll();
        m_cache.Clear();
    }
}