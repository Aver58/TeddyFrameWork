using UnityEngine;
using UnityEngine.UI;

public class BlurTextureOnce : MonoBehaviour
{
    private string m_blurShaderAssetPath = "Assets/Shader/Unlit/BlurShader.shader";
    private string m_blurShaderName = "Unlit/BlurShader";
    private Shader m_blurShader;
    private Material m_blurMaterial;
    private RenderTexture m_renderBuffer;
    private RawImage m_rawImage;
    private Camera m_camera;

    //对输出的结果做一次降采样，也就是降低分辨率，减小RT图的大小
    public int DownSampleNum = 5;
    public float BlurSize = 0.7f;

    public void Init()
    {
        //var assetRequest = LoadModule.LoadAsset(m_blurShaderAssetPath,typeof(Shader));
        //m_blurShader = assetRequest.asset as Shader;
        //if(m_blurShader == null)
        //    Debug.LogError("Shader[Unlit/BlurShader] is null...");

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
        //rt.filterMode = FilterMode.Bilinear;
        rt.name = "UIBlurTextureOne RT";
        //rt.Create();
        //rt.DiscardContents();

        // 相机输出
        Camera camera = m_camera;
        RenderTexture temp = camera.targetTexture;
        camera.targetTexture = rt;
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
        // 将当前摄像机画面渲染到目标RT上
        Graphics.Blit(sourceTexture, temporary, m_blurMaterial, 0);
        sourceTexture.DiscardContents();
        Graphics.Blit(temporary, sourceTexture, m_blurMaterial, 0);

        // 我们只是想获得摄像机的画面，所以完事之后别忘了把画面正常输出出去
        //Graphics.Blit(src, dest);

        //temporary.filterMode = FilterMode.Bilinear;
        //temporary.Create();
        //temporary.filterMode = FilterMode.Bilinear;

        //for(int i = 0; i < 2; i++)
        //{
        //    float iterationOffs = i * 1.2f;
        //    m_blurMaterial.SetFloat("_blurSize", BlurSize + iterationOffs);
        //    //垂直模糊
        //    temporary.DiscardContents();
        //    Graphics.Blit(src, temporary, m_blurMaterial, 0);
        //    src.DiscardContents();
        //    Graphics.Blit(temporary, src, m_blurMaterial, 0);
        //}
        RenderTexture.ReleaseTemporary(temporary);
        m_renderBuffer = sourceTexture;
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
