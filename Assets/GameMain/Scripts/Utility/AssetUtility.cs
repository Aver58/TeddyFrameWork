using GameFramework;

public static class AssetUtility {
    public static string GetUIFormAsset(string assetName)
    {
        return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
    }

    // todo ui资源收集工具接入
    public static string GetUIAtlasAsset(string assetName)
    {
        return Utility.Text.Format("Assets/GameMain/UI/UIAtlas/{0}.spriteatlas", assetName);
    }
    
    
    public static string GetDataTableAsset(string assetName, bool fromBytes)
    {
        return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
    }

    public static string GetEntityAsset(string assetName)
    {
        return Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
    }
    
}