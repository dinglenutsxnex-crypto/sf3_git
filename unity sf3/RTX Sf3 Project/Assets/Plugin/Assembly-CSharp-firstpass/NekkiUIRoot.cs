using System;
using System.Collections.Generic;
using UnityEngine;

public class NekkiUIRoot : ExtentionBehaviour
{
	public delegate void OnResolutionChangedEventHandler(bool fullScreen);

	public delegate void OnUnmountEventHandler(string moduleName);

	private class ImageInfo
	{
		public string Name { get; private set; }

		public Texture2D Texture { get; private set; }

		public string AtlasName { get; private set; }

		public ImageInfo(string atlasName, Texture2D texture, string name)
		{
			Texture = texture;
			AtlasName = atlasName;
			Name = name;
		}
	}

	private enum PathType
	{
		Sprites = 0,
		Textures = 1
	}

	private readonly Dictionary<UIAnchor.Side, UIAnchor> _anchors = new Dictionary<UIAnchor.Side, UIAnchor>();

	[SerializeField]
	public Canvas canvas;

	private NekkiUIRootModules _modules;

	public static Camera Camera;

	public string[] MountOnStart;

	private static NekkiUIRoot _instance;

	public int ScreenWidth
	{
		get
		{
			return Mathf.RoundToInt(NGUITools.screenSize.x * UIRoot.GetPixelSizeAdjustment(base.gameObject));
		}
	}

	public int ScreenHeight
	{
		get
		{
			return Mathf.RoundToInt(NGUITools.screenSize.y * UIRoot.GetPixelSizeAdjustment(base.gameObject));
		}
	}

	public static NekkiUIRoot Instance
	{
		get
		{
			if (!_instance)
			{
				_instance = UnityEngine.Object.FindObjectOfType<NekkiUIRoot>();
				if ((bool)_instance)
				{
					_instance.Init();
				}
				else
				{
					Debug.LogWarning("NekkiUIRoot was not found");
				}
			}
			return _instance;
		}
	}

	public event OnResolutionChangedEventHandler OnResolutionChanged;

	public event OnUnmountEventHandler OnUnmount;

	private void FireOnResolutionChanged(bool fullScreen)
	{
		OnResolutionChangedEventHandler onResolutionChanged = this.OnResolutionChanged;
		if (onResolutionChanged != null)
		{
			onResolutionChanged(fullScreen);
		}
	}

	private void FireOnUnmount(string moduleName)
	{
		OnUnmountEventHandler onUnmount = this.OnUnmount;
		if (onUnmount != null)
		{
			onUnmount(moduleName);
		}
	}

	public bool ContainsAnchor(UIAnchor.Side side)
	{
		return _anchors.ContainsKey(side);
	}

	public void RecalcAnchors()
	{
		_anchors.Clear();
		Camera camera = (Camera = GetComponentInChildren<Camera>());
		Transform parent = camera.transform;
		UIAnchor[] componentsInChildren = GetComponentsInChildren<UIAnchor>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!_anchors.ContainsKey(componentsInChildren[i].side))
			{
				_anchors.Add(componentsInChildren[i].side, componentsInChildren[i]);
			}
			componentsInChildren[i].gameObject.name = string.Format("anchor_{0}", componentsInChildren[i].side);
		}
		for (int j = 0; j < Enum.GetValues(typeof(UIAnchor.Side)).Length; j++)
		{
			if (!_anchors.ContainsKey((UIAnchor.Side)j))
			{
				UIAnchor component = new GameObject(string.Format("anchor_{0}", (UIAnchor.Side)j), typeof(UIAnchor)).GetComponent<UIAnchor>();
				component.side = (UIAnchor.Side)j;
				component.transform.parent = parent;
				component.gameObject.layer = base.gameObject.layer;
				component.uiCamera = camera;
				component.runOnlyOnce = SystemProperties.IsMobilePlatform;
				component.transform.localEulerAngles = Vector3.zero;
				component.transform.localScale = Vector3.one;
			}
		}
	}

	public Transform GetAnchor(UIAnchor.Side side)
	{
		if (_anchors.ContainsKey(side))
		{
			return _anchors[side].transform;
		}
		return null;
	}

	internal void Init()
	{
		_modules = GetComponent<NekkiUIRootModules>();
		_modules.Clear();
		RecalcAnchors();
		_instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Awake()
	{
		if (!_instance)
		{
			Init();
		}
		if (MountOnStart != null)
		{
			for (int i = 0; i < MountOnStart.Length; i++)
			{
				_modules.MountNativeModule(MountOnStart[i]);
			}
		}
	}

	internal void Start()
	{
	}

	internal void Update()
	{
	}
}
