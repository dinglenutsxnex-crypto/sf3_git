Shader "Shader Forge/PartAddUVScrollshader" {
	Properties {
		_MainText ("MainText", 2D) = "white" {}
		_node_4041 ("node_4041", Vector) = (0.5,0.5,0.5,1)
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