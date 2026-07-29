using System.IO;
using Nekki;
using UnityEngine.Networking;

public class NekkiWebDownload : NekkiWebRequest
{
	private readonly string _savePath;

	private readonly string _saveDownloadPath;

	public NekkiWebDownload(string path, float timeout = 5f)
		: base(timeout)
	{
		_savePath = path;
		_saveDownloadPath = Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_download.nekki";
		FilesUtil.DeleteFile(_savePath);
	}

	public void Send(UnityWebRequest webRequest)
	{
		if (FilesUtil.IsFileExists(_saveDownloadPath))
		{
			FileInfo fileInfo = new FileInfo(_saveDownloadPath);
			webRequest.SetRequestHeader("range-start", fileInfo.Length.ToString());
		}
		Send(webRequest);
	}

	protected override void SendSuccessful()
	{
		if (NekkiUtils.IsCompilation)
		{
			FilesUtil.WriteFileBytes(_savePath, base.Bytes);
			FilesUtil.DeleteFile(_saveDownloadPath);
		}
		base.SendSuccessful();
	}

	protected override void SendError()
	{
		FilesUtil.DeleteFile(_savePath);
		FilesUtil.DeleteFile(_saveDownloadPath);
		base.SendError();
	}

	protected override NekkiWebHandler GetDownloadHandler(NekkiUri uri)
	{
		return new NekkiWebHandlerDownload(uri, _savePath, _saveDownloadPath);
	}
}
