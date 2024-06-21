//https://zhuanlan.zhihu.com/p/647256794
Shader "Unlit/InfiniteGridShader"
{
    Properties
    {
        _fadeDivisor ("Fade Divisor", Float) = 20
        _yOffset ("Y Offset", Float) = 10
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
            ZWrite Off
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
                float3 nearPoint : TEXCOORD0;
                float3 farPoint : TEXCOORD1;
            };

            float _fadeDivisor;
            float _yOffset;

            float3 TransformHClipToWorld(float3 positionCS, float4x4 inv_VP) {
                float4 unprojectedPoint = mul(inv_VP, float4(positionCS, 1.0));
                float3 result = unprojectedPoint.xyz / unprojectedPoint.w;
                result.y += _yOffset;
                return result;
            }

            Varings vert(Attributes input){
                //此shader专用于quad mesh
                //所以使用4个顶点的uv值进行变换 作为 裁切空间的坐标
                //保证这是一个覆盖全屏幕的渲染
                Varings o;
                float2 uv = input.uv * 2.0 - 1.0;
                //默认情况下，Zndc = 1是远平面
                half farPlane = 1;
                half nearPlane = 0;

                #if defined(UNITY_REVERSED_Z)
                    //有时候会反转z
                    farPlane = 1 - farPlane;
                    nearPlane = 1 - nearPlane;
                #endif

                float4 position = float4(uv, farPlane, 1);
                float3 nearPoint = TransformHClipToWorld(float3(position.xy, nearPlane), UNITY_MATRIX_I_VP);
                float3 farPoint = TransformHClipToWorld(float3(position.xy, farPlane), UNITY_MATRIX_I_VP);
                o.positionCS = position;
                o.nearPoint = nearPoint;
                o.farPoint = farPoint;
                return o;
            }

            half Grid(float2 uv){
                float2 derivative = fwidth(uv);
                uv = frac(uv - 0.5); //中心对齐
                uv = abs(uv - 0.5);
                uv = uv / derivative;
                float min_value = min(uv.x, uv.y);
                half grid = 1.0 - min(min_value, 1.0);
                return grid;
            }

            float computeViewZ(float3 pos) {
                float4 clip_space_pos = mul(UNITY_MATRIX_VP, float4(pos.xyz, 1.0));
                float viewZ = clip_space_pos.w; //根据projection矩阵定义，positionCS.w = viewZ
                return viewZ;
            }

            half4 frag(Varings input) : SV_TARGET{
                //计算地平面
                float t = -input.nearPoint.y / (input.farPoint.y - input.nearPoint.y);
                float3 positionWS = input.nearPoint + t * (input.farPoint - input.nearPoint);
                half ground = step(0, t);

                float3 cameraPos = _WorldSpaceCameraPos;
                float fromOrigin = abs(cameraPos.y);

                float viewZ = computeViewZ(positionWS);
                float2 uv = positionWS.xz;
                //计算grid
                float fading = max(0.0, 1.0 - viewZ / _fadeDivisor);
                half smallGrid = Grid(uv) * lerp(1, 0, min(1.0, fromOrigin / 100));
                half middleGrid  = Grid(uv * 0.1) * lerp(1, 0, min(1.0, fromOrigin / 300));
                half largeGrid = Grid(uv * 0.01) * lerp(1, 0, min(1.0, fromOrigin / 800));

                //合并计算
                half grid = smallGrid + middleGrid + largeGrid;
                return half4(0.5, 0.5, 0.5 , ground * grid * fading * 0.5); //顺便改色减淡一下
            }
            ENDHLSL
        }
    }
}