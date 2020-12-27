//固定上下左右四边渐变
Shader "AL/UI/DefaultEx_Frame"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		[PerRendererData] _AlphaTex("Sprite Alpha Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_Height("Height", Range(0.0, 1.0)) = 0.0
		_AspectRatio("AspectRatio", Range(0.0, 2.0)) = 0.0

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
				float width : TEXCOORD3;
				fixed gray : TEXCOORD4;
			};

			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			half _Height;
			half _AspectRatio;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				OUT.color = IN.color * _Color;

				OUT.width = _Height * _AspectRatio;
				OUT.uv = IN.uv1;
				OUT.gray = dot(IN.color, fixed4(1, 1, 1, 0));
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			
			fixed4 frag(v2f IN) : SV_Target
			{
			#ifdef ETC1_EXTERNAL_ALPHA
				fixed4 color = UnityGetUIDiffuseColor(IN.texcoord, _MainTex, _AlphaTex, _TextureSampleAdd);
			#else
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
			#endif

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);

				half2 uv = IN.uv;

				uv = abs(uv * 2.0 - 1.0);
				uv.x = 1.0 - saturate((uv.x - 1.0 + IN.width) / IN.width);
				uv.y = 1.0 - saturate((uv.y - 1.0 + _Height) / _Height);
								
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
