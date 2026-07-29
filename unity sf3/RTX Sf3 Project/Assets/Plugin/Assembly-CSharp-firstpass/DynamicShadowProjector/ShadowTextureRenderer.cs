using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DynamicShadowProjector
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Projector))]
	public class ShadowTextureRenderer : MonoBehaviour
	{
		public enum TextureMultiSample
		{
			x1 = 1,
			x2 = 2,
			x4 = 4,
			x8 = 8
		}

		public enum TextureSuperSample
		{
			x1 = 1,
			x4 = 2,
			x16 = 4
		}

		public enum MipmapFalloff
		{
			None = 0,
			Linear = 1,
			Custom = 2
		}

		public enum BlurFilter
		{
			Uniform = 0,
			Gaussian = 1
		}

		private struct BlurParam
		{
			public int tap;

			public Vector4 offset;

			public Vector4 weight;
		}

		[SerializeField]
		private TextureMultiSample m_multiSampling = TextureMultiSample.x4;

		[SerializeField]
		private TextureSuperSample m_superSampling = TextureSuperSample.x1;

		[SerializeField]
		private MipmapFalloff m_mipmapFalloff = MipmapFalloff.Linear;

		[SerializeField]
		private BlurFilter m_blurFilter;

		[SerializeField]
		private bool m_testViewClip = true;

		[SerializeField]
		private int m_textureWidth = 64;

		[SerializeField]
		private int m_textureHeight = 64;

		[SerializeField]
		private int m_mipLevel;

		[SerializeField]
		private int m_blurLevel = 1;

		[SerializeField]
		private float m_blurSize = 3f;

		[SerializeField]
		private float m_mipmapBlurSize;

		[SerializeField]
		private bool m_singlePassMipmapBlur;

		[SerializeField]
		private Color m_shadowColor = new Color(0f, 0f, 0f, 1f);

		[SerializeField]
		private Material m_blurShader;

		[SerializeField]
		private Material m_downsampleShader;

		[SerializeField]
		private Material m_copyMipmapShader;

		[SerializeField]
		private Material m_eraseShadowShader;

		[SerializeField]
		private float[] m_customMipmapFalloff;

		[SerializeField]
		private RenderTextureFormat[] m_preferredTextureFormats;

		[SerializeField]
		private Camera[] m_camerasForViewClipTest;

		private static int s_falloffParamID;

		private static int s_blurOffsetHParamID;

		private static int s_blurOffsetVParamID;

		private static int s_blurWeightHParamID;

		private static int s_blurWeightVParamID;

		private static int s_downSampleBlurOffset0ParamID;

		private static int s_downSampleBlurOffset1ParamID;

		private static int s_downSampleBlurOffset2ParamID;

		private static int s_downSampleBlurOffset3ParamID;

		private static int s_downSampleBlurWeightParamID;

		private Projector m_projector;

		private Material m_projectorMaterial;

		private CommandBuffer m_commandBuffer;

		private RenderTexture m_shadowTexture;

		[SerializeField]
		[HideInInspector]
		private Camera m_camera;

		private bool m_isTexturePropertyChanged;

		private bool m_isVisible;

		private static HashSet<Material> s_sharedMaterials;

		private const HideFlags CLONED_MATERIAL_HIDE_FLAGS = HideFlags.HideAndDontSave;

		private const int MAX_BLUR_TAP_SIZE = 7;

		private static float[] s_blurWeights = new float[7];

		public TextureMultiSample multiSampling
		{
			get
			{
				return m_multiSampling;
			}
			set
			{
				if (m_multiSampling != value)
				{
					m_multiSampling = value;
					SetTexturePropertyDirty();
				}
			}
		}

		public TextureSuperSample superSampling
		{
			get
			{
				return m_superSampling;
			}
			set
			{
				if (m_superSampling != value)
				{
					bool flag = useIntermediateTexture;
					m_superSampling = value;
					if (flag != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1)
					{
						SetTexturePropertyDirty();
					}
				}
			}
		}

		public int textureWidth
		{
			get
			{
				return m_textureWidth;
			}
			set
			{
				if (m_textureWidth != value)
				{
					m_textureWidth = value;
					SetTexturePropertyDirty();
				}
			}
		}

		public int textureHeight
		{
			get
			{
				return m_textureHeight;
			}
			set
			{
				if (m_textureHeight != value)
				{
					m_textureHeight = value;
					SetTexturePropertyDirty();
				}
			}
		}

		public int mipLevel
		{
			get
			{
				return m_mipLevel;
			}
			set
			{
				if (m_mipLevel != value)
				{
					if (m_mipLevel == 0 || value == 0)
					{
						SetTexturePropertyDirty();
					}
					m_mipLevel = value;
				}
			}
		}

		public int blurLevel
		{
			get
			{
				return m_blurLevel;
			}
			set
			{
				if (m_blurLevel != value)
				{
					bool flag = useIntermediateTexture;
					m_blurLevel = value;
					if (flag != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1)
					{
						SetTexturePropertyDirty();
					}
				}
			}
		}

		public float blurSize
		{
			get
			{
				return m_blurSize;
			}
			set
			{
				m_blurSize = value;
			}
		}

		public BlurFilter blurFilter
		{
			get
			{
				return m_blurFilter;
			}
			set
			{
				m_blurFilter = value;
			}
		}

		public float mipmapBlurSize
		{
			get
			{
				return m_mipmapBlurSize;
			}
			set
			{
				m_mipmapBlurSize = value;
			}
		}

		public bool singlePassMipmapBlur
		{
			get
			{
				return m_singlePassMipmapBlur;
			}
			set
			{
				m_singlePassMipmapBlur = value;
			}
		}

		public MipmapFalloff mipmapFalloff
		{
			get
			{
				return m_mipmapFalloff;
			}
			set
			{
				m_mipmapFalloff = value;
			}
		}

		public float[] customMipmapFalloff
		{
			get
			{
				return m_customMipmapFalloff;
			}
			set
			{
				m_customMipmapFalloff = value;
			}
		}

		public Color shadowColor
		{
			get
			{
				return m_shadowColor;
			}
			set
			{
				if (m_shadowColor != value)
				{
					bool flag = useIntermediateTexture;
					m_shadowColor = value;
					if (flag != useIntermediateTexture && m_multiSampling != TextureMultiSample.x1)
					{
						SetTexturePropertyDirty();
					}
				}
			}
		}

		public Material blurShader
		{
			get
			{
				return m_blurShader;
			}
			set
			{
				m_blurShader = value;
			}
		}

		public Material downsampleShader
		{
			get
			{
				return m_downsampleShader;
			}
			set
			{
				m_downsampleShader = value;
			}
		}

		public Material copyMipmapShader
		{
			get
			{
				return m_copyMipmapShader;
			}
			set
			{
				m_copyMipmapShader = value;
			}
		}

		public Material eraseShadowShader
		{
			get
			{
				return m_eraseShadowShader;
			}
			set
			{
				m_eraseShadowShader = value;
			}
		}

		public RenderTexture shadowTexture
		{
			get
			{
				return m_shadowTexture;
			}
		}

		public bool testViewClip
		{
			get
			{
				return m_testViewClip;
			}
			set
			{
				m_testViewClip = value;
			}
		}

		public Camera[] camerasForViewClipTest
		{
			get
			{
				return m_camerasForViewClipTest;
			}
			set
			{
				m_camerasForViewClipTest = value;
			}
		}

		public float cameraNearClipPlane
		{
			get
			{
				if (m_camera == null)
				{
					Initialize();
				}
				return m_camera.nearClipPlane;
			}
			set
			{
				if (m_camera == null)
				{
					Initialize();
				}
				m_camera.nearClipPlane = value;
			}
		}

		public LayerMask cameraCullingMask
		{
			get
			{
				if (m_camera == null)
				{
					Initialize();
				}
				return m_camera.cullingMask;
			}
			set
			{
				if (m_camera == null)
				{
					Initialize();
				}
				m_camera.cullingMask = value;
			}
		}

		public bool isProjectorVisible
		{
			get
			{
				return m_isVisible;
			}
		}

		private bool useIntermediateTexture
		{
			get
			{
				return m_superSampling != TextureSuperSample.x1 || 0 < m_blurLevel || HasShadowColor() || (0 < m_mipLevel && m_multiSampling != TextureMultiSample.x1);
			}
		}

		public void SetReplacementShader(Shader shader, string replacementTag)
		{
			if (m_camera == null)
			{
				Initialize();
			}
			if (shader != null)
			{
				m_camera.SetReplacementShader(shader, replacementTag);
			}
			else
			{
				m_camera.ResetReplacementShader();
			}
		}

		public void SetTexturePropertyDirty()
		{
			m_isTexturePropertyChanged = true;
		}

		public void CreateRenderTexture()
		{
			if (m_textureWidth <= 0 || m_textureHeight <= 0 || m_projector == null)
			{
				return;
			}
			RenderTextureFormat format = RenderTextureFormat.ARGB32;
			if (m_preferredTextureFormats != null && 0 < m_preferredTextureFormats.Length)
			{
				RenderTextureFormat[] preferredTextureFormats = m_preferredTextureFormats;
				foreach (RenderTextureFormat renderTextureFormat in preferredTextureFormats)
				{
					if (SystemInfo.SupportsRenderTextureFormat(format))
					{
						format = renderTextureFormat;
					}
				}
			}
			if (m_shadowTexture != null)
			{
				Object.DestroyImmediate(m_shadowTexture);
			}
			m_shadowTexture = new RenderTexture(m_textureWidth, m_textureHeight, 0, format, RenderTextureReadWrite.Linear);
			if (useIntermediateTexture)
			{
				m_shadowTexture.antiAliasing = 1;
			}
			else
			{
				m_shadowTexture.antiAliasing = (int)m_multiSampling;
			}
			if (0 < m_mipLevel)
			{
				m_shadowTexture.useMipMap = true;
				m_shadowTexture.autoGenerateMips = false;
				m_shadowTexture.mipMapBias = 0f;
				m_shadowTexture.filterMode = FilterMode.Trilinear;
			}
			else
			{
				m_shadowTexture.useMipMap = false;
				m_shadowTexture.filterMode = FilterMode.Bilinear;
			}
			m_shadowTexture.wrapMode = TextureWrapMode.Clamp;
			m_shadowTexture.Create();
			if (m_projector.material != null)
			{
				m_projector.material.SetTexture("_ShadowTex", m_shadowTexture);
				m_projector.material.SetFloat("_DSPMipLevel", m_mipLevel);
			}
			m_isTexturePropertyChanged = false;
		}

		public void AddCommandBuffer(CommandBuffer commandBuffer)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
			m_camera.AddCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		}

		public void RemoveCommandBuffer(CommandBuffer commandBuffer)
		{
			m_camera.RemoveCommandBuffer(CameraEvent.BeforeImageEffectsOpaque, commandBuffer);
		}

		private static void InitializeShaderPropertyIDs()
		{
			s_falloffParamID = Shader.PropertyToID("_Falloff");
			s_blurOffsetHParamID = Shader.PropertyToID("_OffsetH");
			s_blurOffsetVParamID = Shader.PropertyToID("_OffsetV");
			s_blurWeightHParamID = Shader.PropertyToID("_WeightH");
			s_blurWeightVParamID = Shader.PropertyToID("_WeightV");
			s_downSampleBlurOffset0ParamID = Shader.PropertyToID("_Offset0");
			s_downSampleBlurOffset1ParamID = Shader.PropertyToID("_Offset1");
			s_downSampleBlurOffset2ParamID = Shader.PropertyToID("_Offset2");
			s_downSampleBlurOffset3ParamID = Shader.PropertyToID("_Offset3");
			s_downSampleBlurWeightParamID = Shader.PropertyToID("_Weight");
		}

		private bool Initialize()
		{
			m_isVisible = false;
			if (IsInitialized())
			{
				return true;
			}
			m_isTexturePropertyChanged = true;
			InitializeShaderPropertyIDs();
			m_projector = GetComponent<Projector>();
			CloneProjectorMaterialIfShared();
			if (m_camera == null)
			{
				m_camera = base.gameObject.GetComponent<Camera>();
				if (m_camera == null)
				{
					m_camera = base.gameObject.AddComponent<Camera>();
				}
				m_camera.hideFlags = HideFlags.HideInInspector;
			}
			else
			{
				m_camera.RemoveAllCommandBuffers();
			}
			m_camera.depth = -100f;
			m_camera.cullingMask = 0;
			m_camera.clearFlags = CameraClearFlags.Nothing;
			m_camera.backgroundColor = new Color(1f, 1f, 1f, 0f);
			m_camera.useOcclusionCulling = false;
			m_camera.renderingPath = RenderingPath.Forward;
			m_camera.nearClipPlane = 0.01f;
			m_camera.enabled = true;
			CreateRenderTexture();
			return true;
		}

		private bool IsInitialized()
		{
			return m_projector != null && m_camera != null;
		}

		private void Awake()
		{
			Initialize();
		}

		private void OnEnable()
		{
			if (m_camera != null)
			{
				m_camera.enabled = true;
			}
		}

		private void OnDisable()
		{
			if (m_camera != null)
			{
				m_camera.enabled = false;
			}
		}

		private void Start()
		{
			if (m_testViewClip && (m_camerasForViewClipTest == null || m_camerasForViewClipTest.Length == 0))
			{
				m_camerasForViewClipTest = new Camera[1] { Camera.main };
			}
		}

		private void OnValidate()
		{
			SetTexturePropertyDirty();
			InitializeShaderPropertyIDs();
			if (m_mipmapFalloff != MipmapFalloff.Custom || 0 >= m_mipLevel)
			{
				return;
			}
			if (m_customMipmapFalloff == null || m_customMipmapFalloff.Length == 0)
			{
				m_customMipmapFalloff = new float[m_mipLevel];
				for (int i = 0; i < m_mipLevel; i++)
				{
					m_customMipmapFalloff[i] = (float)(m_mipLevel - i) / (float)(m_mipLevel + 1);
				}
			}
			else if (m_mipLevel != m_customMipmapFalloff.Length)
			{
				float[] array = new float[m_mipLevel];
				for (int j = 0; j < m_mipLevel; j++)
				{
					float num = (float)(m_customMipmapFalloff.Length + 1) * (float)(j + 1) / (float)(m_mipLevel + 1);
					int num2 = Mathf.FloorToInt(num);
					float t = num - (float)num2;
					float a = ((num2 != 0) ? m_customMipmapFalloff[num2 - 1] : 1f);
					float b = ((num2 >= m_customMipmapFalloff.Length) ? 0f : m_customMipmapFalloff[num2]);
					array[j] = Mathf.Lerp(a, b, t);
				}
				m_customMipmapFalloff = array;
			}
		}

		private void CloneProjectorMaterialIfShared()
		{
			if (!(m_projector.material == null) && (m_projector.material.hideFlags != HideFlags.HideAndDontSave || !(m_projector.material == m_projectorMaterial)))
			{
				if (m_projectorMaterial != null && m_projectorMaterial.hideFlags == HideFlags.HideAndDontSave)
				{
					Object.DestroyImmediate(m_projectorMaterial);
				}
				if (s_sharedMaterials == null)
				{
					s_sharedMaterials = new HashSet<Material>();
				}
				if (s_sharedMaterials.Contains(m_projector.material))
				{
					m_projector.material = new Material(m_projector.material);
					m_projector.material.hideFlags = HideFlags.HideAndDontSave;
				}
				else
				{
					s_sharedMaterials.Add(m_projector.material);
				}
				m_projectorMaterial = m_projector.material;
			}
		}

		private void OnDestroy()
		{
			if (m_projectorMaterial != null)
			{
				if (s_sharedMaterials != null && s_sharedMaterials.Contains(m_projectorMaterial))
				{
					s_sharedMaterials.Remove(m_projectorMaterial);
				}
				if (m_projectorMaterial.hideFlags == HideFlags.HideAndDontSave)
				{
					if (m_projector.material == m_projectorMaterial)
					{
						m_projector.material = null;
					}
					Object.DestroyImmediate(m_projectorMaterial);
				}
			}
			if (m_shadowTexture != null)
			{
				Object.DestroyImmediate(m_shadowTexture);
				m_shadowTexture = null;
			}
			if (m_camera != null)
			{
				m_camera.RemoveAllCommandBuffers();
			}
			m_isVisible = false;
		}

		private bool IsReadyToExecute()
		{
			if (m_textureWidth <= 0 || m_textureHeight <= 0 || m_eraseShadowShader == null)
			{
				return false;
			}
			if ((0 < m_mipLevel || m_superSampling != TextureSuperSample.x1) && m_downsampleShader == null)
			{
				return false;
			}
			if ((0 < m_blurLevel || (0f < m_mipmapBlurSize && 0 < m_mipLevel)) && m_blurShader == null)
			{
				return false;
			}
			if (0 < m_mipLevel && (m_copyMipmapShader == null || m_downsampleShader == null))
			{
				return false;
			}
			return true;
		}

		private void SetVisible(bool isVisible)
		{
			m_isVisible = isVisible;
			SendMessage("OnVisibilityChanged", isVisible);
		}

		private void OnPreCull()
		{
			if (m_projector.material != m_projectorMaterial)
			{
				CloneProjectorMaterialIfShared();
				m_projector.material.SetTexture("_ShadowTex", m_shadowTexture);
				m_projector.material.SetFloat("_DSPMipLevel", m_mipLevel);
			}
			if (m_isTexturePropertyChanged)
			{
				CreateRenderTexture();
			}
			m_camera.orthographic = m_projector.orthographic;
			m_camera.orthographicSize = m_projector.orthographicSize;
			m_camera.fieldOfView = m_projector.fieldOfView;
			m_camera.aspect = m_projector.aspectRatio;
			m_camera.farClipPlane = m_projector.farClipPlane;
			bool flag = true;
			if (!m_projector.enabled)
			{
				flag = false;
			}
			else if (m_testViewClip && m_camerasForViewClipTest != null && 0 < m_camerasForViewClipTest.Length)
			{
				Vector3 position = m_camera.ViewportToWorldPoint(new Vector3(0f, 0f, m_camera.nearClipPlane));
				Vector3 position2 = m_camera.ViewportToWorldPoint(new Vector3(1f, 0f, m_camera.nearClipPlane));
				Vector3 position3 = m_camera.ViewportToWorldPoint(new Vector3(0f, 1f, m_camera.nearClipPlane));
				Vector3 position4 = m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, m_camera.nearClipPlane));
				Vector3 position5 = m_camera.ViewportToWorldPoint(new Vector3(0f, 0f, m_camera.farClipPlane));
				Vector3 position6 = m_camera.ViewportToWorldPoint(new Vector3(1f, 0f, m_camera.farClipPlane));
				Vector3 position7 = m_camera.ViewportToWorldPoint(new Vector3(0f, 1f, m_camera.farClipPlane));
				Vector3 position8 = m_camera.ViewportToWorldPoint(new Vector3(1f, 1f, m_camera.farClipPlane));
				flag = false;
				for (int i = 0; i < m_camerasForViewClipTest.Length; i++)
				{
					Camera camera = m_camerasForViewClipTest[i];
					Vector3 vector = camera.WorldToViewportPoint(position);
					if (vector.z < 0f)
					{
						vector.x = 0f - vector.x;
						vector.y = 0f - vector.y;
					}
					Vector3 vector2 = vector;
					Vector3 vector3 = camera.WorldToViewportPoint(position2);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					vector3 = camera.WorldToViewportPoint(position3);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					vector3 = camera.WorldToViewportPoint(position4);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					vector3 = camera.WorldToViewportPoint(position5);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					vector3 = camera.WorldToViewportPoint(position6);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					vector3 = camera.WorldToViewportPoint(position7);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					vector3 = camera.WorldToViewportPoint(position8);
					if (vector3.z < 0f)
					{
						vector3.x = 0f - vector3.x;
						vector3.y = 0f - vector3.y;
					}
					vector.x = Mathf.Min(vector.x, vector3.x);
					vector.y = Mathf.Min(vector.y, vector3.y);
					vector.z = Mathf.Min(vector.z, vector3.z);
					vector2.x = Mathf.Max(vector2.x, vector3.x);
					vector2.y = Mathf.Max(vector2.y, vector3.y);
					vector2.z = Mathf.Max(vector2.z, vector3.z);
					if (0f < vector2.x && vector.x < 1f && 0f < vector2.y && vector.y < 1f && camera.nearClipPlane < vector2.z && vector.z < camera.farClipPlane)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag != m_isVisible)
			{
				SetVisible(flag);
			}
		}

		private bool HasShadowColor()
		{
			return m_shadowColor.a != 1f || m_shadowColor.r + shadowColor.g + shadowColor.b != 0f;
		}

		private void OnPreRender()
		{
			if (m_isVisible)
			{
				m_shadowTexture.DiscardContents();
				if (useIntermediateTexture)
				{
					int width = m_textureWidth * (int)m_superSampling;
					int height = m_textureHeight * (int)m_superSampling;
					m_camera.targetTexture = RenderTexture.GetTemporary(width, height, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear, (int)m_multiSampling);
					m_camera.targetTexture.filterMode = FilterMode.Bilinear;
				}
				else
				{
					m_camera.targetTexture = m_shadowTexture;
				}
				m_camera.clearFlags = CameraClearFlags.Color;
			}
		}

		private static BlurParam GetBlurParam(float blurSize, BlurFilter filter)
		{
			BlurParam result = default(BlurParam);
			if (blurSize < 0.1f)
			{
				result.tap = 3;
				result.offset.x = 0f;
				result.offset.y = 0f;
				result.offset.z = 0f;
				result.offset.w = 0f;
				result.weight.x = 1f;
				result.weight.y = 0f;
				result.weight.z = 0f;
				result.weight.w = 0f;
				return result;
			}
			if (filter == BlurFilter.Gaussian)
			{
				float num = 1f / (2f * blurSize * blurSize);
				float num2 = 1f;
				s_blurWeights[0] = 1f;
				for (int i = 1; i < s_blurWeights.Length; i++)
				{
					s_blurWeights[i] = Mathf.Exp((float)(-i * i) * num);
					num2 += 2f * s_blurWeights[i];
				}
				float num3 = 1f / num2;
				for (int j = 0; j < s_blurWeights.Length; j++)
				{
					s_blurWeights[j] *= num3;
				}
			}
			else
			{
				float num4 = 0.5f / (0.5f + blurSize);
				for (int k = 0; k < s_blurWeights.Length; k++)
				{
					if ((float)k <= blurSize)
					{
						s_blurWeights[k] = num4;
					}
					else if ((float)(k - 1) < blurSize)
					{
						s_blurWeights[k] = num4 * (blurSize - (float)(k - 1));
					}
					else
					{
						s_blurWeights[k] = 0f;
					}
				}
			}
			result.offset.x = 1f + s_blurWeights[2] / (s_blurWeights[1] + s_blurWeights[2]);
			result.offset.y = 3f + s_blurWeights[4] / (s_blurWeights[3] + s_blurWeights[4]);
			result.offset.z = 5f + s_blurWeights[6] / (s_blurWeights[5] + s_blurWeights[6]);
			result.offset.w = 0f;
			if (s_blurWeights[3] < 0.02f)
			{
				result.tap = 3;
				float num5 = 0.5f / (0.5f * s_blurWeights[0] + s_blurWeights[1] + s_blurWeights[2]);
				result.weight.x = Mathf.Round(255f * num5 * s_blurWeights[0]) / 255f;
				result.weight.y = 0.5f - 0.5f * result.weight.x;
				result.weight.z = 0f;
				result.weight.w = 0f;
			}
			else if (s_blurWeights[5] < 0.02f)
			{
				result.tap = 5;
				float num6 = 0.5f / (0.5f * s_blurWeights[0] + s_blurWeights[1] + s_blurWeights[2] + s_blurWeights[3] + s_blurWeights[4]);
				result.weight.x = Mathf.Round(255f * num6 * s_blurWeights[0]) / 255f;
				result.weight.y = Mathf.Round(255f * num6 * (s_blurWeights[1] + s_blurWeights[2])) / 255f;
				result.weight.z = 0.5f - (0.5f * result.weight.x + result.weight.y);
				result.weight.w = 0f;
			}
			else
			{
				result.tap = 7;
				result.weight.x = Mathf.Round(255f * s_blurWeights[0]) / 255f;
				result.weight.y = Mathf.Round(255f * (s_blurWeights[1] + s_blurWeights[2])) / 255f;
				result.weight.z = Mathf.Round(255f * (s_blurWeights[3] + s_blurWeights[4])) / 255f;
				result.weight.w = 0.5f - (0.5f * result.weight.x + result.weight.y + result.weight.z);
			}
			return result;
		}

		private static BlurParam GetDownsampleBlurParam(float blurSize, BlurFilter filter)
		{
			BlurParam result = default(BlurParam);
			result.tap = 4;
			if (blurSize < 0.1f)
			{
				result.offset.x = 0f;
				result.offset.y = 0f;
				result.offset.z = 0f;
				result.offset.w = 0f;
				result.weight.x = 1f;
				result.weight.y = 0f;
				result.weight.z = 0f;
				result.weight.w = 0f;
				return result;
			}
			if (filter == BlurFilter.Gaussian)
			{
				float num = 1f / (2f * blurSize * blurSize);
				float num2 = 0f;
				for (int i = 0; i < result.tap; i++)
				{
					float num3 = (float)i + 0.5f;
					s_blurWeights[i] = Mathf.Exp((0f - num3) * num3 * num);
					num2 += 2f * s_blurWeights[i];
				}
				float num4 = 1f / num2;
				for (int j = 0; j < result.tap; j++)
				{
					s_blurWeights[j] *= num4;
				}
			}
			else
			{
				float num5 = 0.5f / blurSize;
				for (int k = 0; k < result.tap; k++)
				{
					if ((float)(k + 1) <= blurSize)
					{
						s_blurWeights[k] = num5;
					}
					else if ((float)k < blurSize)
					{
						s_blurWeights[k] = num5 * (blurSize - (float)k);
					}
					else
					{
						s_blurWeights[k] = 0f;
					}
				}
			}
			result.offset.x = 0.5f + s_blurWeights[1] / (s_blurWeights[0] + s_blurWeights[1]);
			result.offset.y = 2.5f + s_blurWeights[3] / (s_blurWeights[2] + s_blurWeights[3]);
			result.offset.z = 0f;
			result.offset.w = 0f;
			result.weight.x = s_blurWeights[0] + s_blurWeights[1];
			result.weight.y = s_blurWeights[2] + s_blurWeights[3];
			result.weight.z = 0f;
			result.weight.w = 0f;
			return result;
		}

		private void OnPostRender()
		{
			m_camera.clearFlags = CameraClearFlags.Nothing;
			if (!m_isVisible)
			{
				return;
			}
			RenderTexture renderTexture = m_camera.targetTexture;
			m_camera.targetTexture = null;
			if (m_superSampling != TextureSuperSample.x1 || HasShadowColor())
			{
				m_downsampleShader.color = m_shadowColor;
				RenderTexture temporary;
				if (0 < m_blurLevel)
				{
					temporary = RenderTexture.GetTemporary(m_textureWidth, m_textureHeight, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
					temporary.filterMode = FilterMode.Bilinear;
				}
				else
				{
					temporary = m_shadowTexture;
				}
				Graphics.SetRenderTarget(temporary);
				int num = ((m_superSampling != TextureSuperSample.x16) ? 2 : 0);
				Graphics.Blit(renderTexture, temporary, m_downsampleShader, (!HasShadowColor()) ? num : (num + 1));
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary;
			}
			if (0 < m_blurLevel)
			{
				float num2 = m_projector.aspectRatio * (float)m_textureHeight / (float)m_textureWidth;
				float num3 = m_blurSize;
				float num4 = m_blurSize;
				if (num2 < 1f)
				{
					num4 *= num2;
				}
				else
				{
					num3 /= num2;
				}
				BlurParam blurParam = GetBlurParam(num3, m_blurFilter);
				BlurParam blurParam2 = GetBlurParam(num4, m_blurFilter);
				blurParam.tap -= 3;
				blurParam2.tap = blurParam2.tap - 3 + 1;
				m_blurShader.SetVector(s_blurOffsetHParamID, blurParam.offset);
				m_blurShader.SetVector(s_blurOffsetVParamID, blurParam2.offset);
				m_blurShader.SetVector(s_blurWeightHParamID, blurParam.weight);
				m_blurShader.SetVector(s_blurWeightVParamID, blurParam2.weight);
				RenderTexture temporary2 = RenderTexture.GetTemporary(m_textureWidth, m_textureHeight, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
				temporary2.filterMode = FilterMode.Bilinear;
				renderTexture.wrapMode = TextureWrapMode.Clamp;
				temporary2.wrapMode = TextureWrapMode.Clamp;
				Graphics.Blit(renderTexture, temporary2, m_blurShader, blurParam.tap);
				if (1 < renderTexture.antiAliasing)
				{
					RenderTexture.ReleaseTemporary(renderTexture);
					renderTexture = RenderTexture.GetTemporary(m_textureWidth, m_textureHeight, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
				}
				else
				{
					renderTexture.DiscardContents();
				}
				for (int i = 1; i < m_blurLevel - 1; i++)
				{
					Graphics.Blit(temporary2, renderTexture, m_blurShader, blurParam2.tap);
					temporary2.DiscardContents();
					Graphics.Blit(renderTexture, temporary2, m_blurShader, blurParam.tap);
					renderTexture.DiscardContents();
				}
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = m_shadowTexture;
				Graphics.Blit(temporary2, renderTexture, m_blurShader, blurParam2.tap);
				RenderTexture.ReleaseTemporary(temporary2);
			}
			Graphics.SetRenderTarget(m_shadowTexture);
			if (renderTexture != m_shadowTexture)
			{
				Graphics.Blit(renderTexture, m_downsampleShader, 2);
				if (m_mipLevel == 0)
				{
					RenderTexture.ReleaseTemporary(renderTexture);
				}
			}
			EraseShadowOnBoarder(m_textureWidth, m_textureHeight);
			if (0 >= m_mipLevel)
			{
				return;
			}
			BlurParam blurH = default(BlurParam);
			BlurParam blurV = default(BlurParam);
			if (0.1f < m_mipmapBlurSize)
			{
				float num5 = m_projector.aspectRatio * (float)m_textureHeight / (float)m_textureWidth;
				float num6 = m_mipmapBlurSize;
				float num7 = m_mipmapBlurSize;
				if (num5 < 1f)
				{
					num7 *= num5;
				}
				else
				{
					num6 /= num5;
				}
				if (m_singlePassMipmapBlur)
				{
					blurH = GetDownsampleBlurParam(2f + 2f * num6, m_blurFilter);
					blurV = GetDownsampleBlurParam(2f + 2f * num7, m_blurFilter);
					Vector4 vector = new Vector4(blurH.weight.x * blurV.weight.x, blurH.weight.x * blurV.weight.y, blurH.weight.y * blurV.weight.x, blurH.weight.y * blurV.weight.y);
					float num8 = 0.25f / (vector.x + vector.y + vector.z + vector.w);
					vector.x = Mathf.Round(255f * num8 * vector.x) / 255f;
					vector.y = Mathf.Round(255f * num8 * vector.y) / 255f;
					vector.z = Mathf.Round(255f * num8 * vector.z) / 255f;
					vector.w = 0.25f - vector.x - vector.y - vector.z;
					m_downsampleShader.SetVector(s_downSampleBlurWeightParamID, vector);
				}
				else
				{
					blurH = GetBlurParam(num6, m_blurFilter);
					blurV = GetBlurParam(num7, m_blurFilter);
					blurH.tap -= 3;
					blurV.tap = blurV.tap - 3 + 1;
					m_blurShader.SetVector(s_blurOffsetHParamID, blurH.offset);
					m_blurShader.SetVector(s_blurOffsetVParamID, blurV.offset);
					m_blurShader.SetVector(s_blurWeightHParamID, blurH.weight);
					m_blurShader.SetVector(s_blurWeightVParamID, blurV.weight);
				}
			}
			int num9 = m_textureWidth >> 1;
			int num10 = m_textureHeight >> 1;
			RenderTexture temporary3 = RenderTexture.GetTemporary(num9, num10, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
			temporary3.filterMode = FilterMode.Bilinear;
			bool flag = m_singlePassMipmapBlur && 0.1f < m_mipmapBlurSize;
			if (flag)
			{
				SetDownsampleBlurOffsetParams(blurH, blurV, num9, num10);
			}
			if (renderTexture == m_shadowTexture)
			{
				if (flag)
				{
					Graphics.Blit(renderTexture, temporary3, m_downsampleShader, 5);
				}
				else
				{
					Graphics.Blit(renderTexture, temporary3, m_copyMipmapShader, 1);
				}
			}
			else
			{
				Graphics.Blit(renderTexture, temporary3, m_downsampleShader, flag ? 4 : 0);
				RenderTexture.ReleaseTemporary(renderTexture);
			}
			renderTexture = temporary3;
			int num11 = 0;
			float value = 1f;
			while (true)
			{
				if (0.1f < m_mipmapBlurSize && !m_singlePassMipmapBlur)
				{
					temporary3 = RenderTexture.GetTemporary(num9, num10, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
					temporary3.filterMode = FilterMode.Bilinear;
					temporary3.wrapMode = TextureWrapMode.Clamp;
					renderTexture.wrapMode = TextureWrapMode.Clamp;
					Graphics.Blit(renderTexture, temporary3, m_blurShader, blurH.tap);
					renderTexture.DiscardContents();
					Graphics.Blit(temporary3, renderTexture, m_blurShader, blurV.tap);
					RenderTexture.ReleaseTemporary(temporary3);
				}
				if (m_mipmapFalloff == MipmapFalloff.Linear)
				{
					value = (float)(m_mipLevel - num11) / ((float)m_mipLevel + 1f);
				}
				else if (m_mipmapFalloff == MipmapFalloff.Custom && m_customMipmapFalloff != null && 0 < m_customMipmapFalloff.Length)
				{
					value = m_customMipmapFalloff[Mathf.Min(num11, m_customMipmapFalloff.Length - 1)];
				}
				m_copyMipmapShader.SetFloat(s_falloffParamID, value);
				m_copyMipmapShader.SetFloat(s_falloffParamID, value);
				m_shadowTexture.DiscardContents();
				num11++;
				Graphics.SetRenderTarget(m_shadowTexture, num11);
				Graphics.Blit(renderTexture, m_copyMipmapShader, 0);
				EraseShadowOnBoarder(num9, num10);
				num9 = Mathf.Max(1, num9 >> 1);
				num10 = Mathf.Max(1, num10 >> 1);
				if (num11 == m_mipLevel || num9 <= 4 || num10 <= 4)
				{
					break;
				}
				temporary3 = RenderTexture.GetTemporary(num9, num10, 0, m_shadowTexture.format, RenderTextureReadWrite.Linear);
				temporary3.filterMode = FilterMode.Bilinear;
				if (flag)
				{
					SetDownsampleBlurOffsetParams(blurH, blurV, num9, num10);
					Graphics.Blit(renderTexture, temporary3, m_downsampleShader, 4);
				}
				else
				{
					Graphics.Blit(renderTexture, temporary3, m_downsampleShader, 0);
				}
				RenderTexture.ReleaseTemporary(renderTexture);
				renderTexture = temporary3;
			}
			RenderTexture.ReleaseTemporary(renderTexture);
			while (1 <= num9 || 1 <= num10)
			{
				num11++;
				Graphics.SetRenderTarget(m_shadowTexture, num11);
				GL.Clear(false, true, new Color(1f, 1f, 1f, 0f));
				num9 >>= 1;
				num10 >>= 1;
			}
		}

		private void EraseShadowOnBoarder(int w, int h)
		{
			float value = 1f / (float)w;
			float value2 = 1f / (float)h;
			m_eraseShadowShader.SetFloat("xStep", value);
			m_eraseShadowShader.SetFloat("yStep", value2);
			Graphics.Blit(m_shadowTexture, m_eraseShadowShader, 0);
		}

		private void SetDownsampleBlurOffsetParams(BlurParam blurH, BlurParam blurV, int w, int h)
		{
			float num = 0.5f / (float)w;
			float num2 = 0.5f / (float)h;
			float num3 = num * blurH.offset.x;
			float num4 = num * blurH.offset.y;
			float num5 = num2 * blurV.offset.x;
			float num6 = num2 * blurV.offset.y;
			m_downsampleShader.SetVector(s_downSampleBlurOffset0ParamID, new Vector4(num3, num5, 0f - num3, 0f - num5));
			m_downsampleShader.SetVector(s_downSampleBlurOffset1ParamID, new Vector4(num3, num6, 0f - num3, 0f - num6));
			m_downsampleShader.SetVector(s_downSampleBlurOffset2ParamID, new Vector4(num4, num5, 0f - num4, 0f - num5));
			m_downsampleShader.SetVector(s_downSampleBlurOffset3ParamID, new Vector4(num4, num6, 0f - num4, 0f - num6));
		}
	}
}
