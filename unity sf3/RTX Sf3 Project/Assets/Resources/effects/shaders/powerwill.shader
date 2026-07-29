Shader "Unlit/PowerWill" {
	Properties {
		_SFX_ShadowShieldTex ("SFX - Shadow shield textures", 2D) = "white" {}
		_EffectTransition ("SFX - Shadow shield blend", Range(0, 1)) = 1
		_ShadowForm ("Shadowform base", 2D) = "white" {}
		_SFX_SSTune ("SFX - Shadow shield speed", Vector) = (1,1,1,1)
		_SFX_Weight ("SFX - Shadow shield weight", Vector) = (1,1,1,1)
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