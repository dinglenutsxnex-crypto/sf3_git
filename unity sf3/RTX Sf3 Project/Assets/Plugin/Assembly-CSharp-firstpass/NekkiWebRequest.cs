using System;
using System.Collections;
using System.Collections.Generic;
using Nekki;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class NekkiWebRequest
{
	private object _externalData;

	private Coroutine _coroutine;

	private UnityWebRequest _www;

	private NekkiWebHandler _webHandler;

	private bool _isDispose;

	private bool _isTimeOut;

	private float _lastUpdateTime;

	private readonly float _downloadTimeout;

	private int _currentPosition;

	private NekkiUri _uri;

	private bool _certCheck;

	private bool _certError;

	public bool IsDone
	{
		get
		{
			return _www != null && (_www.isDone || IsFail);
		}
	}

	public bool IsSuccessful
	{
		get
		{
			return IsDone && !IsFail;
		}
	}

	public bool IsFail
	{
		get
		{
			return _certError || _www.isNetworkError || _isTimeOut || (!IsResponseCodeSuccessful() && !InProgress());
		}
	}

	public string Url
	{
		get
		{
			return _uri.OriginalString;
		}
	}

	public string Error
	{
		get
		{
			return GetError();
		}
	}

	public byte[] Bytes
	{
		get
		{
			return _www.downloadHandler.data;
		}
	}

	public string Text
	{
		get
		{
			return _www.downloadHandler.text;
		}
	}

	public float Progress
	{
		get
		{
			return _www.downloadProgress;
		}
	}

	public float Total
	{
		get
		{
			return (!NekkiUtils.IsCompilation) ? _webHandler.Total : 0;
		}
	}

	public int Position
	{
		get
		{
			return (!NekkiUtils.IsCompilation) ? _webHandler.Position : ((int)_www.downloadedBytes);
		}
	}

	public event Action<NekkiWebRequest> OnSuccessful = delegate
	{
	};

	public event Action<NekkiWebRequest> OnError = delegate
	{
	};

	public event Action<NekkiWebRequest> OnProgress = delegate
	{
	};

	public NekkiWebRequest(float timeout = 5f)
	{
		_downloadTimeout = timeout;
		Reset();
	}

	public virtual void Send(string urlRequest, bool checkCert)
	{
		_certCheck = checkCert;
		Send(UnityWebRequest.Get(urlRequest));
	}

	public virtual void Send(string urlRequest, string postData, bool checkCert)
	{
		_certCheck = checkCert;
		Send(UnityWebRequest.Post(urlRequest, postData));
	}

	public virtual void Send(string urlRequest, Dictionary<string, string> postData, bool checkCert)
	{
		_certCheck = checkCert;
		Send(UnityWebRequest.Post(urlRequest, postData));
	}

	private void Send(UnityWebRequest webRequest)
	{
		_uri = new NekkiUri(webRequest.url);
		_webHandler = GetDownloadHandler(_uri);
		_lastUpdateTime = Time.realtimeSinceStartup;
		_www = webRequest;
		if (NekkiUtils.IsCompilation)
		{
			RequestCurrentFrame();
			return;
		}
		_www.downloadHandler = _webHandler;
		_coroutine = Routiner.Go(RequestRuntime());
	}

	private IEnumerator RequestRuntime()
	{
		SendRequest();
		while (!IsDone)
		{
			yield return new WaitForSecondsRealtime(1f / 60f);
			UpdateRequest();
		}
		FinishRequest();
	}

	private void RequestCurrentFrame()
	{
		SendRequest();
		while (!IsDone)
		{
			UpdateRequest();
		}
		FinishRequest();
	}

	private void SendRequest()
	{
		Log("WebRequest Send " + Url);
		if (_certCheck && !CertificateValidator.IsHttpsUrlValid(Url))
		{
			_certError = true;
			SendError();
		}
		else
		{
			_www.Send();
		}
	}

	private void UpdateRequest()
	{
		if (_currentPosition != Position)
		{
			_currentPosition = Position;
			_lastUpdateTime = Time.realtimeSinceStartup;
		}
		else
		{
			_isTimeOut = Time.realtimeSinceStartup - _lastUpdateTime >= _downloadTimeout;
		}
		SendProgress();
	}

	private void FinishRequest()
	{
		if (_webHandler != null && NekkiUtils.IsCompilation)
		{
			_webHandler.Finish();
		}
		if (IsSuccessful)
		{
			SendSuccessful();
		}
		else
		{
			SendError();
		}
		Abort();
	}

	protected virtual void SendSuccessful()
	{
		Log("WebRequest Successful " + Url);
		this.OnSuccessful.InvokeSafe(this);
		ResetDelegates();
	}

	protected virtual void SendError()
	{
		Log("WebRequest Error " + Url + " " + Error);
		this.OnError.InvokeSafe(this);
		ResetDelegates();
	}

	protected virtual void SendProgress()
	{
		this.OnProgress.InvokeSafe(this);
	}

	protected virtual NekkiWebHandler GetDownloadHandler(NekkiUri uri)
	{
		return new NekkiWebHandlerRequest(uri);
	}

	public void Abort(bool isError = false)
	{
		if (isError)
		{
			SendError();
		}
		if (!_isDispose)
		{
			_www.Abort();
			_webHandler.Abort();
			_isDispose = true;
			_externalData = null;
		}
		if (_coroutine != null)
		{
			Routiner.Stop(_coroutine);
			_coroutine = null;
		}
		Reset();
	}

	private void Reset()
	{
		_isDispose = false;
		_isTimeOut = false;
		_lastUpdateTime = 0f;
		_currentPosition = 0;
	}

	private void ResetDelegates()
	{
		this.OnSuccessful = null;
		this.OnError = null;
		this.OnSuccessful = null;
	}

	private void Log(string value)
	{
		Debug.Log(value);
	}

	private bool IsResponseCodeSuccessful()
	{
		return 200 <= _www.responseCode && _www.responseCode < 300;
	}

	private bool InProgress()
	{
		return _www.responseCode == -1;
	}

	private string GetError()
	{
		if (_certError)
		{
			return "HTTPS certificate check error";
		}
		if (_isTimeOut)
		{
			return "Failed with timeout";
		}
		if (!IsResponseCodeSuccessful())
		{
			return "Failed with responseCode - " + _www.responseCode;
		}
		return _www.error;
	}

	public T GetJson<T>() where T : class
	{
		try
		{
			return JsonConvert.DeserializeObject<T>(Text);
		}
		catch (Exception ex)
		{
			Debug.LogError("NekkiWeb - Error Parse JSON (" + ex.Message + ")");
		}
		return (T)null;
	}

	public void SetExternalData(object value)
	{
		_externalData = value;
	}

	public T GetExternalData<T>()
	{
		return (T)_externalData;
	}
}
