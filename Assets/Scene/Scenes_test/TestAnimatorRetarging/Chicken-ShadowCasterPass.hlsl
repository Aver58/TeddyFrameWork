#ifndef CHICKEN_SHADOW_CASTER_PASS_INCLUDED
#define CHICKEN_SHADOW_CASTER_PASS_INCLUDED

#include "Chicken-Common.hlsl"

#ifdef _USE_ALPHA_TEST
    TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
#endif

CBUFFER_START(UnityPerMaterial)
#ifdef _USE_ALPHA_TEST
    float _Cutoff;
#endif
CBUFFER_END

float3 _LightDirection;

struct Attributes {
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 texcoord     : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings {
    #ifdef _USE_ALPHA_TEST
    float2 uv       : TEXCOORD0;
    #endif
    float4 positionCS   : SV_POSITION;
};

float4 GetShadowPositionHClip(Attributes input) {
    float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
    float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

    float invNdotL = 1.0 - saturate(dot(_LightDirection, normalWS));
    float scale = invNdotL * _ShadowBias.y;

    // normal bias is negative since we want to apply an inset normal offset
    positionWS = _LightDirection * _ShadowBias.xxx + positionWS;
    positionWS = normalWS * scale.xxx + positionWS;
    float4 positionCS = TransformWorldToHClip(positionWS);

    #if UNITY_REVERSED_Z
    positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #else
    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
    #endif

    return positionCS;
}

Varyings ShadowCasterVertex(Attributes input) {
    Varyings output;
    UNITY_SETUP_INSTANCE_ID(input);
    #ifdef _USE_ALPHA_TEST
    output.uv = input.texcoord;
    #endif
    output.positionCS = GetShadowPositionHClip(input);
    return output;
}

half4 ShadowCasterFragment(Varyings input) : SV_TARGET {
    #ifdef _USE_ALPHA_TEST
    half alpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).a;
    clip(alpha - _Cutoff);
    #endif
    #ifdef _NOT_CAST_SHADOW
    discard;
    #endif
    return 0;
}

#endif
