using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	[ExecuteInEditMode]
	[AddComponentMenu("Image Effects/Blur/Motion Blur (Color Accumulation)")]
	[RequireComponent(typeof(Camera))]
	public class MotionBlur : ImageEffectBase
	{
		[Range(0f, 0.92f)]
		public float blurAmount = 0.8f;

		private RenderTexture accumTexture;

		protected override void Start()
		{
			base.Start();
			string text = SystemInfo.deviceModel.ToLower();
			if (text.Contains("iphone8") || text.Contains("iphone9"))
			{
				base.enabled = false;
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			Object.DestroyImmediate(accumTexture);
		}

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (accumTexture == null || accumTexture.width != source.width || accumTexture.height != source.height)
			{
				Object.DestroyImmediate(accumTexture);
				accumTexture = new RenderTexture(source.width, source.height, 0);
				accumTexture.hideFlags = HideFlags.HideAndDontSave;
				Graphics.Blit(source, accumTexture);
			}
			blurAmount = Mathf.Clamp(blurAmount, 0f, 0.92f);
			base.material.SetTexture("_MainTex", accumTexture);
			base.material.SetFloat("_AccumOrig", 1f - blurAmount);
			accumTexture.MarkRestoreExpected();
			Graphics.Blit(source, accumTexture, base.material);
			Graphics.Blit(accumTexture, destination);
		}
	}
}
