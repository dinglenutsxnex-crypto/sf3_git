Shader "Nekki/LocationShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTexPreRender ("Base (RGB)", 2D) = "white" {}
		_LightMapDefault ("Lightmap default x2 (RGB)", 2D) = "white" {}
		_LightMapShadowForm ("Lightmap shadow form x2 (RGB)", 2D) = "white" {}
		_MaskSize ("Mask size", Float) = 10
		_FogStart ("Fog Start - Y", Float) = 0
		_FogEnd ("Fog End - Y", Float) = 0
		_VertexAnimationFactorX ("Vertex animation factor - X", Float) = 0.1
		_VertexAnimationFactorY ("Vertex animation factor - Y", Float) = 0.1
		_MatCap ("MatCap", 2D) = "white" {}
		_Offset ("Offset", Vector) = (0,0,0,0)
		_LightmapOffset ("Lightmap Offset", Vector) = (0,0,0,0)
		_DecalTex ("Decal texture", 2D) = "white" {}
		_MainColor ("Main color", Vector) = (1,1,1,1)
		[HideInInspector] _Mode ("__mode", Float) = 0
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _ZWrite ("__zw", Float) = 1
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
	//CustomEditor "LocationMaterialInspector"
}