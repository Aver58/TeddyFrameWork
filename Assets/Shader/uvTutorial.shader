Shader "Unlit/uvTutorial"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags{
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "Queue"="Transparent"
        }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite off
            Cull off
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            struct Attributes{
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varings{
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _fragSize;
            float _frageCenter;

            Varings vert(Attributes input){
                Varings o;
                o.positionCS = TransformObjectToHClip(input.positionOS);
                o.uv = input.uv;
                return o;
            }

            half4 frag(Varings input) : SV_TARGET{
                float2 uv = input.uv * 10;
                float2 derivative = fwidth(uv);
                uv = frac(uv);
                uv = abs(uv - 0.5);
                uv = uv / derivative; //这里开始进行因子缩放，d越大，uv越小，更有可能有“正alpha值”
                float min_value = min(uv.x, uv.y);
                half alpha = 1.0 - min(min_value, 1.0); //实际的线框是靠近x,y轴的，
                //远离x, y轴的uv值大，min后等于1，取反等于0
                return half4(1.0, 0.0, 0.0, alpha);
            }
            ENDHLSL
        }
    }
}