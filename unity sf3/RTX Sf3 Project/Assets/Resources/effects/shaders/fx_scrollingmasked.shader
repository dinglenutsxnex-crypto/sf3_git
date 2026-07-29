Shader "ShaftFX/Scrolling Masked" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _AlphaMask ("Alpha Mask", 2D) = "white" {}
		_ScrollSpdHor ("Horizontal Scrollig Speed", Float) = 0.1
		_ScrollSpdVer ("Vertical Scrollig Speed", Float) = 0.1
		_EmissivePower ("Emissive power", Float) = 1
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