Shader "Unlit/FieldOfViewShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Color("Color",Color)=(1,1,1,1)//颜色
		_StrongFloat("_StrongFloat",float)=0.1//增强圆形边缘效果的值
		_AlphaDownFloat("_AlphaDownFloat",float)=0.2//降低锥形区域外的alpha
		_Angle("Angle",float)=25//25*2度角的锥形
		_GradientFloat("_GradientFloat",float)=0.3//渐变半圆弧的颜色
    }
    SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			fixed4 _Color;
			float _StrongFloat;
			float _AlphaDownFloat;
			float _Angle;
			float _GradientFloat;
			uniform float _FloatArray[256];

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

				fixed4 col;
				//圆弧
				float2 uv=i.uv;
				uv.x=uv.x-0.5;
				uv.y=uv.y-0.5;
				//i.uv=i.uv-float2(0.5,0.5);
				//根据UV来计算出一个变化区域，一个以UV中心点为中心，半径为0.5的圆形，圆形内到外从1渐变到0
				fixed dis=1.0-sqrt(uv.x*uv.x+uv.y*uv.y)*2;
				//中心点向周围发射的向量（归一化）
				fixed2 fragmentDir=normalize(uv.xy);
				//半圆弧，圆弧中心向两旁的值从1逐渐变为0，cos正好满足
				fixed rightHalfCircle=clamp(dot(float2(1,0),fragmentDir.xy),0,1);
				//渐变半圆弧颜色（内到外）
				col=lerp(_Color,fixed4(1,1,1,1),dis*_GradientFloat);
				//衰减半圆弧两旁，衰减_Angle角后的
				fixed tempAngleCos=cos(radians(_Angle));
				//增强边缘效果
				fixed strongF=pow(dis,_StrongFloat);
				col.a=col.a*dis*rightHalfCircle*strongF;
				//大于_Angle角度区域的像素衰减（在视野之外的），_Angle是视野角度的一半
				if(rightHalfCircle<tempAngleCos){
					col.a*=_AlphaDownFloat;
				} else {
					//扫描遮挡的核心：视野角度内（-_Angle,_Angle）范围内进行一个遮挡处理
					//计算出index
					//fragmentDir.y是归一化后的向量y值
					//因为sqrt(fragmentDir.y*fragmentDir.y+fragmentDir.x*fragmentDir.x)=1
					//sin(fragmentDir)与UV正x轴（0.5,0.5）的角度弧度为fragmentDir.y/1,即fragmentDir.y,
					//反过来说fragmentDir.y就是sin（角度）
					//输入的是正弦值 sin(角度)=对边/斜边=fragmentDir.y/1  斜边是1，因为fragmentDir是归一化向量
					//反正弦函数 输入[-1,1](sin值) 输出[-π/2, π/2]（弧度）
					//简单来讲就是将偏移后的uv坐标点与中心点向量 和 正X轴的夹角角度转成了弧度..
					//知道什么是反正弦函数就很容易了。。 就是反着来，正弦函数是输入弧度 输出正弦值，反正弦就是输入正弦值输出弧度
					float curRad=asin(fragmentDir.y);
					curRad+=radians(_Angle);//偏移到正数(上面的弧度是指（-_Angle,_Angle）角度的当前片元所在的角度弧度)
					float f=curRad/radians(_Angle*2);//当前弧度/总弧度  得到一个系数
					float index=f*256;		//系数乘上索引最大值 获取索引
					//因为c#计算出的当index为0时，应该是照射区域上方，而此时Shader是不是0时上方，
					//答案不是，上方index为0时，curRad是0，在没有经过偏移时，它位于照射区域最下。所以应该取反索引
					index=256-index;
					float curFloat=_FloatArray[index];
					//dis是1到0，(1-dis)就是0到1，curFloat是锥形尖角的位置到目标障碍物的距离，*5要根据实际情况考虑
					if(curFloat>0&&(1-dis)*5>curFloat){
						col*=0;
					}
				}
				return col;
            }
            ENDCG
        }
    }
}

