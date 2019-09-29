// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Alpha Splited Colored"
{
	Properties
	{
		_MainTex("RGB", 2D) = "black" {}
		_AlphaTex("Alpha", 2D) = "black" {}
		_Diffuse("Diffuse", Color) = (1,1,1,1)           // 漫反射颜色
	}

		SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			//Offset - 1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag            

			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed4 _Diffuse;
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f IN) : COLOR
			{
				// [dev]
				fixed4 col;
				col.rgb = tex2D(_MainTex, IN.texcoord).rgb;
				col.a = tex2D(_AlphaTex, IN.texcoord).r;
				return col;
			}
			ENDCG
		}
	}
}
