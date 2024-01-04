Shader "Chicken/MatCap/Role" {
	Properties {
        [Header(Maps)]
		[NoScaleOffset]_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset]_MaskTex("Mask R: Glossiness, G: Specular, B: MatCap", 2D) = "black" {}
		[NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
		[NoScaleOffset]_EmissionTex("Custom R: Emission Strength, G: Color ID", 2D) = "black" {}
		[NoScaleOffset]_MatCap ("MatCap (RGB)", 2D) = "white" {}
		[Space(6)]
        [Header(Colors)]
		_RimColor("Rim Color", Color) = (0.8, 0.8, 0.8, 0.0)
		_LocalSpecularColor("Specular Color", COLOR) = (1, 1, 1, 1)
		[Space(6)]
        [Header(Values)]
		_Alpha("Alpha", Range(0.0, 1.0)) = 1.0
		_RimPower ("Rim Power", Range(1.0, 10.0)) = 3.0
		_RimScale ("Rim Scale", Range(0,10)) = 2.0
		_FlareSpeed("Rim FlareSpeed",Range(0,10)) = 0.0
		_AttenBias("AttenBias", float) = 0
		[Space(6)]
        [Header(Toggles)]
		[Toggle(_USE_RIM_LIGHT)] _USE_RIM_LIGHT("Use RimColor", float) = 0
		[Toggle(_USE_RIM_INSIDE)] _USE_RIM_INSIDE("Use RimColor Inside", float) = 0
		[Space(6)]
        [Header(Enum)]
		[Enum(Off, 0, On, 1)] _ZWrite("ZWrite", Float) = 0
	}

	SubShader {
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite[_ZWrite]
		ZTest LEqual
		Cull Back
		LOD 200

		Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			// -------------------------------------
            // Unity defined keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			// -------------------------------------
            // keywords
			#pragma multi_compile _ _USE_RIM_LIGHT
			#pragma multi_compile _ _USE_RIM_INSIDE
			#pragma multi_compile _ _USE_SH
			
			#pragma vertex SimpleLitVertex
			#pragma fragment SimpleLitFragment

			#define _USE_MAINTEX 1
			#define _USE_NORMALMAP 1
			#define _USE_MATCAP 1
			#define _USE_EMISSION 1
			//#define _USE_ID_COLOR_TEX 1
			#define _USE_ATTENBIAS 1
			
			#define _USE_SPECULAR_CUSTOM 1
			#define _USE_CUSTOMCOLOR 1
			#define _USE_CUSTOMFRESNEL 1
			
			#define _USE_ALPHA 1

			#define _HAVE_RIM 1
			
			#include "Chicken-SimpleLitPass.hlsl"

			ENDHLSL
		}

        Pass {
            Name "ShadowCaster"
            Tags{ "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex ShadowCasterVertex
            #pragma fragment ShadowCasterFragment

            #include "Chicken-ShadowCasterPass.hlsl"

            ENDHLSL
        }
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite[_ZWrite]
		ZTest LEqual
		Cull Back
		LOD 100

		Pass {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0
			// -------------------------------------
            // keywords
			#pragma multi_compile _ _USE_RIM_LIGHT
			
			#pragma vertex SimpleLitVertex
			#pragma fragment SimpleLitFragment

			#define _USE_MAINTEX 1
			#define _USE_SPECULAR_CUSTOM 1
			#define _USE_CUSTOMCOLOR 1
			#define _USE_CUSTOMFRESNEL 1

			#define _USE_MATCAP 1
            
            #define _USE_ALPHA 1

			#define _HAVE_RIM 1

			#include "Chicken-SimpleLitLodPass.hlsl"

			ENDHLSL
		}
	}
}
