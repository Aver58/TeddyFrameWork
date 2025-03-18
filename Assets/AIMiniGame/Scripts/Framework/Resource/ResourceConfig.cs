using UnityEngine;
using UnityEngine.Serialization;

namespace AIMiniGame.Scripts.Framework.Resource {
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "AIMiniGame/ResourceConfig")]
    public class ResourceConfig : ScriptableObject {
        public enum LoadModeEnum {
            Addressables,
            AssetBundle
        }

        public LoadModeEnum LoadMode = LoadModeEnum.Addressables;
        public string RemoteBaseURL; // 远程资源地址
        public string LocalAssetBundlePath; // 本地AssetBundle路径
        public string UIAssetPathPrefix; // UI资源路径前缀

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