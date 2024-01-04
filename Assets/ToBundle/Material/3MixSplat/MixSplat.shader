Shader "Custom/Test Shader" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		[NoScaleOffset]_TextureR("_TextureR", 2D) = "white" {}
		[NoScaleOffset]_TextureG("_TextureG", 2D) = "white" {}
		[NoScaleOffset]_TextureB("_TextureB", 2D) = "white" {}
		[NoScaleOffset]_TextureA("_TextureA", 2D) = "white" {}
	}
	SubShader {
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _TextureR, _TextureG, _TextureB, _TextureA;
			float4 _MainTex_ST;

			v2f vert(appdata In)
			{
				v2f Out;
				Out.vertex = UnityObjectToClipPos(In.vertex);
				Out.uv = TRANSFORM_TEX(In.uv, _MainTex);
				Out.uv1 = In.uv;
				return Out;
			}

			fixed4 frag(v2f In) : SV_Target
			{
				float4 splat = tex2D(_MainTex,In.uv1);
				float4 color = tex2D(_MainTex,In.uv);
				return	tex2D(_TextureR, In.uv) * splat.r +
						tex2D(_TextureG, In.uv) * splat.g +
						tex2D(_TextureB, In.uv) * splat.b + 
						tex2D(_TextureA, In.uv) * (1- splat.r - splat.g - splat.b);
				//return float4(In.uv,1,1);
				//return In.vertex;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
