using System;
using System.Collections;
using System.Collections.Generic;
using Nekki;
using UnityEngine;

[RequireComponent(typeof(NekkiUIModule))]
public abstract class NekkiUIDialog : MonoBehaviour
{
	private NekkiUIModule _module;

	[NonSerialized]
	public int ContentPadLeft;

	[NonSerialized]
	public int ContentPadRight;

	[NonSerialized]
	public int ContentPadBottom;

	[NonSerialized]
	public int ContentPadTop;

	[NonSerialized]
	public bool CloseOnBack = true;

	protected int dialogPanelStartDepth = 2000;

	private bool _modal = true;

	protected UIPanel dialogPanel;

	protected string dialogPrefabPath;

	protected GameObject dlg;

	protected NekkiUIDialogInfo dlgScript;

	protected GameObject content;

	protected EventDelegate _destroyDelegate;

	public NekkiUIModule Module
	{
		get
		{
			if (_module == null)
			{
				_module = GetComponent<NekkiUIModule>();
			}
			return _module;
		}
	}

	public virtual bool Modal
	{
		get
		{
			return _modal;
		}
		set
		{
			_modal = value;
			if (dlgScript.ModalWidget != null)
			{
				dlgScript.ModalWidget.GetComponent<Collider>().enabled = _modal;
			}
			if (dlgScript.ModalBg != null)
			{
				dlgScript.ModalBg.gameObject.SetActive(_modal);
			}
		}
	}

	public EventDelegate DestroyDelegate
	{
		get
		{
			return _destroyDelegate;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
	}

	protected abstract void Init();

	protected virtual void CreateDialog()
	{
		Init();
		dlgScript = NekkiUtils.AddChild<NekkiUIDialogInfo>(base.gameObject, dialogPrefabPath);
		dlg = dlgScript.gameObject;
		dialogPanel = GetComponent<UIPanel>();
		dialogPanel.depth = dialogPanelStartDepth;
		OnScreenResize();
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Combine(UICamera.onScreenResize, new UICamera.OnScreenResize(OnScreenResize));
		_destroyDelegate = new EventDelegate(DestroyDlg);
		StartCoroutine(RefreshNextFrame());
	}

	protected virtual void OnScreenResize()
	{
		int screenWidth = NekkiUIRoot.Instance.ScreenWidth;
		int screenHeight = NekkiUIRoot.Instance.ScreenHeight;
		if (dlgScript.ModalWidget != null)
		{
			dlgScript.ModalWidget.width = screenWidth;
			dlgScript.ModalWidget.height = screenHeight;
		}
		if (dlgScript.ModalBg != null)
		{
			dlgScript.ModalBg.width = screenWidth;
			dlgScript.ModalBg.height = screenHeight;
		}
	}

	protected IEnumerator RefreshNextFrame()
	{
		yield return null;
		Refresh();
	}

	protected virtual void Refresh()
	{
		dialogPanel.depth = 0;
		UpdateSize();
		NekkiUtils.BringForward(base.gameObject);
		OnRefresh();
	}

	protected virtual void OnRefresh()
	{
	}

	public virtual void UpdateSize()
	{
	}

	public virtual void SetContentPad(int left, int right, int bottom, int top)
	{
		ContentPadLeft = left;
		ContentPadRight = right;
		ContentPadBottom = bottom;
		ContentPadTop = top;
	}

	protected virtual GameObject AddContent(string prefabPath)
	{
		content = NekkiUtils.AddChild(dlgScript.ContentPanel.gameObject, prefabPath);
		return content;
	}

	protected virtual T AddContent<T>(string prefabPath) where T : Component
	{
		return AddContent(prefabPath).GetComponent<T>();
	}

	private void DestroyDlg()
	{
		DestroyDialog();
	}

	public virtual void DestroyDialog(GameObject go = null)
	{
		DestroyDialogNow();
	}

	public virtual void DestroyDialogNow(bool simpleDestroy = false)
	{
		NekkiUIRootModules.Instance.UnmountModule(Module);
	}

	protected virtual void Update()
	{
		if (CloseOnBack && Input.GetKeyDown(KeyCode.Escape))
		{
			List<NekkiUIDialog> modulesWithComponent = NekkiUIRootModules.Instance.GetModulesWithComponent<NekkiUIDialog>();
			if (modulesWithComponent[modulesWithComponent.Count - 1] == this)
			{
				DestroyDialogNextFrame();
			}
		}
	}

	protected IEnumerator DestroyDialogNextFrameCoroutine()
	{
		yield return null;
		DestroyDialog();
	}

	public virtual void DestroyDialogNextFrame()
	{
		CloseOnBack = false;
		StartCoroutine(DestroyDialogNextFrameCoroutine());
	}

	protected virtual void OnDestroy()
	{
		UICamera.onScreenResize = (UICamera.OnScreenResize)Delegate.Remove(UICamera.onScreenResize, new UICamera.OnScreenResize(OnScreenResize));
	}
}
