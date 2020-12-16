using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBlurEffect : MonoBehaviour
{
    private string m_blurShaderAssetPath = "Assets/Shader/Unlit/BlurShader.shader";
    private string m_blurShaderName = "Unlit/BlurShader";
    private Shader m_blurShader;
    private Material m_blurMaterial;
    private RenderTexture m_renderBuffer;
    private RawImage m_rawImage;

    //对输出的结果做一次降采样，也就是降低分辨率，减小RT图的大小
    public int DownSampleNum = 5;
    public float BlurSize = 0.7f;

    void Start()
    {
        var assetRequest = LoadModule.LoadAsset(m_blurShaderAssetPath,typeof(Shader));
        m_blurShader = assetRequest.asset as Shader;
        if(m_blurShader == null)
            Debug.LogError("Shader[Unlit/BlurShader] is null...");

        m_rawImage = gameObject.GetComponent<RawImage>();
        m_blurMaterial = m_rawImage.material;

    }

    private int OrderCamera(Camera camera1, Camera camera2)
    {
        float num = camera1.depth - camera2.depth;
        if(num < 0f)
        {
            return -1;
        }
        if(num > 0f)
        {
            return 1;
        }
        return 0;
    }

    private RenderTexture GetRenderTexture(bool isChanging = false)
    {
        // 首先对输出的结果做一次降采样，也就是降低分辨率，减小RT图的大小
        int width = (int)m_rawImage.rectTransform.rect.width / DownSampleNum;
        int height = (int)m_rawImage.rectTransform.rect.height / DownSampleNum;
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 30, RenderTextureFormat.ARGB32);
        rt.filterMode = FilterMode.Bilinear;
        rt.name = "UIBlurTextureOne RT";
        rt.Create();
        rt.DiscardContents();

        if(isChanging)
            m_rawImage.enabled = false;

        //var currentPosition = this.transform.position;
        //if(ParentPopumMaskTransform)
        //{
        //    ParentPopumMaskTransform.parent.position = new Vector3(currentPosition.x, currentPosition.y + 10000, currentPosition.z);
        //}

        Camera[] allCameras = Camera.allCameras;
        Array.Sort<Camera>(allCameras, new Comparison<Camera>(this.OrderCamera));
        Camera[] array = allCameras;
        for(int i = 0; i < array.Length; i++)
        {
            Camera camera = array[i];
            RenderTexture temp = camera.targetTexture;
            camera.targetTexture = rt;
            camera.Render();
            camera.targetTexture = temp;
        }

        //if(ParentPopumMaskTransform)
        //{
        //    ParentPopumMaskTransform.parent.position = currentPosition;
        //}

        if(isChanging)
            m_rawImage.enabled = true;

        return rt;
    }

    void BlurRender(RenderTexture sourceTexture)
    {
        int rtW = sourceTexture.width;
        int rtH = sourceTexture.height;
        RenderTexture temporary = RenderTexture.GetTemporary(rtW, rtH, 0, sourceTexture.format);
        temporary.filterMode = FilterMode.Bilinear;
        temporary.Create();
        temporary.filterMode = FilterMode.Bilinear;

        // float widthMod = 1.0f / (1.0f * (1 << DownSampleNum));
        for(int i = 0; i < 2; i++)
        {
            float iterationOffs = i * 1.2f;
            m_blurMaterial.SetFloat("_blurSize", BlurSize + iterationOffs);
            //垂直模糊
            temporary.DiscardContents();
            Graphics.Blit(sourceTexture, temporary, m_blurMaterial, 0);
            sourceTexture.DiscardContents();
            Graphics.Blit(temporary, sourceTexture, m_blurMaterial, 0);
        }
        RenderTexture.ReleaseTemporary(temporary);
        m_renderBuffer = sourceTexture;
        m_rawImage.texture = m_renderBuffer;
    }

    public void GenerateRender()
    {
        RenderTexture rt = GetRenderTexture(true);
        BlurRender(rt);
    }

    public void ReleaseRender()
    {
        if(m_renderBuffer != null)
            RenderTexture.ReleaseTemporary(m_renderBuffer);

        m_rawImage.texture = null;
    }
}
