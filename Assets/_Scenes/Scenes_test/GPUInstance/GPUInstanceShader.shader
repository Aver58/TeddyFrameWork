Shader "GPUInstanceShader"{
    Properties{
        _Color("Color", Color) = (1, 1, 1, 1)
    }

    SubShader{
        Tags { "RenderType" = "Opaque" }

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing                        // 开启多实例的变量编译
            #include "UnityCG.cginc"

            // TEXTURE2D_FLOAT(_tex_ani);
            int _tex_ani_side_length;
            // 采样器要用clamp方式  “Point”, “Linear” or “Trilinear” (required) set up texture filtering mode.
            // “Clamp”, “Repeat”, “Mirror” or “MirrorOnce” (required) set up texture wrap mode.Wrap modes can be specified per-axis (UVW), e.g. “ClampU_RepeatV”.
            //SAMPLER(sampler_PointClamp);

            // 计算骨骼矩阵
		    //float4x4 cal_bone_mtx(const uint bone_idx){
		    //    // 拿到 C# 层传入的骨骼矩阵索引 
            //    uint idx = UNITY_ACCESS_INSTANCED_PROP(Props, _matrix_index) + bone_idx * 4;
            //    
            //    uint x = idx_1dx % _tex_ani_side_length;
			//	uint y = idx_1dx / _tex_ani_side_length;
//
			//	float2 ani_xy = float2(0, 0);
			//	ani_xy.x = (float)x / _tex_ani_side_length;
			//	ani_xy.y = (float)y / _tex_ani_side_length;
//
			//	float4 mat1 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(0, 0), 0);
			//	float4 mat2 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(1.0 / _tex_ani_side_length, 0), 0);
			//	float4 mat3 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(2.0 / _tex_ani_side_length, 0), 0);
			//	float4 mat4 = SAMPLE_TEXTURE2D_LOD(_tex_ani, sampler_PointClamp, ani_xy + float2(3.0 / _tex_ani_side_length, 0), 0);
			//	
            //    float4x4 mtx_x = float4x4(
			//		mat1,
			//		mat2,
			//		mat3,
			//		float4(0, 0, 0, 1));
			//		
            //    return mtx_x;
		    //}
		    
            struct appdata{
                float4 vertex : POSITION;
                uint4 vBones : BLENDINDICES;
				float4 vWeights : BLENDWEIGHTS;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID                      //顶点着色器的 InstancingID 定义
            };

            struct v2f{
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID                      //片元着色器的 InstancingID 定义
            };

            UNITY_INSTANCING_BUFFER_START(Props)                    // 定义多实例变量数组
            
			    UNITY_DEFINE_INSTANCED_PROP(float, _matrix_index)   // 矩阵索引
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)         // 颜色

            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v){
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);                         //装配 InstancingID
                UNITY_TRANSFER_INSTANCE_ID(v, o);                   //输入到结构中传给片元着色器

                // 拿到骨骼变换矩阵
				float3 obj_vertex = v.vertex.xyz;
				//float4x4 mtx_1 = cal_bone_mtx(v.vBones.x);
				//float4x4 mtx_2 = cal_bone_mtx(v.vBones.y);
				//// 根据骨骼权重，算出蒙皮顶点位置
				//float4x4 mtx = mtx_1 * v.vWeights.x + mtx_2 * v.vWeights.y;
				
				//obj_vertex = mul(mtx, float4(v.vertex.xyz, 1.0f)).xyz;
                o.vertex = UnityObjectToClipPos(obj_vertex);          // 顶点的世界坐标转屏幕裁剪坐标
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target{
                UNITY_SETUP_INSTANCE_ID(i);                         //装配 InstancingID
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color);  //提取多实例中的当前实例的Color属性变量值
            }
            
            ENDCG
        }
    }
}