using UnityEngine;

namespace AIMiniGame.Scripts.Framework.Resource {
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "AIMiniGame/ResourceConfig")]
    public class ResourceConfig : ScriptableObject {
        public enum LoadMode {
            Addressables,
            AssetBundle
        }

        public LoadMode loadMode = LoadMode.Addressables;
        public string remoteBaseURL; // 远程资源地址
        public string localAssetBundlePath; // 本地AssetBundle路径

        private static ResourceConfig instance;
        public static ResourceConfig Instance {
            get {
                if (instance == null) {
                    instance = Resources.Load<ResourceConfig>("ResourceConfig");
                }

                return instance;
            }
        }

    }
}