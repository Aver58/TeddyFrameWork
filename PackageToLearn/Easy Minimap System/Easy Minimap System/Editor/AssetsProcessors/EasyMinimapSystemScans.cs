using UnityEngine;
using UnityEditor;

public class EasyMinimapSystemScans : AssetPostprocessor
{
    //Class that configures automatic import of scans. Only images imported into the scans directory will be set.
    //This code is executed automatically by the editor whenever an asset is imported.

    void OnPreprocessTexture()
    {
        if (assetPath.ToLower().IndexOf("/mt assets/_assetsdata/scans/") == -1)
        {
            return;
        }

        TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (textureImporter != null)
        {
            textureImporter.textureType = TextureImporterType.Default;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
            textureImporter.alphaIsTransparency = true;
            textureImporter.maxTextureSize = 8192;
            textureImporter.wrapMode = TextureWrapMode.Clamp;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.mipmapEnabled = true;
        }
    }
}