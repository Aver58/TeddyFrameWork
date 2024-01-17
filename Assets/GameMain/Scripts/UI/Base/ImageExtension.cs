using System;
using System.Collections.Generic;
using GameFramework.Resource;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public static class ImageExtension {
    public static void SetSprite(this Image image, string assetName, bool setNativeSize = false) {
        if (image == null) {
            throw new NullReferenceException();
        }

        if (string.IsNullOrEmpty(assetName)) {
            return;
        }

        var config = UISpriteConfig.Get(assetName);
        if (config == null) {
            Debug.LogErrorFormat("获取图片配置 iconKey:{0} 失败", assetName);
            return;
        }

        var atlasName = GetSpriteName(config.AtlasId);
        var spriteName = config.SpriteName;
        if (string.IsNullOrEmpty(spriteName) || string.IsNullOrEmpty(atlasName)) {
            return;
        }

        LoadSpriteAsync(image, spriteName);
    }

    private static string GetSpriteName(int atlasId) {
        var atlasConfig = UIAtlasConfig.Get(atlasId);
        return atlasConfig?.AtlasName;
    }

    private static readonly Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
    private static readonly Dictionary<string, SpriteAtlas> atlasMap = new Dictionary<string, SpriteAtlas>();

    private static void LoadSpriteAsync(Image image, string spriteName) {
        if (!UISpriteConfig.Has(spriteName)) {
            return;
        }

        var config = UISpriteConfig.Get(spriteName);
        if (config == null) {
            return;
        }

        if (sprites.ContainsKey(spriteName)) {
            image.sprite = sprites[spriteName];
        }

        var atlasName = GetSpriteName(config.AtlasId);
        if (atlasMap.TryGetValue(atlasName, out var atlas) && atlas != null) {
            sprites[spriteName] = atlas.GetSprite(spriteName);
            image.sprite = sprites[spriteName];
        }

        var atlasPath = AssetUtility.GetUIAtlasAsset(atlasName);
        GameEntry.Resource.LoadAsset(atlasPath, new LoadAssetCallbacks((assetName, asset, duration, userData) => {
            if (asset != null && asset is SpriteAtlas spriteAtlas) {
                atlasMap[atlasName] = spriteAtlas;
                sprites[spriteName] = spriteAtlas.GetSprite(spriteName);
                image.sprite = sprites[spriteName];
            }
        }));
    }
}