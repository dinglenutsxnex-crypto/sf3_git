using UnityEngine;

namespace Nekki.StandardAssets
{
	[AddComponentMenu("Image Effects/Blur/Blur (Optimized)")]
	public class NekkiBlurOptimized : BlurOptimized
	{
		public void BlurTexture(UITexture textureToBlur)
		{
			Texture mainTexture = textureToBlur.mainTexture;
			RenderTexture temporary = RenderTexture.GetTemporary(mainTexture.width, mainTexture.height, 0);
			RenderTexture temporary2 = RenderTexture.GetTemporary(mainTexture.width, mainTexture.height, 0);
			Graphics.Blit(mainTexture, temporary);
			OnRenderImage(temporary, temporary2);
			textureToBlur.mainTexture = temporary2;
		}
	}
}
