using System.Collections.Generic;

[System.Serializable]
public class AH_SerializableAssetInfo
{
    public string ID;
    public List<string> Refs;

    public AH_SerializableAssetInfo()
    { }

    public AH_SerializableAssetInfo(string assetPath, List<string> scenes)
    {
        this.Refs = scenes;
        this.ID = UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
    }
}