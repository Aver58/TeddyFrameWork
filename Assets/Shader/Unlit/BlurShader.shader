Shader "Unlit/BlurShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {} // 主纹理，我们进行模糊处理的对象就是它
        _BlurSize("BlurSize", Range(0, 127)) = 1.0 // 对周边采样的偏移量
    }
    SubShader
    {
        CGINCLUDE
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            half4 _MainTex_TexelSize;
            fixed _BlurSize;

            struct v2f {
                float4 pos : SV_POSITION;
                half2 uv[5] : TEXCOORD0;
            };

            // 水平uv数据扩展采样
            v2f vert_hor(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                half2 uv = v.texcoord;

                // 下标从0开始,分别取到当前像素，偏移1单位和2单位的uv位置
                o.uv[0] = uv;
                o.uv[1] = uv + half2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;
                o.uv[2] = uv - half2(_MainTex_TexelSize.x * 1.0, 0.0) * _BlurSize;

                o.uv[3] = uv + half2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;
                o.uv[4] = uv - half2(_MainTex_TexelSize.x * 2.0, 0.0) * _BlurSize;

                return o;
            }

            // 水平uv数据扩展采样
            v2f vert_ver(appdata_img v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                half2 uv = v.texcoord;

                // 下标从0开始,分别取到当前像素，偏移1单位和2单位的uv位置
                o.uv[0] = uv;
                o.uv[1] = uv + half2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;
                o.uv[2] = uv - half2(0.0, _MainTex_TexelSize.y * 1.0) * _BlurSize;

                o.uv[3] = uv + half2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;
                o.uv[4] = uv - half2(0.0, _MainTex_TexelSize.y * 2.0) * _BlurSize;

                return o;
            }

            // 处理模糊片元
            fixed4 frag(v2f i) : SV_TARGET
            {
                // 模糊算子，分别决定了当前像素，上下（左右）偏移1个单位和2个单位的计算权重
                // 算子决定了模糊的质量，算子越大越复杂效果越好，当然性能上就要差一些
                half weight[3] = {0.4026, 0.2442, 0.0545};

                // 当前像素片元颜色（乘以权重）
                fixed3 color = tex2D(_MainTex, i.uv[0]).rgb * weight[0];
                // 根据权重叠加上下（左右）像素颜色
                color += tex2D(_MainTex, i.uv[1]).rgb * weight[1];
                color += tex2D(_MainTex, i.uv[2]).rgb * weight[1];
                color += tex2D(_MainTex, i.uv[3]).rgb * weight[2];
                color += tex2D(_MainTex, i.uv[4]).rgb * weight[2];

                return fixed4(color, 1.0);
            }
        ENDCG

        Cull Off
        ZWrite Off
        Pass // 0:处理水平模糊
        {
            Name "BLUR_HORIZONTAL"
            CGPROGRAM
            #pragma vertex vert_hor
            #pragma fragment frag
            ENDCG
        }

        Pass // 1:处理垂直模糊
        {
            Name "BLUR_VERTICAL"
            CGPROGRAM
            #pragma vertex vert_ver
            #pragma fragment frag
            ENDCG
        }
    }
    Fallback Off
}
