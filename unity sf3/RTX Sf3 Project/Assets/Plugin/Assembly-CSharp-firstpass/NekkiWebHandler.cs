using UnityEngine.Networking;

public class NekkiWebHandler : DownloadHandlerScript
{
	private const int preallocatedSize = 8192;

	private readonly NekkiUri _uri;

	private bool _isCompleteContent;

	private int _offset;

	private int _total;

	private bool _isAbort;

	public string Url
	{
		get
		{
			return _uri.OriginalString;
		}
	}

	public int Total
	{
		get
		{
			return _total;
		}
	}

	public int Position
	{
		get
		{
			return _offset;
		}
	}

	public bool IsDone
	{
		get
		{
			return _isCompleteContent;
		}
	}

	public NekkiWebHandler(NekkiUri uriRequest)
		: base(new byte[8192])
	{
		_uri = uriRequest;
		_isCompleteContent = false;
		_offset = 0;
		_total = 0;
		_isAbort = false;
	}

	public virtual void Abort()
	{
		_isAbort = true;
	}

	public virtual void Finish()
	{
		CompleteContent();
	}

	protected override void ReceiveContentLength(int contentLength)
	{
		_total = contentLength;
		SetContentLength(_total);
	}

	protected override bool ReceiveData(byte[] data, int dataLength)
	{
		if (_isAbort || data == null || data.Length < 1)
		{
			return false;
		}
		SetBytes(data, _offset, dataLength);
		_offset += dataLength;
		return true;
	}

	protected override void CompleteContent()
	{
		_total = _offset;
		_isCompleteContent = true;
		SetCompleteContent();
	}

	protected override float GetProgress()
	{
		return (_offset <= 0) ? 0f : ((float)Total / (float)_offset);
	}

	protected virtual void SetContentLength(int contentLength)
	{
	}

	protected virtual void SetBytes(byte[] data, int dataOffset, int dataLength)
	{
	}

	protected virtual void SetCompleteContent()
	{
	}
}
