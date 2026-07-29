using UnityEngine;

[RequireComponent(typeof(NekkiUIElements))]
public class NekkiUIModule : ExtentionBehaviour
{
	private NekkiUIElements _elements;

	[HideInInspector]
	public Vector3 Position;

	[HideInInspector]
	public Vector3 Scale;

	[HideInInspector]
	public Vector3 Rotation;

	public string ModuleName;

	public UIAnchor.Side Side;

	[SerializeField]
	private bool _isChildModule;

	private UISprite[] _sprites;

	private UITexture[] _textures;

	public NekkiUIElements Elements
	{
		get
		{
			if (!_elements)
			{
				_elements = GetComponent<NekkiUIElements>();
			}
			return _elements;
		}
	}

	public int Layer { get; set; }

	public bool isChildModule
	{
		get
		{
			return _isChildModule;
		}
	}

	public bool isNative
	{
		get
		{
			return GetComponent<RectTransform>() != null;
		}
	}

	public UISprite[] Sprites
	{
		get
		{
			UISprite[] array = _sprites;
			if (array == null)
			{
				UISprite[] obj = GetComponentsInChildren<UISprite>() ?? new UISprite[0];
				UISprite[] array2 = obj;
				_sprites = obj;
				array = array2;
			}
			return array;
		}
	}

	public UITexture[] Textures
	{
		get
		{
			UITexture[] array = _textures;
			if (array == null)
			{
				UITexture[] obj = GetComponentsInChildren<UITexture>() ?? new UITexture[0];
				UITexture[] array2 = obj;
				_textures = obj;
				array = array2;
			}
			return array;
		}
	}

	public virtual void Setup(object settings)
	{
	}

	public virtual void UpdateModule()
	{
	}

	public override string ToString()
	{
		return string.Format("Modul [{0}] ; ChildModule [{1}] ; Side [{2}]", ModuleName, isChildModule, Side);
	}
}
