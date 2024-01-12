using System;
using UnityEngine.UI;

namespace StarForce {
    public static class ImageExtension {
        public static void SetSprite(this Image image, string iconKey, bool setNativeSize = false) {
            if (image == null) {
                throw new NullReferenceException();
            }

            if (string.IsNullOrEmpty(iconKey)) {
                return;
            }

            // string assetPath = AssetUtility.GetUISpriteAsset(assetName);
            // var config = ImageConfig.Get(iconKey);
            // if (!config.HasValue) {
            //     Debug.LogErrorFormat("获取图片配置 iconKey:{0} 失败", iconKey);
            //     return;
            // }

        }
    }
}