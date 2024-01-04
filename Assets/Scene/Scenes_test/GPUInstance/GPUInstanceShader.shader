Shader "GPUInstanceShader"{
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
    }

    SubShader{
        Tags { "RenderType" = "Opaque" }

        Pass{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // 开启多实例的变量编译
            #pragma multi_compile_instancing                        
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			
			UNITY_INSTANCING_BUFFER_START(Props)                        // 定义多实例变量数组
			    UNITY_DEFINE_INSTANCED_PROP(float, _ani_matrix_index)   // 矩阵索引
            UNITY_INSTANCING_BUFFER_END(Props)
            
            int _tex_ani_side_length;
            // 采样器要用clamp方式  “Point”, “Linear” or “Trilinear” (required) set up texture filtering mode.
            // “Clamp”, “Repeat”, “Mirror” or “MirrorOnce” (required) set up texture wrap mode.Wrap modes can be specified per-axis (UVW), e.g. “ClampU_RepeatV”.
            SAMPLER(sampler_PointClamp);
            TEXTURE2D_FLOAT(_tex_ani);
            sampler2D _MainTex;
            float4 _MainTex_ST;
            StructuredBuffer<float4> _instancing_color;

            // 贴图纹理：每帧、每个骨骼矩阵，一个矩阵4个像素
            // 计算骨骼矩阵
		    float4x4 cal_bone_mtx(const uint bone_idx){
		        // 拿到 C# 层传入的骨骼矩阵索引 
                uint idx_1dx = (int)UNITY_ACCESS_INSTANCED_PROP(Props, _ani_matrix_index) + bone_idx * 4;
                
                uint x = idx_1dx % _tex_ani_side_length;
				uint y = idx_1dx / _tex_ani_side_length;

				float2 ani_xy = float2(0, 0);
				// 为啥这里要再除一次呀？UV 以(0,1) 表示？
				ani_xy.x = (float)x / _tex_ani_side_length;
				ani_xy.y = (float)y / _tex_ani_side_length;

                // 采样4个像素
				float4 mat1 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(0, 0), 0);
				float4 mat2 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(1.0 / _tex_ani_side_length, 0), 0);
				float4 mat3 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(2.0 / _tex_ani_side_length, 0), 0);
				float4 mat4 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(3.0 / _tex_ani_side_length, 0), 0);
				
                float4x4 mtx_x = float4x4(
					mat1,
					mat2,
					mat3,
					float4(0, 0, 0, 1));
					
                return mtx_x;
		    }
		    
            struct appdata{
                float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                uint4 vBones : BLENDINDICES;
				float4 vWeights : BLENDWEIGHTS;
                UNITY_VERTEX_INPUT_INSTANCE_ID                      //顶点着色器的 InstancingID 定义
            };

            struct v2f{
                float2 uv : TEXCOORD0;
                float3 positionWS               : TEXCOORD2;
                float4 positionCS               : SV_POSITION;
                uint4 vBones : BLENDINDICES;
                float4 vWeights : BLENDWEIGHTS;
                UNITY_VERTEX_INPUT_INSTANCE_ID                      //片元着色器的 InstancingID 定义
            };

            v2f vert(appdata v){
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);                         //装配 InstancingID
                UNITY_TRANSFER_INSTANCE_ID(v, o);                   //输入到结构中传给片元着色器

				float3 obj_vertex = v.vertex.xyz;
				// 拿到骨骼变换矩阵
                float4x4 mtx_1 = cal_bone_mtx(v.vBones.x);
                float4x4 mtx_2 = cal_bone_mtx(v.vBones.y);
                float4x4 mtx_3 = cal_bone_mtx(v.vBones.z);
                float4x4 mtx_4 = cal_bone_mtx(v.vBones.w);

                float4x4 mtx =
                    mtx_1 * v.vWeights.x +
                    mtx_2 * v.vWeights.y +
                    mtx_3 * v.vWeights.z +
                    mtx_4 * v.vWeights.w;
				
				obj_vertex = mul(mtx, float4(v.vertex.xyz, 1.0f)).xyz;
                
                o.positionWS = mul(UNITY_MATRIX_M,  float4(obj_vertex, 1.0)).xyz;
                o.positionCS = mul(UNITY_MATRIX_VP, float4(o.positionWS, 1.0));// 顶点的世界坐标转屏幕裁剪坐标
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.vBones = v.vBones;
                o.vWeights = v.vWeights;
                return o;
            }

            half4 frag(v2f i) : SV_Target{
                UNITY_SETUP_INSTANCE_ID(i);                         //装配 InstancingID
                //return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);  //提取多实例中的当前实例的Color属性变量值
                half4 col = tex2D(_MainTex, i.uv);
                #ifdef UNITY_INSTANCING_ENABLED
                    col *= _instancing_color[unity_InstanceID];
                #endif
                return col;
            }
            
            ENDHLSL
        }
    }
}