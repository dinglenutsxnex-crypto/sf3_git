using DG.Tweening;
using DG.Tweening.Core.Surrogates;
using UnityEngine;

namespace DOTweenUtils
{
	public static class DONgui
	{
		public static Tweener Fade<T>(T widget, float endValue, float duration) where T : UIRect
		{
			return DOTween.To(() => widget.alpha, delegate(float x)
			{
				widget.alpha = x;
			}, endValue, duration);
		}

		public static Tweener Fade<T>(T widget, float startValue, float endValue, float duration) where T : UIRect
		{
			return DOTween.To(delegate(float x)
			{
				widget.alpha = x;
			}, startValue, endValue, duration);
		}

		public static Tweener ColorTween<T>(T widget, Color endValue, float duration) where T : UIWidget
		{
			return DOTween.To(() => widget.color, delegate(ColorWrapper x)
			{
				widget.color = x.value;
			}, endValue, duration);
		}

		public static Tweener DOTiling(this UITexture texture, Vector2 endValue, string textureName, float duration)
		{
			return DOTween.To(delegate
			{
				Material uITextureMaterial2 = GetUITextureMaterial(texture);
				return uITextureMaterial2.GetTextureScale(textureName);
			}, delegate(Vector2Wrapper x)
			{
				Material uITextureMaterial = GetUITextureMaterial(texture);
				uITextureMaterial.SetTextureScale(textureName, x);
			}, endValue, duration);
		}

		public static Tweener DOOffset(this UITexture texture, Vector2 endValue, string textureName, float duration)
		{
			return DOTween.To(delegate
			{
				Material uITextureMaterial2 = GetUITextureMaterial(texture);
				return uITextureMaterial2.GetTextureOffset(textureName);
			}, delegate(Vector2Wrapper x)
			{
				Material uITextureMaterial = GetUITextureMaterial(texture);
				uITextureMaterial.SetTextureOffset(textureName, x);
			}, endValue, duration);
		}

		public static Tweener DOFloat(this UITexture texture, float endValue, string param, float duration)
		{
			return DOTween.To(delegate
			{
				Material uITextureMaterial2 = GetUITextureMaterial(texture);
				return uITextureMaterial2.GetFloat(param);
			}, delegate(float x)
			{
				Material uITextureMaterial = GetUITextureMaterial(texture);
				uITextureMaterial.SetFloat(param, x);
			}, endValue, duration);
		}

		private static Material GetUITextureMaterial(UITexture texture)
		{
			if (texture.drawCall != null)
			{
				return texture.drawCall.dynamicMaterial;
			}
			return texture.material;
		}

		public static Tweener DOProgress(this UIProgressBar progressBar, float endValue, float duration)
		{
			return DOTween.To(() => progressBar.value, delegate(float x)
			{
				progressBar.value = x;
			}, endValue, duration);
		}

		public static Tweener DOProgress(this UIProgressBar progressBar, float startValue, float endValue, float duration)
		{
			return DOTween.To(delegate(float x)
			{
				progressBar.value = x;
			}, startValue, endValue, duration);
		}

		public static Sequence TwoWidgetBlinkSequence(UIWidget firstWidget, UIWidget secondWidget, float duration, float showTime)
		{
			Sequence sequence = DOTween.Sequence();
			TweenCallback action = delegate
			{
				firstWidget.alpha = 1f;
				secondWidget.alpha = 0f;
			};
			sequence.OnStart(action);
			sequence.OnRewind(action);
			sequence.AppendInterval(showTime);
			sequence.Append(Fade(firstWidget, 1f, 0f, duration).SetEase(Ease.OutExpo));
			sequence.Join(Fade(secondWidget, 0f, 1f, duration).SetEase(Ease.InExpo));
			sequence.AppendInterval(showTime);
			sequence.Append(Fade(secondWidget, 1f, 0f, duration).SetEase(Ease.OutExpo));
			sequence.Join(Fade(firstWidget, 0f, 1f, duration).SetEase(Ease.InExpo));
			sequence.SetLoops(-1);
			return sequence;
		}

		public static Sequence GetProgressBarSequence(UIProgressBar progressBar, float endValue, int repeatCount, float progressBarDuration, TweenCallback levelupCallback = null)
		{
			Sequence sequence = DOTween.Sequence();
			if (repeatCount > 0)
			{
				float duration = progressBarDuration * (1f - progressBar.value);
				float duration2 = progressBarDuration * endValue;
				sequence.Append(progressBar.DOProgress(1f, duration));
				if (levelupCallback != null)
				{
					sequence.AppendCallback(levelupCallback);
				}
				sequence.Append(progressBar.DOProgress(0f, endValue, duration2));
			}
			else
			{
				sequence.Append(progressBar.DOProgress(endValue, progressBarDuration));
			}
			return sequence;
		}
	}
}
