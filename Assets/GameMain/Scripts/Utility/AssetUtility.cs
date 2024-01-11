using GameFramework;

public static class AssetUtility {
    public static string GetUIFormAsset(string assetName)
    {
        return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
    }

    // todo ui资源收集工具接入
    public static string GetUISpriteAsset(string assetName)
    {
        return Utility.Text.Format("Assets/GameMain/UI/UISprites/{0}.png", assetName);
    }
    
    
    public static string GetDataTableAsset(string assetName, bool fromBytes)
    {
        return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
    }

}