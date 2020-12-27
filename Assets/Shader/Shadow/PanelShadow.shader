Shader "AL/Shadow/PlaneShadow"
{
    Properties
    {
        _ShadowCol("Color", color) = (1,1,1,1)
        _StencilID("StencilID", float) = 2
        _Plane("Plane", vector) = (0,1,0,0)
        _LightDir("LightDir", vector) = (1,-1,1,1)
    }
    SubShader
    {
        Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
        
        Pass
        {
            Stencil{
                Ref [_StencilID]
                Comp NotEqual
                Pass replace
            }
            zwrite off
            blend srcalpha oneminussrcalpha
            offset -1,-1

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
 
            struct appdata
            {
                float4 vertex : POSITION;
            };
 
            struct v2f
            {
                UNITY_FOG_COORDS(0)
                float4 vertex : SV_POSITION;
                float distance : TEXCOORD0;
            };
 
            float4 _ShadowCol;
            float4 _Plane;
            float4 _LightDir;
             
            v2f vert (appdata v)
            {
                v2f o;
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 panelNor = normalize(_Plane.xyz);
                float3 lightDir = normalize(_LightDir.xyz);
                o.distance = (_Plane.w - dot(worldPos.xyz, panelNor)) / dot(lightDir, panelNor);
                worldPos.xyz = worldPos.xyz + o.distance * lightDir;
                o.vertex = mul(unity_MatrixVP, worldPos);
 
                return o;
            }
             
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _ShadowCol * step(0, i.distance);
                return col;
            }
            ENDCG
        }
    }
}