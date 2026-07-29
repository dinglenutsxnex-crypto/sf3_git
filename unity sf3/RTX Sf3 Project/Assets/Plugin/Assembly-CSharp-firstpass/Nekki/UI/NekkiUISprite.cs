using System;
using UnityEngine;

namespace Nekki.UI
{
	public class NekkiUISprite : UISprite
	{
		private static Texture2D _noImageTexture;

		[HideInInspector]
		[SerializeField]
		private Shader mShader;

		[SerializeField]
		protected UIAtlas Low;

		[SerializeField]
		protected UIAtlas High;

		[NonSerialized]
		private int mPMA = -1;

		private UITexture noImageTexture;

		private Texture2D getNoImageTexture
		{
			get
			{
				if (_noImageTexture == null)
				{
					_noImageTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
					_noImageTexture.SetPixel(1, 1, Color.red);
					_noImageTexture.Apply();
				}
				return _noImageTexture;
			}
		}

		public override Shader shader
		{
			get
			{
				if (mShader != null)
				{
					return mShader;
				}
				return base.shader;
			}
			set
			{
				if (mShader != value)
				{
					RemoveFromPanel();
					mShader = value;
					mPMA = -1;
					MarkAsChanged();
				}
			}
		}

		public override bool premultipliedAlpha
		{
			get
			{
				if (mShader != null)
				{
					if (mPMA == -1)
					{
						mPMA = (mShader.name.Contains("Premultiplied") ? 1 : 0);
					}
					return mPMA == 1;
				}
				return base.premultipliedAlpha;
			}
		}

		public override string spriteName
		{
			get
			{
				return mSpriteName;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					SetNoImageTexture();
					if (!string.IsNullOrEmpty(mSpriteName))
					{
						mSpriteName = string.Empty;
						mSprite = null;
						mChanged = true;
						mSpriteSet = false;
					}
				}
				else if (mSpriteName != value)
				{
					CheckAtlasOnSprite(value);
					mSpriteName = value;
					mSprite = null;
					mChanged = true;
					mSpriteSet = false;
				}
			}
		}

		public void SetGrayscale(bool value)
		{
			shader = ((!value) ? null : Shader.Find("Unlit/Transparent Grayscale Colored"));
		}

		public void SetAtlas(UIAtlas high, UIAtlas low)
		{
			Low = low ?? Low;
			High = high ?? High;
		}

		internal new void Start()
		{
			base.Start();
			base.atlas = ((!SystemProperties.IsHighResolution) ? Low : High);
			CheckAtlasOnSprite(spriteName);
		}

		private void CheckAtlasOnSprite(string spriteName)
		{
			if (base.atlas == null)
			{
				SetNoImageTexture();
				return;
			}
			foreach (UISpriteData sprite in base.atlas.spriteList)
			{
				if (spriteName == sprite.name)
				{
					RemoveNoImageTexture();
					return;
				}
			}
			SetNoImageTexture();
		}

		private void RemoveNoImageTexture()
		{
			if (noImageTexture != null)
			{
				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(noImageTexture);
				}
				else
				{
					UnityEngine.Object.DestroyImmediate(noImageTexture);
				}
			}
		}

		private void SetNoImageTexture()
		{
			if (noImageTexture == null)
			{
				noImageTexture = base.gameObject.AddComponent<UITexture>();
			}
			noImageTexture.depth = base.depth;
			noImageTexture.width = base.width;
			noImageTexture.height = base.height;
			noImageTexture.mainTexture = getNoImageTexture;
			noImageTexture.type = Type.Sliced;
			noImageTexture.MarkAsChanged();
		}
	}
}
