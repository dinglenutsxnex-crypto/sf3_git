using System;
using System.Collections.Generic;
using UnityEngine;

public class NekkiCanvasRoot : ExtentionBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Camera canvasCamera;

	[SerializeField]
	protected NekkiUIRootModules modules;

	private Dictionary<string, NekkiUIModule> _mounted;

	private RectTransform _rectTransform;

	public Canvas Canvas
	{
		get
		{
			return canvas;
		}
	}

	public RectTransform RectTransform
	{
		get
		{
			return _rectTransform;
		}
	}

	public static NekkiCanvasRoot instance { get; protected set; }

	public event Action<string> OnUnmount;

	private void Init()
	{
		_mounted = new Dictionary<string, NekkiUIModule>();
		modules.PerformMountOnStart();
	}

	private void Awake()
	{
		instance = this;
		_rectTransform = canvas.GetComponent<RectTransform>();
		StaticObjectsManager.AddObject(base.gameObject, false);
		Init();
	}

	public NekkiUIModule Mount(string moduleName)
	{
		if (_mounted.ContainsKey(moduleName))
		{
			Debug.LogError(string.Format("Module {0} already mounted.", moduleName));
			return null;
		}
		NekkiUIModule module = modules.GetModule(moduleName);
		if (module == null || module.isChildModule)
		{
			Debug.LogWarning(moduleName + " is null ");
			return null;
		}
		if (!module || module.isChildModule)
		{
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(module.gameObject);
		gameObject.transform.SetParent(canvas.transform, false);
		NekkiUIModule component = gameObject.GetComponent<NekkiUIModule>();
		_mounted.Add(moduleName, component);
		return component;
	}

	public void Unmount(string moduleName)
	{
		if (!_mounted.ContainsKey(moduleName))
		{
			Debug.LogError(string.Format("Module {0} not found.", moduleName));
		}
		else
		{
			Unmount(_mounted[moduleName]);
		}
	}

	public void Unmount(NekkiUIModule module)
	{
		_mounted.Remove(module.ModuleName);
		GlobalLoad.Unload(module.gameObject);
		this.OnUnmount.InvokeSafe(module.ModuleName);
	}

	[ContextMenu("MountTest")]
	public void MountTest()
	{
		Mount("SystemAlert");
	}

	[ContextMenu("UnMountTest")]
	public void UnMountTest()
	{
		Unmount("SystemAlert");
	}

	public Camera GetCanvasCamera()
	{
		return canvasCamera;
	}
}
