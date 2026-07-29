using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DOTweenUtils;
using Nekki.StandardAssets;
using UnityEngine;

public class ScreenTexture : MonoBehaviour
{
	public enum TextureOutputFilter
	{
		None = 0,
		Blur = 1
	}

	public enum TextureOutputCamera
	{
		Both = 0,
		Main = 1,
		Ui = 2
	}

	public Action OnScreenBlocked;

	public Action OnScreenUnblocked;

	private Action _onScreenTaken;

	[SerializeField]
	public UITexture _screenTexture;

	private UIPanel _panel;

	private Dictionary<UIPanel, int> _overlaysDepth;

	private bool _isInit;

	private NekkiBlurOptimized _blur;

	private NekkiBlurOptimized _blurUI;

	private TextureOutputFilter _currentTextureOutputFilter;

	private TextureOutputCamera _currentTextureOutputCamera;

	private readonly LayerableCommand _layerableBlockScreen = LayerableCommand.Create(typeof(ScreenTexture).Name);

	[SerializeField]
	private BoxCollider _blockCollider;

	[SerializeField]
	private float _screenFadeDuration = 0.5f;

	private Coroutine _clearCoroutine;

	private List<Camera> _camerasToEnable;

	private Tweener _fadeoutTween;

	private const int DEFAULT_PANEL_DEPTH = 9999;

	private List<string> _textureCallersNames = new List<string>();

	private List<string> _blockCallersNames = new List<string>();

	public bool MainCameraEnabled;

	public RenderTexture renderTexture;

	public static ScreenTexture Instance { get; private set; }

	public bool Active
	{
		get
		{
			return _screenTexture.enabled;
		}
	}

	private void Awake()
	{
		Instance = this;
		_panel = GetComponent<UIPanel>();
		MainCameraEnabled = true;
		_blockCollider.enabled = false;
	}

	private void Start()
	{
		if (!_isInit)
		{
			Init();
		}
	}

	private void Init()
	{
		FoundBlurComponents();
		_camerasToEnable = new List<Camera>();
		_currentTextureOutputFilter = TextureOutputFilter.None;
		_currentTextureOutputCamera = TextureOutputCamera.Both;
		_overlaysDepth = new Dictionary<UIPanel, int>();
		_isInit = true;
		_screenTexture.enabled = false;
	}

	private void FoundBlurComponents()
	{
		if (_blur == null)
		{
			_blur = Camera.main.GetComponent<NekkiBlurOptimized>();
		}
		if (_blurUI == null)
		{
			_blurUI = UICamera.mainCamera.GetComponent<NekkiBlurOptimized>();
		}
		if (_blur == null && _blurUI == null)
		{
			Debug.LogError(" No blur found. ");
		}
	}

	public void BlockScreen(string callersName)
	{
		if (!_blockCallersNames.Contains(callersName))
		{
			_blockCallersNames.Add(callersName);
			_layerableBlockScreen.AddLayer(delegate
			{
				_blockCollider.enabled = true;
				OnScreenBlocked.InvokeSafe();
			});
		}
	}

	public void UnBlockScreen(string callersName, float duration = 0.5f, Action callback = null)
	{
		if (!_blockCallersNames.Contains(callersName))
		{
			Debug.LogWarning("Can't remove " + callersName + " from list of block callers because it's not exist in this list");
			callback.InvokeSafe();
			return;
		}
		_blockCallersNames.Remove(callersName);
		if (_blockCallersNames.IsEmpty())
		{
			_layerableBlockScreen.RemoveLayer(delegate
			{
				_blockCollider.enabled = false;
				OnScreenUnblocked.InvokeSafe();
			});
		}
	}

	private void RemoveCallerName(string callerName)
	{
		if (_textureCallersNames.Count != 0 && !_textureCallersNames.Remove(callerName))
		{
			Debug.LogError("Can't remove caller's name from list. That because of wrong caller's name: " + callerName);
		}
	}

	public bool IsScreenBlocked()
	{
		return _layerableBlockScreen.IsAnyLayerActive;
	}

	public void SetTexture(string callersName, TextureOutputCamera tCamera, TextureOutputFilter tFilter, Action callback = null, int depth = 9999)
	{
		if (!_textureCallersNames.Contains(callersName))
		{
			_textureCallersNames.Add(callersName);
		}
		_panel.depth = depth;
		if (_clearCoroutine != null)
		{
			StopCoroutine(_clearCoroutine);
			_clearCoroutine = null;
		}
		if (_fadeoutTween != null)
		{
			_fadeoutTween.Kill();
			_fadeoutTween = null;
		}
		_screenTexture.alpha = 1f;
		TweenCallback tweenCallback = delegate
		{
			callback.InvokeSafe();
			MainCameraEnabled = false;
		};
		if (!_screenTexture.enabled)
		{
			_screenTexture.enabled = true;
			_currentTextureOutputFilter = tFilter;
			_currentTextureOutputCamera = tCamera;
			_onScreenTaken = (Action)Delegate.Combine(_onScreenTaken, (Action)delegate
			{
				DONgui.Fade(_screenTexture, 0f, 1f, _screenFadeDuration).SetUpdate(true).OnComplete(tweenCallback);
			});
			TakeScreenshot();
		}
		else if (_currentTextureOutputFilter != tFilter)
		{
			_currentTextureOutputFilter = tFilter;
			_currentTextureOutputCamera = tCamera;
			if (tFilter == TextureOutputFilter.Blur)
			{
				BlurScreenTexture();
				callback.InvokeSafe();
				return;
			}
			_onScreenTaken = (Action)Delegate.Combine(_onScreenTaken, (Action)delegate
			{
				DONgui.Fade(_screenTexture, 0f, 1f, _screenFadeDuration).SetUpdate(true).OnComplete(tweenCallback);
			});
			TakeScreenshot();
		}
		else
		{
			callback.InvokeSafe();
		}
	}

	public void SetTexture(string callersName, TextureOutputCamera tCamera, Action callback = null)
	{
		SetTexture(callersName, tCamera, TextureOutputFilter.None, callback);
	}

	[ContextMenu("Set texture")]
	public void SetTexture(string callersName, Action callback = null)
	{
		SetTexture(callersName, TextureOutputCamera.Both, TextureOutputFilter.None, callback);
	}

	private void TakeScreenshot()
	{
		if (_currentTextureOutputFilter == TextureOutputFilter.Blur)
		{
			SetBlurEnabling(true);
		}
		renderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 16);
		SetOtherCamerasEnabling(false);
		if (_currentTextureOutputCamera == TextureOutputCamera.Main || _currentTextureOutputCamera == TextureOutputCamera.Both)
		{
			RenderCameraToTexture(Camera.main, renderTexture);
		}
		if (_currentTextureOutputCamera == TextureOutputCamera.Ui || _currentTextureOutputCamera == TextureOutputCamera.Both)
		{
			RenderCameraToTexture(UICamera.mainCamera, renderTexture);
		}
		SetOtherCamerasEnabling(true);
		_screenTexture.mainTexture = renderTexture;
		_screenTexture.width = NekkiUIRoot.Instance.ScreenWidth;
		_screenTexture.height = NekkiUIRoot.Instance.ScreenHeight;
		_screenTexture.color = new Color(_screenTexture.color.r, _screenTexture.color.g, _screenTexture.color.b, 0f);
		if (_currentTextureOutputFilter == TextureOutputFilter.Blur)
		{
			SetBlurEnabling(false);
		}
		_onScreenTaken.InvokeSafe();
		_onScreenTaken = null;
	}

	private void RenderCameraToTexture(Camera cam, RenderTexture tex)
	{
		cam.targetTexture = tex;
		cam.Render();
		cam.targetTexture = null;
	}

	private void SetBlurEnabling(bool isEnable)
	{
		if (_blur == null || _blurUI == null)
		{
			FoundBlurComponents();
		}
		if (_blur != null)
		{
			_blur.enabled = isEnable && (_currentTextureOutputCamera == TextureOutputCamera.Both || _currentTextureOutputCamera == TextureOutputCamera.Main);
		}
		if (_blurUI != null)
		{
			_blurUI.enabled = isEnable && (_currentTextureOutputCamera == TextureOutputCamera.Both || _currentTextureOutputCamera == TextureOutputCamera.Ui);
		}
	}

	private void BlurScreenTexture()
	{
		if (_blur == null || _blurUI == null)
		{
			FoundBlurComponents();
		}
		if (_blur != null && (_currentTextureOutputCamera == TextureOutputCamera.Both || _currentTextureOutputCamera == TextureOutputCamera.Main))
		{
			_blur.BlurTexture(_screenTexture);
		}
		else if (_blurUI != null && (_currentTextureOutputCamera == TextureOutputCamera.Both || _currentTextureOutputCamera == TextureOutputCamera.Ui))
		{
			_blurUI.BlurTexture(_screenTexture);
		}
	}

	private void SetOtherCamerasEnabling(bool isEnable)
	{
		if (isEnable)
		{
			for (int i = 0; i < _camerasToEnable.Count; i++)
			{
				_camerasToEnable[i].gameObject.SetActive(true);
			}
			return;
		}
		_camerasToEnable.Clear();
		for (int j = 0; j < Camera.allCamerasCount; j++)
		{
			Camera camera = Camera.allCameras[j];
			if (camera.tag.Equals("disableOnScreenshot") && camera.gameObject.activeInHierarchy)
			{
				_camerasToEnable.Add(camera);
				camera.gameObject.SetActive(false);
			}
		}
	}

	public void Clear(string callersName, float duration = 0.5f, Action callBack = null)
	{
		MainCameraEnabled = true;
		RemoveCallerName(callersName);
		if (!_textureCallersNames.IsEmpty())
		{
			callBack.InvokeSafe();
			return;
		}
		_fadeoutTween = DONgui.Fade(_screenTexture, 0f, duration).OnComplete(delegate
		{
			_fadeoutTween = null;
			_clearCoroutine = StartCoroutine(ClearTexture());
			callBack.InvokeSafe();
		});
		_fadeoutTween.SetUpdate(true);
		_panel.depth = 9999;
	}

	private IEnumerator ClearTexture()
	{
		yield return new WaitForEndOfFrame();
		_clearCoroutine = null;
		GlobalLoad.Unload(_screenTexture.mainTexture);
		_screenTexture.enabled = false;
		_screenTexture.mainTexture = null;
	}

	public void AddOverlayPanel(UIPanel overlayPanel, int priority = 1)
	{
		if (!_overlaysDepth.ContainsKey(overlayPanel))
		{
			_overlaysDepth.Add(overlayPanel, overlayPanel.depth);
			overlayPanel.depth = _panel.depth + priority;
		}
	}

	public void AddOverlay(GameObject gameObject, int priority = 1)
	{
	}

	public void RemoveOverlayPanel(UIPanel overlayPanel)
	{
		if (_overlaysDepth.ContainsKey(overlayPanel))
		{
			overlayPanel.depth = _overlaysDepth[overlayPanel];
			_overlaysDepth.Remove(overlayPanel);
		}
	}
}
