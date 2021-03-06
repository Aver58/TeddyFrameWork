﻿using UnityEngine;
using UnityEngine.UI;

/// 负责“弹出窗体”模态显示实现
/// https://blog.csdn.net/SaberZG/article/details/109347619
/// 
public class BlurTextureOnce : MonoBehaviour
{
    private Material m_blurMaterial;
    private RenderTexture m_renderBuffer;
    private RawImage m_rawImage;
    private Camera m_camera;

    public int DownSampleNum = 5;
    public float BlurSize = 0.7f;

    public void Init()
    {
        m_rawImage = gameObject.GetComponent<RawImage>();
        m_blurMaterial = m_rawImage.material;
        m_camera = GameObject.Find("UICamera").GetComponent<Camera>(); // 就一个相机，可以直接用，多个的话要多个渲染的结果
    }

    private RenderTexture GetRenderTexture()
    {
        // 首先对输出的结果做一次降采样，也就是降低分辨率，减小RT图的大小
        int width = (int)m_rawImage.rectTransform.rect.width / DownSampleNum;
        int height = (int)m_rawImage.rectTransform.rect.height / DownSampleNum;
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        rt.name = "UIBlurTextureOne RT";

        Camera camera = m_camera;
        RenderTexture temp = camera.targetTexture;
        camera.targetTexture = rt;
        // 将当前摄像机画面渲染到目标RT上
        camera.Render();
        camera.targetTexture = temp;

        return rt;
    }

    void BlurRender(RenderTexture sourceTexture)
    {
        int width = sourceTexture.width;
        int height = sourceTexture.height;
        RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, sourceTexture.format);
        m_blurMaterial.SetFloat("_blurSize", BlurSize + 1.2f);
        // 将sourceTexture位块传输到temporary上
        Graphics.Blit(sourceTexture, temporary, m_blurMaterial);
   
        RenderTexture.ReleaseTemporary(sourceTexture);
        m_renderBuffer = temporary;
        m_rawImage.texture = m_renderBuffer;
    }

    public void GenerateRender()
    {
        m_rawImage.enabled = false;
        RenderTexture rt = GetRenderTexture();
        BlurRender(rt);
        m_rawImage.enabled = true;
    }

    public void ReleaseRender()
    {
        if(m_renderBuffer != null)
            RenderTexture.ReleaseTemporary(m_renderBuffer);

        m_rawImage.texture = null;
    }
}
