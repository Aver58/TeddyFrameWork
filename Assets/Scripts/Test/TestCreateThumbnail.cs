using System.IO;
using UnityEngine;

public class TestCreateThumbnail : MonoBehaviour {
    public Camera targetCamera; // 需要截取的摄像机
    public int thumbnailWidth = 320; // 略缩图的宽度
    public int thumbnailHeight = 180; // 略缩图的高度
    public string savePath = "Assets/thumbnail.png"; // 保存路径

    void Start() {
        CaptureThumbnail();
    }

    void CaptureThumbnail() {
        // 创建RenderTexture
        RenderTexture rt = new RenderTexture(thumbnailWidth, thumbnailHeight, 24);
        targetCamera.targetTexture = rt;
        // 渲染到RenderTexture
        targetCamera.Render();

        // 激活目标RenderTexture
        RenderTexture.active = rt;

        // 创建Texture2D，并读取像素
        Texture2D thumbnail = new Texture2D(thumbnailWidth, thumbnailHeight, TextureFormat.RGB24, false);
        thumbnail.ReadPixels(new Rect(0, 0, thumbnailWidth, thumbnailHeight), 0, 0);
        thumbnail.Apply();

        // 保存略缩图为PNG文件
        SaveThumbnail(thumbnail);

        // 释放RenderTexture
        RenderTexture.active = null;
        targetCamera.targetTexture = null;
        Destroy(rt);
    }

    void SaveThumbnail(Texture2D thumbnail) {
        // 将Texture2D保存为PNG文件
        byte[] bytes = thumbnail.EncodeToPNG();
        File.WriteAllBytes(savePath, bytes);
        Debug.Log("略缩图已保存：" + savePath);
    }
}