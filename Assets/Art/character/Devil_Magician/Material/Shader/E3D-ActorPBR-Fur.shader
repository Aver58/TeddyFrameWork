// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "E3D/Actor/ActorPBR-Fur"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Alpha("Alpha", Range( 0 , 2)) = 2
		_BaseColor("BaseColor", Color) = (0,0,0,0)
		_Metallic("Metallic", Range( 0 , 1)) = 1
		_Smooth("Smooth", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" }
		Cull Back
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#pragma target 3.0
		#pragma exclude_renderers xbox360 xboxone ps4 psp2 n3ds wiiu 
		#pragma surface surf StandardCustomLighting keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Alpha;
		uniform float4 _BaseColor;
		uniform float _Metallic;
		uniform float _Smooth;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode7 = tex2D( _MainTex, uv_MainTex );
			SurfaceOutputStandard s49 = (SurfaceOutputStandard ) 0;
			s49.Albedo = _BaseColor.rgb;
			float3 ase_worldNormal = i.worldNormal;
			s49.Normal = ase_worldNormal;
			s49.Emission = float3( 0,0,0 );
			s49.Metallic = _Metallic;
			s49.Smoothness = _Smooth;
			s49.Occlusion = 1.0;

			data.light = gi.light;

			UnityGI gi49 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g49 = UnityGlossyEnvironmentSetup( s49.Smoothness, data.worldViewDir, s49.Normal, float3(0,0,0));
			gi49 = UnityGlobalIllumination( data, s49.Occlusion, s49.Normal, g49 );
			#endif

			float3 surfResult49 = LightingStandard ( s49, viewDir, gi49 ).rgb;
			surfResult49 += s49.Emission;

			#ifdef UNITY_PASS_FORWARDADD//49
			surfResult49 -= s49.Emission;
			#endif//49
			c.rgb = surfResult49;
			c.a = saturate( ( tex2DNode7 * tex2DNode7.a * _Alpha ) ).r;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16400
7;332;1515;701;-1230.049;795.8116;1;True;False
Node;AmplifyShaderEditor.SamplerNode;7;1833.286,-386.8098;Float;True;Property;_MainTex;MainTex;1;0;Create;True;0;0;False;0;None;1d0159b0b8d67d24eb570edb70743176;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;48;1842.026,-183.4758;Float;False;Property;_Alpha;Alpha;2;0;Create;True;0;0;False;0;2;2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;1919.076,-588.1649;Float;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;1918.939,-670.4967;Float;False;Property;_Smooth;Smooth;5;0;Create;True;0;0;False;0;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;45;1927.12,-857.668;Float;False;Property;_BaseColor;BaseColor;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;46;2155.845,-310.1318;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.CustomStandardSurface;49;2297.151,-841.8685;Float;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;47;2340.412,-311.5717;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2655.406,-691.8234;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;E3D/Actor/ActorPBR-Fur;False;False;False;False;True;True;True;True;True;False;True;True;False;False;False;False;False;False;False;False;False;Back;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Transparent;All;True;True;True;True;True;True;True;False;False;False;False;False;False;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;46;0;7;0
WireConnection;46;1;7;4
WireConnection;46;2;48;0
WireConnection;49;0;45;0
WireConnection;49;3;51;0
WireConnection;49;4;50;0
WireConnection;47;0;46;0
WireConnection;0;9;47;0
WireConnection;0;13;49;0
ASEEND*/
//CHKSM=EF1CC4AD35DA7F7560FFF871E377D8C0E32E1B5C