using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace DynamicShadowProjector
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(ShadowTextureRenderer))]
	public class DrawTargetObject : MonoBehaviour
	{
		[Serializable]
		public struct ReplaceShader
		{
			public string renderType;

			public Shader shader;
		}

		public enum TextureAlignment
		{
			None = 0,
			TargetAxisX = 1,
			TargetAxisY = 2,
			TargetAxisZ = 3
		}

		public enum UpdateFunction
		{
			OnPreCull = 0,
			LateUpdate = 1,
			UpdateTransform = 2
		}

		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private LayerMask m_layerMask = -1;

		[SerializeField]
		private TextureAlignment m_textureAlignment;

		[SerializeField]
		private UpdateFunction m_updateFunction;

		[SerializeField]
		private Material m_shadowShader;

		[SerializeField]
		private ReplaceShader[] m_replacementShaders;

		[SerializeField]
		private bool m_renderChildren = true;

		[SerializeField]
		private bool m_followTarget = true;

		private bool m_isCommandBufferDirty;

		private CommandBuffer m_commandBuffer;

		private ShadowTextureRenderer m_shadowRenderer;

		private Vector3 m_localTargetPosition;

		private Dictionary<Material, Material> m_replacedMaterialCache;

		public Transform target
		{
			get
			{
				return m_target;
			}
			set
			{
				if (m_target != value)
				{
					m_target = value;
					SetCommandBufferDirty();
				}
			}
		}

		public bool renderChildren
		{
			get
			{
				return m_renderChildren;
			}
			set
			{
				if (m_renderChildren != value)
				{
					m_renderChildren = value;
					SetCommandBufferDirty();
				}
			}
		}

		public LayerMask layerMask
		{
			get
			{
				return m_layerMask;
			}
			set
			{
				if ((int)m_layerMask != (int)value)
				{
					m_layerMask = value;
					if (m_renderChildren)
					{
						SetCommandBufferDirty();
					}
				}
			}
		}

		public TextureAlignment textureAlignment
		{
			get
			{
				return m_textureAlignment;
			}
			set
			{
				m_textureAlignment = value;
			}
		}

		public UpdateFunction updateFunction
		{
			get
			{
				return m_updateFunction;
			}
			set
			{
				m_updateFunction = value;
			}
		}

		public bool followTarget
		{
			get
			{
				return m_followTarget;
			}
			set
			{
				m_followTarget = value;
			}
		}

		public Material shadowShader
		{
			get
			{
				return m_shadowShader;
			}
			set
			{
				if (m_shadowShader != value)
				{
					m_shadowShader = value;
					SetCommandBufferDirty();
				}
			}
		}

		public ReplaceShader[] replacementShaders
		{
			get
			{
				return m_replacementShaders;
			}
			set
			{
				m_replacementShaders = value;
				SetCommandBufferDirty();
			}
		}

		public void SetCommandBufferDirty()
		{
			m_isCommandBufferDirty = true;
		}

		public void UpdateCommandBuffer()
		{
			if (m_target == null)
			{
				return;
			}
			m_commandBuffer.Clear();
			int num = ((m_replacementShaders != null) ? m_replacementShaders.Length : 0);
			if (m_renderChildren)
			{
				Renderer[] componentsInChildren = m_target.gameObject.GetComponentsInChildren<Renderer>();
				for (int i = -1; i < num; i++)
				{
					Renderer[] array = componentsInChildren;
					foreach (Renderer renderer in array)
					{
						if (renderer.enabled && ((int)m_layerMask & (1 << renderer.gameObject.layer)) != 0)
						{
							AddDrawCommand(renderer, i);
						}
					}
				}
			}
			else
			{
				Renderer component = m_target.gameObject.GetComponent<Renderer>();
				if (component != null)
				{
					for (int k = -1; k < num; k++)
					{
						AddDrawCommand(component, k);
					}
				}
				else if (Debug.isDebugBuild || Application.isEditor)
				{
					Debug.LogError("The target object does not have a Renderer component!", m_target);
				}
			}
			m_isCommandBufferDirty = false;
		}

		public void UpdateMaterial(Material mat)
		{
			Material value;
			if (m_replacedMaterialCache != null && m_replacedMaterialCache.TryGetValue(mat, out value))
			{
				value.CopyPropertiesFromMaterial(mat);
			}
		}

		public void UpdateTransform()
		{
			if (m_textureAlignment != 0)
			{
				Vector3 worldUp;
				switch (m_textureAlignment)
				{
				case TextureAlignment.TargetAxisX:
					worldUp = m_target.right;
					break;
				case TextureAlignment.TargetAxisZ:
					worldUp = m_target.forward;
					break;
				default:
					worldUp = m_target.up;
					break;
				}
				base.transform.LookAt(base.transform.position + base.transform.forward, worldUp);
			}
			if (m_followTarget)
			{
				Vector3 vector = base.transform.TransformPoint(m_localTargetPosition);
				base.transform.position += m_target.position - vector;
			}
		}

		private void Awake()
		{
			m_shadowRenderer = GetComponent<ShadowTextureRenderer>();
			if (m_target != null)
			{
				m_localTargetPosition = base.transform.InverseTransformPoint(m_target.position);
			}
			CreateCommandBuffer();
		}

		private void OnValidate()
		{
			if (m_commandBuffer != null)
			{
				UpdateCommandBuffer();
			}
		}

		private void OnEnable()
		{
			if (m_commandBuffer == null)
			{
				CreateCommandBuffer();
			}
			else if (m_shadowRenderer != null && m_shadowRenderer.isProjectorVisible)
			{
				m_shadowRenderer.AddCommandBuffer(m_commandBuffer);
			}
		}

		private void OnDisable()
		{
			if (m_shadowRenderer != null && m_commandBuffer != null)
			{
				m_shadowRenderer.RemoveCommandBuffer(m_commandBuffer);
			}
		}

		private void OnDestroy()
		{
			if (m_commandBuffer != null)
			{
				m_commandBuffer.Dispose();
				m_commandBuffer = null;
			}
			if (m_replacedMaterialCache == null)
			{
				return;
			}
			foreach (KeyValuePair<Material, Material> item in m_replacedMaterialCache)
			{
				UnityEngine.Object.DestroyImmediate(item.Value);
			}
			m_replacedMaterialCache.Clear();
		}

		private void LateUpdate()
		{
			if (m_updateFunction == UpdateFunction.LateUpdate)
			{
				UpdateTransform();
			}
		}

		private void OnPreCull()
		{
			if (m_isCommandBufferDirty)
			{
				UpdateCommandBuffer();
			}
			if (m_updateFunction == UpdateFunction.OnPreCull)
			{
				UpdateTransform();
			}
		}

		private void OnVisibilityChanged(bool isVisible)
		{
			if (isVisible)
			{
				m_shadowRenderer.AddCommandBuffer(m_commandBuffer);
			}
			else
			{
				m_shadowRenderer.RemoveCommandBuffer(m_commandBuffer);
			}
		}

		private void CreateCommandBuffer()
		{
			m_commandBuffer = new CommandBuffer();
			if (m_shadowRenderer.isProjectorVisible)
			{
				m_shadowRenderer.AddCommandBuffer(m_commandBuffer);
			}
			m_isCommandBufferDirty = true;
		}

		private void AddDrawCommand(Renderer renderer, int renderTypeIndex)
		{
			Material[] sharedMaterials = renderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Material material = sharedMaterials[i];
				string text = material.GetTag("RenderType", false);
				if (material.shader.name == "Standard" && (material.IsKeywordEnabled("_ALPHABLEND_ON") || material.IsKeywordEnabled("_ALPHATEST_ON") || material.IsKeywordEnabled("_ALPHAPREMULTIPLY_ON")))
				{
					text = "Transparent";
				}
				int num = -1;
				if (m_replacementShaders != null && !string.IsNullOrEmpty(text))
				{
					for (int j = 0; j < m_replacementShaders.Length; j++)
					{
						if (!(text == m_replacementShaders[j].renderType))
						{
							continue;
						}
						num = j;
						Shader shader = m_replacementShaders[j].shader;
						if (renderTypeIndex == j && shader != null)
						{
							if (m_replacedMaterialCache == null)
							{
								m_replacedMaterialCache = new Dictionary<Material, Material>();
							}
							Material value;
							if (!m_replacedMaterialCache.TryGetValue(material, out value))
							{
								value = new Material(material);
								value.shader = shader;
								value.hideFlags = HideFlags.HideAndDontSave;
								m_replacedMaterialCache.Add(material, value);
							}
							else
							{
								value.CopyPropertiesFromMaterial(material);
								value.shader = shader;
							}
							m_commandBuffer.DrawRenderer(renderer, value, i);
						}
						break;
					}
				}
				if (num == -1 && renderTypeIndex == -1)
				{
					m_commandBuffer.DrawRenderer(renderer, m_shadowShader, i);
				}
			}
		}
	}
}
