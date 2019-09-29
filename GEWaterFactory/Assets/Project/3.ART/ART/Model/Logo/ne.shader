// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:34487,y:32200,varname:node_9361,prsc:2|diff-3265-OUT,spec-8753-OUT,gloss-2764-OUT,normal-8084-RGB,emission-8992-OUT,clip-5602-OUT;n:type:ShaderForge.SFN_ObjectPosition,id:6903,x:31350,y:32574,varname:node_6903,prsc:2;n:type:ShaderForge.SFN_FragmentPosition,id:6858,x:31350,y:32458,varname:node_6858,prsc:2;n:type:ShaderForge.SFN_Subtract,id:8249,x:31659,y:32515,varname:node_8249,prsc:2|A-6858-XYZ,B-6903-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:6908,x:32129,y:32516,varname:node_6908,prsc:2,cc1:1,cc2:-1,cc3:-1,cc4:-1|IN-8249-OUT;n:type:ShaderForge.SFN_OneMinus,id:973,x:32286,y:32516,varname:node_973,prsc:2|IN-6908-OUT;n:type:ShaderForge.SFN_Add,id:5794,x:32526,y:32603,varname:node_5794,prsc:2|A-973-OUT,B-8214-OUT;n:type:ShaderForge.SFN_Slider,id:8214,x:31972,y:32691,ptovrint:False,ptlb:Slider,ptin:_Slider,varname:node_8214,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:-2,cur:2,max:2;n:type:ShaderForge.SFN_Clamp01,id:5773,x:32744,y:32478,varname:node_5773,prsc:2|IN-5794-OUT;n:type:ShaderForge.SFN_Color,id:3685,x:32488,y:32317,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_3685,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Tex2d,id:1683,x:32488,y:32122,ptovrint:False,ptlb:Tex,ptin:_Tex,varname:node_1683,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Cubemap,id:6843,x:32488,y:31945,ptovrint:False,ptlb:Cubemap,ptin:_Cubemap,varname:node_6843,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,pvfc:0;n:type:ShaderForge.SFN_Tex2d,id:8084,x:34030,y:32287,ptovrint:False,ptlb:Normal,ptin:_Normal,varname:node_8084,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:2324,x:32704,y:32122,varname:node_2324,prsc:2|A-1683-RGB,B-3685-RGB;n:type:ShaderForge.SFN_Add,id:6918,x:32931,y:32122,varname:node_6918,prsc:2|A-2324-OUT,B-3597-OUT,C-8995-OUT;n:type:ShaderForge.SFN_Tex2d,id:3519,x:33534,y:33190,ptovrint:False,ptlb:Tex_Emission,ptin:_Tex_Emission,varname:node_3519,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:8447,x:33690,y:31836,ptovrint:False,ptlb:Specular,ptin:_Specular,varname:node_8447,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:10;n:type:ShaderForge.SFN_Slider,id:6543,x:33594,y:32008,ptovrint:False,ptlb:Glow,ptin:_Glow,varname:_node_8447_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.25,max:1;n:type:ShaderForge.SFN_If,id:5111,x:32917,y:32531,varname:node_5111,prsc:2|A-5794-OUT,B-8608-OUT,GT-2289-OUT,EQ-4536-OUT,LT-4536-OUT;n:type:ShaderForge.SFN_Vector1,id:2289,x:32683,y:32852,varname:node_2289,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:8608,x:32526,y:32779,ptovrint:False,ptlb:RimThickness,ptin:_RimThickness,varname:node_8608,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:4.061508,max:5;n:type:ShaderForge.SFN_Vector1,id:4536,x:32683,y:32918,varname:node_4536,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:7713,x:33232,y:32468,varname:node_7713,prsc:2|A-5773-OUT,B-5111-OUT,C-7348-OUT;n:type:ShaderForge.SFN_Add,id:5602,x:33682,y:32522,varname:node_5602,prsc:2|A-5340-OUT,B-9323-OUT,C-5773-OUT;n:type:ShaderForge.SFN_Add,id:8992,x:33745,y:32713,varname:node_8992,prsc:2|A-9362-OUT,B-8136-OUT;n:type:ShaderForge.SFN_Multiply,id:9362,x:33355,y:32740,varname:node_9362,prsc:2|A-5111-OUT,B-9314-RGB,C-9174-OUT;n:type:ShaderForge.SFN_Color,id:9314,x:32760,y:33009,ptovrint:False,ptlb:RimColor,ptin:_RimColor,varname:node_9314,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7803922,c2:0.9686275,c3:1,c4:1;n:type:ShaderForge.SFN_OneMinus,id:7766,x:33251,y:32330,varname:node_7766,prsc:2|IN-5111-OUT;n:type:ShaderForge.SFN_Multiply,id:3265,x:33477,y:32231,varname:node_3265,prsc:2|A-6918-OUT,B-7766-OUT;n:type:ShaderForge.SFN_Multiply,id:8136,x:33802,y:33130,varname:node_8136,prsc:2|A-7766-OUT,B-3519-RGB,C-3099-RGB,D-3099-A;n:type:ShaderForge.SFN_Multiply,id:3597,x:32704,y:31945,varname:node_3597,prsc:2|A-2074-OUT,B-6843-RGB;n:type:ShaderForge.SFN_Slider,id:2074,x:32463,y:31803,ptovrint:False,ptlb:CubeMapPower,ptin:_CubeMapPower,varname:node_2074,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:2;n:type:ShaderForge.SFN_Frac,id:9499,x:31490,y:33138,varname:node_9499,prsc:2|IN-5130-OUT;n:type:ShaderForge.SFN_Subtract,id:6459,x:31654,y:33138,varname:node_6459,prsc:2|A-9499-OUT,B-1713-OUT;n:type:ShaderForge.SFN_Vector1,id:1713,x:31654,y:33270,varname:node_1713,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Abs,id:3359,x:31812,y:33138,varname:node_3359,prsc:2|IN-6459-OUT;n:type:ShaderForge.SFN_Add,id:9040,x:31970,y:33138,varname:node_9040,prsc:2|A-3359-OUT,B-1713-OUT;n:type:ShaderForge.SFN_Power,id:525,x:32149,y:33138,varname:node_525,prsc:2|VAL-9040-OUT,EXP-5509-OUT;n:type:ShaderForge.SFN_Vector1,id:5509,x:31970,y:33268,varname:node_5509,prsc:2,v1:10;n:type:ShaderForge.SFN_ComponentMask,id:7891,x:30751,y:33138,varname:node_7891,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-7253-OUT;n:type:ShaderForge.SFN_Multiply,id:9415,x:30936,y:33138,varname:node_9415,prsc:2|A-7891-R,B-9091-OUT;n:type:ShaderForge.SFN_Multiply,id:7584,x:30936,y:33268,varname:node_7584,prsc:2|A-7891-G,B-8396-OUT;n:type:ShaderForge.SFN_Append,id:9649,x:31105,y:33138,varname:node_9649,prsc:2|A-9415-OUT,B-7584-OUT;n:type:ShaderForge.SFN_ValueProperty,id:9091,x:30751,y:33336,ptovrint:False,ptlb:R,ptin:_R,varname:node_7086,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_ValueProperty,id:8396,x:30751,y:33410,ptovrint:False,ptlb:G,ptin:_G,varname:_node_7086_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_Desaturate,id:2527,x:32319,y:33138,varname:node_2527,prsc:2|COL-525-OUT;n:type:ShaderForge.SFN_Multiply,id:1882,x:30936,y:33410,varname:node_1882,prsc:2|A-7891-B,B-7258-OUT;n:type:ShaderForge.SFN_Append,id:5130,x:31301,y:33138,varname:node_5130,prsc:2|A-9649-OUT,B-1882-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7258,x:30751,y:33500,ptovrint:False,ptlb:B,ptin:_B,varname:_V_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:40;n:type:ShaderForge.SFN_Subtract,id:70,x:32908,y:33159,varname:node_70,prsc:2|A-2527-OUT,B-3215-OUT;n:type:ShaderForge.SFN_Vector1,id:3215,x:32582,y:33227,varname:node_3215,prsc:2,v1:0.5;n:type:ShaderForge.SFN_ToggleProperty,id:9912,x:32908,y:33098,ptovrint:False,ptlb:Toggle,ptin:_Toggle,varname:node_9912,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True;n:type:ShaderForge.SFN_Multiply,id:9323,x:33143,y:33110,varname:node_9323,prsc:2|A-9912-OUT,B-70-OUT;n:type:ShaderForge.SFN_Set,id:9554,x:31903,y:32588,varname:ObjPos,prsc:2|IN-8249-OUT;n:type:ShaderForge.SFN_Get,id:7253,x:30571,y:33138,varname:node_7253,prsc:2|IN-9554-OUT;n:type:ShaderForge.SFN_SwitchProperty,id:7348,x:32496,y:33033,ptovrint:False,ptlb:WireFrame,ptin:_WireFrame,varname:node_7348,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,on:True|A-7206-OUT,B-2527-OUT;n:type:ShaderForge.SFN_Vector1,id:7206,x:32319,y:33067,varname:node_7206,prsc:2,v1:1;n:type:ShaderForge.SFN_Clamp01,id:5340,x:33373,y:32600,varname:node_5340,prsc:2|IN-7713-OUT;n:type:ShaderForge.SFN_Color,id:3099,x:33277,y:33110,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_3099,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Slider,id:9174,x:33377,y:32991,ptovrint:False,ptlb:node_9174,ptin:_node_9174,varname:node_9174,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:3,max:3;n:type:ShaderForge.SFN_Tex2d,id:1130,x:33088,y:31555,ptovrint:False,ptlb:AO,ptin:_AO,varname:node_1130,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_AmbientLight,id:215,x:32884,y:31705,varname:node_215,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8995,x:33337,y:31743,varname:node_8995,prsc:2|A-1130-RGB,B-215-RGB;n:type:ShaderForge.SFN_Slider,id:4871,x:32873,y:31902,ptovrint:False,ptlb:A0S,ptin:_A0S,varname:node_4871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2d,id:2770,x:34513,y:31593,ptovrint:False,ptlb:gaoguang,ptin:_gaoguang,varname:node_2770,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:7289,x:34422,y:31868,varname:node_7289,prsc:2|A-7688-OUT,B-7921-OUT,C-6572-RGB;n:type:ShaderForge.SFN_NormalVector,id:1311,x:33907,y:31664,prsc:2,pt:False;n:type:ShaderForge.SFN_Fresnel,id:7688,x:34146,y:31691,varname:node_7688,prsc:2|NRM-1311-OUT,EXP-8447-OUT;n:type:ShaderForge.SFN_Color,id:6572,x:34255,y:32021,ptovrint:False,ptlb:node_6572,ptin:_node_6572,varname:node_6572,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Add,id:8753,x:34605,y:31890,varname:node_8753,prsc:2|A-2770-RGB,B-7289-OUT;n:type:ShaderForge.SFN_Slider,id:7921,x:33734,y:31923,ptovrint:False,ptlb:Specular2,ptin:_Specular2,varname:node_7921,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:10;n:type:ShaderForge.SFN_Color,id:6510,x:32971,y:32396,ptovrint:False,ptlb:Color_copy,ptin:_Color_copy,varname:_Color_copy,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.4849697,c2:0.517301,c3:0.8455882,c4:1;n:type:ShaderForge.SFN_Tex2d,id:7840,x:33713,y:32111,ptovrint:False,ptlb:node_7840,ptin:_node_7840,varname:node_7840,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2764,x:34030,y:32104,varname:node_2764,prsc:2|A-6543-OUT,B-7840-R;proporder:8214-1683-3685-6843-2074-8084-3519-1130-4871-8608-9314-8447-7921-6543-9091-8396-7258-7348-9912-3099-9174-2770-6572-7840;pass:END;sub:END;*/

Shader "zhongche/ne" {
	Properties{
		_Slider("Slider", Range(-2.5, 2.5)) = 2
		_Tex("Tex", 2D) = "white" {}
		_Color("Color", Color) = (0.5,0.5,0.5,1)
		_Cubemap("Cubemap", Cube) = "_Skybox" {}
		_CubeMapPower("CubeMapPower", Range(0, 2)) = 0.5
		_Normal("Normal", 2D) = "bump" {}
		_Tex_Emission("Tex_Emission", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_A0S("A0S", Range(0, 1)) = 0.5
		_RimThickness("RimThickness", Range(0, 5)) = 4.061508
		_RimColor("RimColor", Color) = (0.7803922,0.9686275,1,1)
		_Specular("Specular", Range(0, 10)) = 0.25
		_Specular2("Specular2", Range(0, 10)) = 0
		_Glow("Glow", Range(0, 1)) = 0.25
		_R("R", Float) = 40
		_G("G", Float) = 40
		_B("B", Float) = 40
		[MaterialToggle] _WireFrame("WireFrame", Float) = 1
		[MaterialToggle] _Toggle("Toggle", Float) = 1
		_Emission("Emission", Color) = (0.5,0.5,0.5,1)
		_node_9174("node_9174", Range(0, 3)) = 3
		_gaoguang("gaoguang", 2D) = "white" {}
		_node_6572("node_6572", Color) = (0.5,0.5,0.5,1)
		_node_7840("node_7840", 2D) = "bump" {}
		[HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.5
	}
		SubShader{
			Tags {
				"Queue" = "Transparent"
				"RenderType" = "TransparentCutout"
			}
			Pass {
				Name "FORWARD"
				Tags {
					"LightMode" = "ForwardBase"
				}


				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_instancing
				#define UNITY_PASS_FORWARDBASE
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				#pragma multi_compile_fwdbase_fullshadows
				#pragma only_renderers d3d9 d3d11 glcore gles gles3 metal 
				#pragma target 3.0
				uniform float4 _LightColor0;
				uniform float _Slider;
				uniform float4 _Color;
				uniform sampler2D _Tex; uniform float4 _Tex_ST;
				uniform samplerCUBE _Cubemap;
				uniform sampler2D _Normal; uniform float4 _Normal_ST;
				uniform sampler2D _Tex_Emission; uniform float4 _Tex_Emission_ST;
				uniform float _Specular;
				uniform float _Glow;
				uniform float _RimThickness;
				uniform float4 _RimColor;
				uniform float _CubeMapPower;
				uniform float _R;
				uniform float _G;
				uniform float _B;
				uniform fixed _Toggle;
				uniform fixed _WireFrame;
				uniform float4 _Emission;
				uniform float _node_9174;
				uniform sampler2D _AO; uniform float4 _AO_ST;
				uniform sampler2D _gaoguang; uniform float4 _gaoguang_ST;
				uniform float4 _node_6572;
				uniform float _Specular2;
				uniform sampler2D _node_7840; uniform float4 _node_7840_ST;
				struct VertexInput {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
					float4 tangent : TANGENT;
					float2 texcoord0 : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};
				struct VertexOutput {
					float4 pos : SV_POSITION;
					float2 uv0 : TEXCOORD0;
					float4 posWorld : TEXCOORD1;
					float3 normalDir : TEXCOORD2;
					float3 tangentDir : TEXCOORD3;
					float3 bitangentDir : TEXCOORD4;
					LIGHTING_COORDS(5,6)
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
				};
				VertexOutput vert(VertexInput v) {
					VertexOutput o = (VertexOutput)0;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.uv0 = v.texcoord0;
					o.normalDir = UnityObjectToWorldNormal(v.normal);
					o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
					o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
					float4 objPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
					o.posWorld = mul(unity_ObjectToWorld, v.vertex);
					float3 lightColor = _LightColor0.rgb;
					o.pos = UnityObjectToClipPos(v.vertex);
					TRANSFER_VERTEX_TO_FRAGMENT(o)
					return o;
				}
				float4 frag(VertexOutput i) : COLOR {
					UNITY_SETUP_INSTANCE_ID(i);
					float4 objPos = mul(unity_ObjectToWorld, float4(0,0,0,1));
					i.normalDir = normalize(i.normalDir);
					float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
					float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
					float3 _Normal_var = UnpackNormal(tex2D(_Normal,TRANSFORM_TEX(i.uv0, _Normal)));
					float3 normalLocal = _Normal_var.rgb;
					float3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
					float3 viewReflectDirection = reflect(-viewDirection, normalDirection);
					float3 node_8249 = (i.posWorld.rgb - objPos.rgb);
					float node_5794 = ((1.0 - node_8249.g) + _Slider);
					float node_5773 = saturate(node_5794);
					float node_5111_if_leA = step(node_5794,_RimThickness);
					float node_5111_if_leB = step(_RimThickness,node_5794);
					float node_4536 = 1.0;
					float node_5111 = lerp((node_5111_if_leA*node_4536) + (node_5111_if_leB*0.0),node_4536,node_5111_if_leA*node_5111_if_leB);
					float3 ObjPos = node_8249;
					float3 node_7891 = ObjPos.rgb;
					float node_1713 = 0.5;
					float node_2527 = dot(pow((abs((frac(float3(float2((node_7891.r*_R),(node_7891.g*_G)),(node_7891.b*_B))) - node_1713)) + node_1713),10.0),float3(0.3,0.59,0.11));
					clip((saturate((node_5773*node_5111*lerp(1.0, node_2527, _WireFrame))) + (_Toggle*(node_2527 - 0.5)) + node_5773) - 0.5);
					float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
					float3 lightColor = _LightColor0.rgb;
					float3 halfDirection = normalize(viewDirection + lightDirection);
					////// Lighting:
									float attenuation = LIGHT_ATTENUATION(i);
									float3 attenColor = attenuation * _LightColor0.xyz;
									///////// Gloss:
													float4 _node_7840_var = tex2D(_node_7840,TRANSFORM_TEX(i.uv0, _node_7840));
													float gloss = (_Glow*_node_7840_var.r);
													float specPow = exp2(gloss * 10.0 + 1.0);
													////// Specular:
																	float NdotL = saturate(dot(normalDirection, lightDirection));
																	float4 _gaoguang_var = tex2D(_gaoguang,TRANSFORM_TEX(i.uv0, _gaoguang));
																	float3 specularColor = (_gaoguang_var.rgb + (pow(1.0 - max(0,dot(i.normalDir, viewDirection)),_Specular)*_Specular2*_node_6572.rgb));
																	float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
																	float3 specular = directSpecular;
																	/////// Diffuse:
																					NdotL = max(0.0,dot(normalDirection, lightDirection));
																					float3 directDiffuse = max(0.0, NdotL) * attenColor;
																					float3 indirectDiffuse = float3(0,0,0);
																					indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
																					float4 _Tex_var = tex2D(_Tex,TRANSFORM_TEX(i.uv0, _Tex));
																					float4 _AO_var = tex2D(_AO,TRANSFORM_TEX(i.uv0, _AO));
																					float node_7766 = (1.0 - node_5111);
																					float3 diffuseColor = (((_Tex_var.rgb*_Color.rgb) + (_CubeMapPower*texCUBE(_Cubemap,viewReflectDirection).rgb) + (_AO_var.rgb*UNITY_LIGHTMODEL_AMBIENT.rgb))*node_7766);
																					float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
																					////// Emissive:
																									float4 _Tex_Emission_var = tex2D(_Tex_Emission,TRANSFORM_TEX(i.uv0, _Tex_Emission));
																									float3 emissive = ((node_5111*_RimColor.rgb*_node_9174) + (node_7766*_Tex_Emission_var.rgb*_Emission.rgb*_Emission.a));
																									/// Final Color:
																													float3 finalColor = diffuse + specular + emissive;
																													return fixed4(finalColor,1);
																												}
																												ENDCG
																											}

		}
			CustomEditor "ShaderForgeMaterialInspector"
}
