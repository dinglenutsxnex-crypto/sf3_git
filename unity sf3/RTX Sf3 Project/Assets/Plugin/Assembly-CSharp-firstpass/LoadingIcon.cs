using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class LoadingIcon : NekkiUIModule
{
	private static LoadingIcon _instance;

	public Action OnEnableLoadingScreen;

	[SerializeField]
	private Image _loadingImage;

	[SerializeField]
	private Image _bgImage;

	[SerializeField]
	private LocalizationText _loadingMessage;

	[SerializeField]
	private float _fullRotationTime;

	private Sequence _imageRotating;

	private List<string> _loadingMessages = new List<string>();

	public static LoadingIcon Instance
	{
		get
		{
			if (_instance == null)
			{
				NekkiUIRootModules.Instance.MountNativeModule("LoadingIcon");
			}
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		InitImageRotatingSequence();
	}

	private void InitImageRotatingSequence()
	{
		_imageRotating = DOTween.Sequence();
		_loadingImage.fillAmount = 0f;
		TweenerCore<float, float, FloatOptions> t = DOTween.To(() => _loadingImage.fillAmount, delegate(float x)
		{
			_loadingImage.fillAmount = x;
		}, 1f, _fullRotationTime).SetEase(Ease.Linear);
		TweenerCore<float, float, FloatOptions> t2 = DOTween.To(() => _loadingImage.fillAmount, delegate(float x)
		{
			_loadingImage.fillAmount = x;
		}, 0f, _fullRotationTime).SetEase(Ease.Linear);
		_imageRotating.Append(t);
		_imageRotating.Append(t2);
		_imageRotating.SetUpdate(true);
		_imageRotating.SetLoops(-1);
	}

	public void EnableLoadingScreen(string messageAlias = null)
	{
		_loadingMessages.Add(messageAlias);
		ScreenTexture.Instance.SetTexture(base.name, ScreenTexture.TextureOutputCamera.Both, ScreenTexture.TextureOutputFilter.Blur, delegate
		{
			if (_loadingMessages.Contains(messageAlias))
			{
				EnableLoadingImage();
				if (messageAlias != null)
				{
					_loadingMessage.SetAlias(messageAlias);
					_loadingMessage.gameObject.SetActive(true);
				}
			}
		});
		OnEnableLoadingScreen.InvokeSafe();
	}

	public void DisableLoadingScreen(float duration = 0.5f, Action callBack = null, string aliasToRemove = null)
	{
		RemoveAlias(aliasToRemove);
		if (_loadingMessages.IsEmpty())
		{
			DisableLoadingImage();
			_loadingMessage.gameObject.SetActive(false);
			ScreenTexture.Instance.Clear(base.name, duration, callBack);
			return;
		}
		string text = _loadingMessages[_loadingMessages.Count - 1];
		if (text != null)
		{
			_loadingMessage.SetAlias(text);
			_loadingMessage.gameObject.SetActive(true);
		}
		else
		{
			_loadingMessage.gameObject.SetActive(false);
		}
		callBack.InvokeSafe();
	}

	private void EnableLoadingImage()
	{
		_bgImage.gameObject.SetActive(true);
		_loadingImage.gameObject.SetActive(true);
		_loadingImage.transform.rotation = Quaternion.Euler(new Vector3(base.transform.rotation.x, base.transform.rotation.y, 0f));
		_imageRotating.Restart();
	}

	private void DisableLoadingImage()
	{
		_bgImage.gameObject.SetActive(false);
		_loadingImage.gameObject.SetActive(false);
		_imageRotating.Pause();
	}

	private void RemoveAlias(string aliasToRemove)
	{
		for (int i = 0; i < _loadingMessages.Count; i++)
		{
			if (_loadingMessages[i] == aliasToRemove)
			{
				_loadingMessages.RemoveAt(i);
				return;
			}
		}
		Debug.LogError("Trying to Clear() the alias that is not present in loadingMessagesCount. This should not happen.");
	}
}
