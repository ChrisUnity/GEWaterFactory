// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Effect/Ocean"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	//噪音图
	_NoiseTex("Texture", 2D) = "white" {}
	//叠加颜色
	_Color("Color",Color) = (1,1,1,1)
		//亮度
		_Light("Light", Range(0, 10)) = 2
		//扭曲强度
		_Intensity("intensity", float) = 0.1
		//偏移速度
		_XSpeed("XSpeed", float) = 0.1
		_YSpeed("YSpeed", float) = 0.1
	}
		SubShader
	{
		Tags{ "RenderType" = "Transparent" }
		LOD 100

		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog
 #pragma multi_compile_instancing

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		UNITY_FOG_COORDS(1)
		float4 vertex : SV_POSITION;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	sampler2D _NoiseTex;
	float4 _Color;
	float _Light;
	float _Intensity;
	float _XSpeed;
	float _YSpeed;

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_INSTANCE_ID(i);
	//根据时间和偏移速度获取噪音图的颜色作为uv偏移
	fixed4 noise_col = tex2D(_NoiseTex, i.uv + fixed2(_Time.y*_XSpeed, _Time.y*_YSpeed));
	//计算uv偏移的颜色和亮度和附加颜色计算
	fixed4 col = tex2D(_MainTex, i.uv + _Intensity * noise_col.rg)*_Light*_Color;
	UNITY_APPLY_FOG(i.fogCoord, col);
	col.a = _Color.a;
	return col;
}
	ENDCG
}
	}
}
