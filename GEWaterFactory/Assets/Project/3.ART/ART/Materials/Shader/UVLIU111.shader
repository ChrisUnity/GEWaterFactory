Shader "CookBookShaders/Scroll UV" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TransVal("Transparency Value", Range(0,1)) = 0.5
		_Color("Diffuse Tint", Color) = (1, 1, 1, 1)
		_ScrollYSpeed("YSpeed", Range(0, 100)) = 2
	    _ScrollYPos("XPos", Range(0, 1)) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "Queue" = "Transparent-1" }
			LOD 200
			ZWrite Off
			Cull Off
			//开启透明度混合
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			//#pragma surface surf Lambert
			#pragma surface surf Lambert alpha
			sampler2D _MainTex;
			float _ScrollXSpeed;
			float _ScrollYSpeed;
			fixed4 _Color;
			float _TransVal;
			float _ScrollYPos;
			struct Input {
				float2 uv_MainTex;
			};

			void surf(Input IN, inout SurfaceOutput o)
			{
				//根据时间增加，修改 uv_MainTex ，例如从 (0,0) 到 (1,1) ，这样就类似于贴图在反方向运动。
				fixed2 scrolledUV = IN.uv_MainTex;

				fixed xScrollOffset = _ScrollYSpeed * _Time;
				//注意_Time这个内置变量是float4 值为 Time (t/20, t, t*2, t*3), 
				//就是说Unity给我们几个内定的时间速度来使用，默认是取X值。
				fixed yScrollOffset = _ScrollYPos;

				scrolledUV += fixed2(xScrollOffset,yScrollOffset);

				half4 c = tex2D(_MainTex, scrolledUV)*_Color;
				o.Albedo = c.rgb*5 ;
				o.Alpha = c.b*_TransVal;
			}
			ENDCG
	}
		FallBack "Diffuse"
}