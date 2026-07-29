using UnityEngine;

namespace DynamicShadowProjector
{
	[RequireComponent(typeof(ShadowTextureRenderer))]
	public class MipmappedShadowFallback : MonoBehaviour
	{
		public Object m_fallbackShaderOrMaterial;

		public int m_blurLevel = 1;

		public float m_blurSize = 2f;

		public bool m_modifyTextureSize;

		public ShadowTextureRenderer.TextureMultiSample m_multiSampling = ShadowTextureRenderer.TextureMultiSample.x4;

		public ShadowTextureRenderer.TextureSuperSample m_superSampling = ShadowTextureRenderer.TextureSuperSample.x1;

		public int m_textureWidth = 64;

		public int m_textureHeight = 64;

		public Shader m_tex2DlodCheckShader;

		public Shader m_glslCheckShader;

		private void Awake()
		{
			Projector component = GetComponent<Projector>();
			if (component == null || component.material == null)
			{
				return;
			}
			bool isSupported = component.material.shader.isSupported;
			if (isSupported && m_tex2DlodCheckShader != null && m_glslCheckShader != null && m_glslCheckShader.isSupported)
			{
				isSupported = m_tex2DlodCheckShader.isSupported;
			}
			if (!isSupported)
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log("This device does not support tex2Dlod. Use fallback shader instead: " + SystemInfo.graphicsDeviceID);
				}
				ApplyFallback(component);
			}
		}

		public void ApplyFallback(Projector projector)
		{
			if (m_fallbackShaderOrMaterial is Shader)
			{
				projector.material.shader = m_fallbackShaderOrMaterial as Shader;
			}
			else if (m_fallbackShaderOrMaterial is Material)
			{
				projector.material = m_fallbackShaderOrMaterial as Material;
			}
			ShadowTextureRenderer component = projector.GetComponent<ShadowTextureRenderer>();
			component.blurLevel = m_blurLevel;
			component.blurSize = m_blurSize;
			component.mipLevel = 0;
			if (m_modifyTextureSize)
			{
				component.textureWidth = m_textureWidth;
				component.textureHeight = m_textureHeight;
				component.multiSampling = m_multiSampling;
				component.superSampling = m_superSampling;
			}
		}
	}
}
