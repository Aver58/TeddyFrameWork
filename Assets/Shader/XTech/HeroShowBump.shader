Shader "XTech/HeroShowBump"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_NormalTex ("Normal Texture", 2D) = "bump" {}
		_GlobalLight ("Global Light Color(全局光颜色)", Color) = (1,1,1,1)

		_LightTex ("Environments Texture(环境光贴图)", 2D) = "black" {}
		_LightPower ("Environments Power (环境光强度)", Range(0, 6)) = 1
		
		_LightPos ("Light Direction(方向光方向)", Vector) = (1,1,1,1)
		_LightColor ("Light Color(方向光颜色)", Color) = (1,1,1,1)

		_Gloss ("Gloss(高光范围)", Range(0, 200)) = 1
		_SpecularBright ("Specualr Bright(高光强度)", Range(0, 20)) = 1

		_MaskTex("Mask Textue", 2D) = "black" {}
		_Bright ("Light Bright", Range(0, 10)) = 2
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" "LightMode"="ForwardBase"}
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "Assets/Shader/AL.cginc"
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;

				float3 worldNormal : TEXCOORD1;
				float3 worldTangent : TEXCOORD2;
				float3 worldBinormal : TEXCOORD3;
				//float3 TtoW0 : TEXCOORD1;
				//float3 TtoW1 : TEXCOORD2;
				//float3 TtoW2 : TEXCOORD3;

				float4 worldPos : TEXCOORD4;
				float3 lightDir : TEXCOORD5;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _NormalTex;

			fixed4 _GlobalLight;

			sampler2D _LightTex;

			float4 _LightPos;
			float4 _LightColor;

			float _Gloss;

			float _LightPower;
			float _Bright;
			float _SpecularBright;

			sampler2D _MaskTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv.xy = TRANSFORM_TEX(v.uv, _MainTex);

				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
           		o.worldBinormal = cross(o.worldNormal, o.worldTangent) * v.tangent.w;
				//float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				//float3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
				//float3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
				//o.TtoW0 = float3(worldTangent.x, worldBinormal.x, worldNormal.x);
				//o.TtoW1 = float3(worldTangent.y, worldBinormal.y, worldNormal.y);
				//o.TtoW2 = float3(worldTangent.z, worldBinormal.z, worldNormal.z);

				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldPos.w = 1 - v.vertex.x;
				o.uv.zw = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal)).xy * 0.5 + 0.5;

				float4 lightDir = float4(normalize(_LightPos.xyz), 1.0);
				o.lightDir = mul(UNITY_MATRIX_I_V, lightDir).xyz - _WorldSpaceCameraPos.xyz;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 mainCol = tex2D(_MainTex, i.uv.xy) * _GlobalLight;

				// ---------------------------
				fixed4 maskCol = tex2D(_MaskTex, i.uv.xy);
				fixed3 evirCol = tex2D(_LightTex, i.uv.zw).rgb * _LightPower * maskCol.r;
				//return fixed4(tex2Dlod(_LightTex, fixed4(i.uv.zw, 0.0, _LightPower)).rgb, 1.0);

				fixed3 texNormal = UnpackNormal(tex2D(_NormalTex, i.uv.xy));
				fixed3 N = normalize(mul(texNormal, float3x3(i.worldTangent, i.worldBinormal, i.worldNormal)));
				//fixed3 N = fixed3(dot(i.TtoW0, texNormal), dot(i.TtoW1, texNormal), dot(i.TtoW2, texNormal));
				
				//fixed3 N = normalize(i.worldNormal.xyz);
				fixed3 L = normalize(i.lightDir);
				fixed3 V = normalize(UnityWorldSpaceViewDir(i.worldPos.xyz));
				fixed3 H = normalize(L + V);

				float NdotH = saturate(dot(N, H));

				fixed3 specular = _LightColor.rgb * mainCol.rgb * pow(NdotH, _Gloss) * maskCol.r * _SpecularBright;

				fixed4 col = fixed4(mainCol.rgb + specular + evirCol, 1.0);
				return GetBrightnessColor(col);
			}

			ENDCG
		}
	}
}