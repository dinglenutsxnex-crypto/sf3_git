Shader "ShaftFX/SadowCarapace" {
	Properties {
		[NoScaleOffset] _MainTex ("Main texture", 2D) = "white" {}
		_ScrollTex ("Scrolling texture", 2D) = "white" {}
		_AlphaMask ("Alpha mask", 2D) = "white" {}
		_AMscrollSpeed ("Alpha scrolling speed", Float) = 1
		_ScrollingColor ("Color", Vector) = (1,1,1,1)
		_ScrollSpdHor ("Horizontal Scrollig Speed", Float) = 0.1
		_ScrollSpdVer ("Vertical Scrollig Speed", Float) = 0.1
		_ScrollSize ("Scrolling texture tiling amount", Float) = 0.1
		_EmissivePower ("Scrolling texture power", Float) = 1
		_AnimState ("Animation state, passed from outside", Float) = 0
		_Transparency ("Transparency, passed from outside", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}