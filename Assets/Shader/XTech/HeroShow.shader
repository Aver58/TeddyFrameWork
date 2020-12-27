Shader "XTech/HeroShow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_GlobalLight ("Global Light Color(全局光颜色)", Color) = (1,1,1,1)

		_LightTex ("Environments Texture(环境光贴图)", 2D) = "black" {}
		_LightPower ("Environments Power (环境光强度)", Range(0, 10)) = 1
		
		_LightPos ("Light Direction(方向光方向)", Vector) = (1,1,1,1)
		_LightColor ("Light Color(方向光颜色)", Color) = (1,1,1,1)

		_Gloss ("Gloss(高光范围)", Range(0, 100)) = 1
		_SpecularBright ("Specualr Bright(高光强度)", Range(0, 2)) = 1

		_MaskTex("Mask Textue", 2D) = "white" {}
		_Bright ("Light Bright", Range(0, 10)) = 2

		_HeightColor ("Height Color(高度渐变颜色)", Color) = (1,1,1,1)
		_HeightControl ("Height Control(高度渐变控制)", Range(0, 1)) = 0

		_ColorControl("Color Control", Range(0, 1)) = 0.5

		_Contrast("Contrast(对比度)", Range(0, 2)) = 1
		_Saturation("Saturation(饱和度)", Range(0, 2)) = 1
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Assets/Shader/AL.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _GlobalLight;

			sampler2D _LightTex;

			float4 _LightPos;
			float4 _LightColor;

			float _Gloss;

			float _LightPower;
			float _Bright;
			float _SpecularBright;

			sampler2D _MaskTex;

			float4 _HeightColor;
			float _HeightControl;

			float _ColorControl;
			float _Contrast;
			float _Saturation;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(UNITY_MATRIX_M, v.vertex);
				o.worldPos.w = 1 - v.vertex.x;
				o.uv.zw = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal)).xy * 0.5 + 0.5;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex, i.uv.xy) * _GlobalLight;

				// ---------------------------
				fixed4 maskCol = tex2D(_MaskTex, i.uv.xy);
				fixed3 evirCol = tex2D(_LightTex, i.uv.zw).rgb * _LightPower * maskCol.r;

				fixed3 N = normalize(i.worldNormal.xyz);
				fixed3 L = normalize(-_LightPos.xyz);
				fixed3 V = normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));
				fixed3 H = normalize(L + V);

				float NdotL = saturate(dot(N, L));

				fixed3 specular = _LightColor.rgb * mainCol.rgb * pow(saturate(dot(N, H)), _Gloss) * maskCol.r * _SpecularBright;

				float heightOffset = i.worldPos.w  + _HeightControl - N.z;
				fixed3 heightCol = saturate(lerp(_HeightColor.rgb, fixed3(1,1,1), heightOffset));

				fixed3 resultCol = (mainCol.rgb + evirCol + specular) * _Bright;

				fixed3 shadowCol = mainCol.rgb * NdotL;

				fixed3 col = lerp(shadowCol, resultCol, _ColorControl) * heightCol;

				fixed3 luminanceColor = Luminance(col);

				col = lerp(luminanceColor, col, _Saturation);

				fixed3 avgColor = fixed3(0.5, 0.5, 0.5);  

				col = lerp(avgColor, col, _Contrast);  
			
				fixed4 lastColor = fixed4(col.rgb + specular, 1);
				return GetBrightnessColor(lastColor);

			}

			ENDCG
		}
	}
}