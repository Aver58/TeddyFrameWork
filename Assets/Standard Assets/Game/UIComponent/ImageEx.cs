using UnityEngine;
using UnityEngine.UI;

public class ImageEx : Image
{
    public void SetSprite(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            this.sprite = null;
        }
        else
        {
            var path = Util.GetSpritePath(name);
            LoadModule.LoadAsset(path, typeof(Sprite), OnLoadComplete);
        }
    }

    private void OnLoadComplete(AssetRequest request)
    {
        if(!string.IsNullOrEmpty(request.error))
        {
            request.Release();
            return;
        }

        this.sprite = request.asset as Sprite;
    }
}
