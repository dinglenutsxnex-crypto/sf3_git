Shader "Shader Forge/BlackFireTorch" {
	Properties {
		_Color_Fire ("Color_Fire", Vector) = (0.09019608,0.09803922,0.1176471,1)
		_FireMult ("FireMult", Float) = 2
		_TextureFire ("TextureFire", 2D) = "white" {}
		_vertexAlphaMult ("vertexAlphaMult", Float) = 2
		_V_SpeedFire1 ("V_SpeedFire1", Float) = -1.5
		_V_SpeedFire ("V_SpeedFire", Float) = -1
		_U_SpeedFire1 ("U_SpeedFire1", Float) = 0
		_U_SpeedFire ("U_SpeedFire", Float) = 0
		_FireUp ("FireUp", Float) = -0.6
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
}