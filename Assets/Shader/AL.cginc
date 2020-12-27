half _GlobalBrightness = 1;		//默认为1
half _SingleBrightness = 0;		//默认为0，与_GlobalBrightness相加为1时为标准亮度

half4 GetBrightnessColor(half4 col)
{
	col.rgb *= _GlobalBrightness + _SingleBrightness;
	return col;
}

half3 RGBtoHSV(half3 c)
{
	half4 K = half4(0, -0.33, 2 * 0.33, -1);
	half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
	half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));
	half  d = q.x - min(q.w, q.y);

	half reciprocal = 1 / q.x;
	half h = abs(q.z + (q.w - q.y) * 0.17);
	half s = d * reciprocal;
	half v = q.x;
	return half3(h, s, v);
}

half3 HSVtoRGB(half3 c)
{
	half4 K = half4(1, 2 * 0.33, 0.33, 3);
	half3 p = abs(frac(c.xxx + K.xyz) * 6 - K.www);
	return saturate(c.z * lerp(K.xxx, saturate(p - K.xxx), c.y));
}