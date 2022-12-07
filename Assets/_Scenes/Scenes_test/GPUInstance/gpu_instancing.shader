Shader "lsc/gpu_instancing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            //启用gpu instancing
            #pragma multi_compile_instancing 

            //开启主灯光的阴影
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            //自定义数组,保存每个实例的颜色
            StructuredBuffer<float4> _instancing_color;
            StructuredBuffer<int> _ani_matrix_index;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                uint4 vBones : BLENDINDICES;
                float4 vWeights : BLENDWEIGHTS;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;

                float3 positionWS               : TEXCOORD2;
                float4 positionCS               : SV_POSITION;

                uint4 vBones : BLENDINDICES;
                float4 vWeights : BLENDWEIGHTS;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            TEXTURE2D_FLOAT(_tex_ani);
            SAMPLER(sampler_PointClamp);

            int _tex_ani_side_length;
            float _tex_ani_unit_size;


            float4x4 cal_bone_mtx(const uint bone_idx)
            {
#ifdef UNITY_INSTANCING_ENABLED
                uint idx_1dx = _ani_matrix_index[unity_InstanceID] + bone_idx * 4;
#else
                uint idx_1dx = bone_idx * 4;
#endif

                uint x = idx_1dx % _tex_ani_side_length;
                uint y = idx_1dx / _tex_ani_side_length;

                float2 ani_xy = float2(0, 0);
                ani_xy.x = (float)x / _tex_ani_side_length;
                ani_xy.y = (float)y / _tex_ani_side_length;
                float4 mat1 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(0, 0), 0);
                float4 mat2 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(1.0 / _tex_ani_side_length, 0), 0);
                float4 mat3 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(2.0 / _tex_ani_side_length, 0), 0);
                float4 mat4 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(3.0 / _tex_ani_side_length, 0), 0);


                float4x4 mtx_x = float4x4(
                    mat1,
                    mat2,
                    mat3,
                    float4(0, 0, 0, 1)
                    );

                return mtx_x;
            }

            v2f vert (appdata v)
            {
                v2f o;
                //给unity_InstanceID赋值,使urp的内部函数会自动调用unity_Builtins0数组中的属性
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                float3 obj_vertex = v.vertex.xyz;

                float4x4 mtx_1 = cal_bone_mtx(v.vBones.x);
                float4x4 mtx_2 = cal_bone_mtx(v.vBones.y);
                float4x4 mtx_3 = cal_bone_mtx(v.vBones.z);
                float4x4 mtx_4 = cal_bone_mtx(v.vBones.w);

                float4x4 mtx =
                    mtx_1 * v.vWeights.x +
                    mtx_2 * v.vWeights.y +
                    mtx_3 * v.vWeights.z +
                    mtx_4 * v.vWeights.w;

                obj_vertex = mul(mtx_1, float4(v.vertex.xyz, 1.0f)).xyz;

                //内部会自动取实例的变幻矩阵,在不同的位置画出实例
                //VertexPositionInputs vertexInput = GetVertexPositionInputs(obj_vertex);
                //o.positionWS = vertexInput.positionWS;
                //o.positionCS = vertexInput.positionCS;

                o.positionWS = mul(UNITY_MATRIX_M,  float4(obj_vertex, 1.0)).xyz;
                o.positionCS = mul(UNITY_MATRIX_VP, float4(o.positionWS, 1.0));
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.vBones = v.vBones;
                o.vWeights = v.vWeights;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                half4 col = tex2D(_MainTex, i.uv);
                //计算主光与阴影
//                float4 shadow_coord = TransformWorldToShadowCoord(i.positionWS);
//                Light mainLight = GetMainLight(shadow_coord);
//                half3 light_col = mainLight.color * mainLight.shadowAttenuation;
//
//
//#ifdef UNITY_INSTANCING_ENABLED
//                col.rgb = light_col * col.rgb * _instancing_color[unity_InstanceID];
//#else
//                col.rgb = light_col * col.rgb;
//#endif

                return col;
            }
            ENDHLSL
        }

        Pass //产生阴景 todo 根据ShadowCasterPass.hlsl修改成gpu skin,从而能产生正确的阴影
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
		}

        Pass //写入深度 todo 修改成gpu skin
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma exclude_renderers gles gles3 glcore
            #pragma target 4.5

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }
    }
}
