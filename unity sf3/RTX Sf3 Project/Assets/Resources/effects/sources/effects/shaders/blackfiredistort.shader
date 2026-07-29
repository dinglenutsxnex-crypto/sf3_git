Shader "Shader Forge/BlackFireDistort" {
	Properties {
		_TintColor ("Color_Splash", Vector) = (0,1,1,1)
		_Color_Fire ("Color_Fire", Vector) = (0.09019608,0.09803922,0.1176471,1)
		_FireMult ("FireMult", Float) = 2
		_TextureFire ("TextureFire", 2D) = "white" {}
		_vertexAlphaMult ("vertexAlphaMult", Float) = 2
		_V_SpeedFire1 ("V_SpeedFire1", Float) = -1.5
		_V_SpeedFire2 ("V_SpeedFire", Float) = -1
		_V_SpeedSplash1 ("V_SpeedSplash1", Float) = -2
		_V_SpeedSplash2 ("V_SpeedSplash2", Float) = -1.6
		_FireUp ("FireUp", Float) = -0.6
		_V_SpeedDist ("V_SpeedDist", Float) = -2
		_node_4762 ("node_4762", 2D) = "white" {}
		_DistortMult ("DistortMult", Float) = 0.1
		_Alpha ("Alpha", 2D) = "white" {}
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