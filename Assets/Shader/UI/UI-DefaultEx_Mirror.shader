//可选上下、左右、四角对称设置渐变
Shader "AL/UI/DefaultEx_Mirror"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _AlphaTex("Sprite Alpha Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[Enum(Horizontal, 0, Vertical, 1, Quarter, 2)] _Mirror("Mirror", Float) = 0
		_Range("Range", Range(0.0, 1.0)) = 0.0

		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOp]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			Name "Default"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			
			#pragma multi_compile __ ETC1_EXTERNAL_ALPHA

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				float2 uv  : TEXCOORD2;
				fixed gray : TEXCOORD3;
			};

			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				OUT.color = IN.color * _Color;

				OUT.uv = IN.uv1;
				OUT.gray = dot(IN.color, fixed4(1, 1, 1, 0));
				return OUT;
			}

			sampler2D _MainTex;	
			sampler2D _AlphaTex;
			half _Mirror;
			half _Range;

			fixed4 frag(v2f IN) : SV_Target
			{
			#ifdef ETC1_EXTERNAL_ALPHA
				fixed4 color = UnityGetUIDiffuseColor(IN.texcoord, _MainTex, _AlphaTex, _TextureSampleAdd);
			#else
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
			#endif

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				half2 uv = IN.uv;
				if(_Mirror > 1.5)
				{ //四角
					uv = abs(uv * 2.0 - 1.0);
					uv = 1.0 - saturate((uv - 1.0 + _Range) / _Range);
				}
				else if(_Mirror > 0.5)
				{ //上下
					uv.x = 1.0;
					uv.y = abs(uv.y * 2.0 - 1.0);
					uv.y = 1.0 - saturate((uv.y - 1.0 + _Range) / _Range);
				}
				else
				{ //左右
					uv.x = abs(uv.x * 2.0 - 1.0);
					uv.x = 1.0 - saturate((uv.x - 1.0 + _Range) / _Range);
					uv.y = 1.0;
				}
				
				color.a *= uv.x * uv.y;

				if (IN.gray == 0.0) 
				{
					half grey = dot(color.rgb, half3(0.299, 0.587, 0.114));
					color.rgb = half3(grey, grey, grey);
					color.a = color.a * IN.color.a;
				}
				else
				{
					color = color * IN.color;
				}

				return color;
			}
			ENDCG
		}
	}
}
