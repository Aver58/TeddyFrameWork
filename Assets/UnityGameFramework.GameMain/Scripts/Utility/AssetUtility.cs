public static class AssetUtility {
    public static string GetUIFormAsset(string assetName)
    {
        return GameFramework.Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
    }
}