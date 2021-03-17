Shader "SimplestInstancedShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 1)
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing // 开启多实例的变量编译
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID //顶点着色器的 InstancingID定义
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID //片元着色器的 InstancingID定义
            };

            UNITY_INSTANCING_BUFFER_START(Props) // 定义多实例变量数组

                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)

            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //装配 InstancingID
                UNITY_TRANSFER_INSTANCE_ID(v, o); //输入到结构中传给片元着色器

                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i); //装配 InstancingID
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Color); //提取多实例中的当前实例的Color属性变量值
            }
            ENDCG
        }
    }
}