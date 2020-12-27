Shader "Unlit/Light Shader"
{
	Properties
	{
		_MainTex ("Albedo", 2D) = "white" {}
		_Tint("Tint",Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { 
		"LightMode" = "ForwardBase"
		"RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityStandardBRDF.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Tint;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex);
				return o;
			}
			
			float4 frag(v2f i) : SV_Target
			{
				return float4(1,1,1,1);
				fixed4 col = tex2D(_MainTex, i.uv);
				i.normal = normalize(i.normal);
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 lightColor = _LightColor0.rgb;
				float3 viewDir = _WorldSpaceCameraPos - i.worldPos;
				float3 albedo = tex2D(_MainTex, i.uv).rgb * _Tint.rgb;
				float3 diffuse = albedo * lightColor * DotClamped(lightDir, i.normal);
				return float4(viewDir, 1);
				float3 reflectionDir = reflect(-lightDir,i.normal);
				return float4(reflectionDir,1);
			}
			ENDCG
		}
	}
}
