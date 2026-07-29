Shader "Nekki/CharacterShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MainTexPreRender ("Base (RGB)", 2D) = "white" {}
		_MatCap ("MatCap", 2D) = "white" {}
		_Mask ("MatCap Mask", 2D) = "white" {}
		_ShadowForm ("Shadowform base", 2D) = "white" {}
		_ShadowFormPreRender ("Shadowform prerender", 2D) = "white" {}
		_BlendValue ("Blend value", Range(0, 1)) = 0
		_MaskSize ("Mask size", Float) = 10
		_FogStart ("Fog Start - Y", Float) = 0
		_FogEnd ("Fog End - Y", Float) = 0
		_MainColor ("Main color", Vector) = (1,1,1,1)
		_MaskColor ("Mask color", Vector) = (1,1,1,1)
		_MaskEnemyColor ("Enemy mask color", Vector) = (1,1,1,1)
		_MaskPlayerColor ("Player mask color", Vector) = (1,1,1,1)
		[Enum(Off,0,Front,1,Back,2)] _Cull ("__cull", Float) = 2
		_GradientStart ("Gradient start", Float) = 0.25
		_GradientEnd ("Gradient start", Float) = 0.5
		_GradientStartColor ("Gradient start color", Vector) = (0,0,0,1)
		_GradientEndColor ("Gradient start color", Vector) = (1,1,1,1)
		_GlowTex ("GlowTex", 2D) = "white" {}
		_GlowColor ("Glow Color", Vector) = (1,1,1,1)
		_GlowColorMult ("Glow Color Multiplier", Vector) = (1,1,1,1)
		_GlowPower ("Glow Power", Float) = 1
		_MaskStartColor ("Mask Start  Color", Vector) = (1,1,1,1)
		_MaskEndColor ("Mask Start  Color", Vector) = (1,1,1,1)
		_SkinColor ("Skin  Color", Vector) = (1,1,1,1)
		_SkinColorBase ("Skin  Color", Vector) = (1,1,1,1)
		_SkinColorDifference ("Skin  Color Difference", Vector) = (1,1,1,1)
		[HideInInspector] _SrcBlend ("__src", Float) = 1
		[HideInInspector] _DstBlend ("__dst", Float) = 0
		[HideInInspector] _ZWrite ("__zw", Float) = 1
		_Alpha ("Alpha", Float) = 1
		_AlphaTex ("Alpha Texture", 2D) = "white" {}
		_MetalPower ("Metal power", Float) = 2
		_RimMaterialPower ("Rim power", Float) = 1
		[Enum(Weapon,0,Armor,1,Head,2,Trail,3)] _MaterialType ("Material type", Float) = 0
		[Enum(Off,0,Linear,1,LinearWithYBias,2)] _FogType ("FogType", Float) = 0
		[Enum(Off,0,On,1)] _IgnoreLight ("Ignore Light", Float) = 0
		[Enum(Off,0,On,1)] _ShadowFormEnabled ("Shadow Form", Float) = 0
		[Enum(Off,0,On,1)] _UseSkinColor ("Use Skin Color", Float) = 0
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
	//CustomEditor "CharacterMaterialInspector"
}