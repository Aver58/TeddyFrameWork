Shader "Unlit/BlurShader"
{
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}

	}

	SubShader{

		Pass{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;

			float4 _MainTex_ST;
			float4 _MainTex_TexelSize;
			float _blurSize;

			struct a2v {
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 offsetuv[4] : TEXCOORD1;
			};

			v2f vert(a2v IN) {
				v2f o;
				o.pos = UnityObjectToClipPos(IN.vertex);
				o.uv = TRANSFORM_TEX(IN.uv, _MainTex);

				float2 offsetWeight[4] = {
					float2(0, 1),
					float2(-1, 0),
					float2(1, 0),
					float2(0, -1)
				};

				o.offsetuv[0] = o.uv.xy + float2(offsetWeight[0].x, offsetWeight[0].y) * _MainTex_TexelSize * _blurSize;
				o.offsetuv[1] = o.uv.xy + float2(offsetWeight[1].x, offsetWeight[1].y) * _MainTex_TexelSize * _blurSize;
				o.offsetuv[2] = o.uv.xy + float2(offsetWeight[2].x, offsetWeight[2].y) * _MainTex_TexelSize * _blurSize;
				o.offsetuv[3] = o.uv.xy + float2(offsetWeight[3].x, offsetWeight[3].y) * _MainTex_TexelSize * _blurSize;

				return o;
			}

			fixed4 frag(v2f IN) : SV_TARGET{

				fixed4 col = fixed4(0, 0, 0, 0);
				col += tex2D(_MainTex, IN.offsetuv[0]);
				col += tex2D(_MainTex, IN.offsetuv[1]);
				col += tex2D(_MainTex, IN.offsetuv[2]);
				col += tex2D(_MainTex, IN.offsetuv[3]);

				col *= 0.25;
				col.a = 1;

				return col;
			}

			ENDCG
		}
	}
}
