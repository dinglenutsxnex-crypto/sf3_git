using System;

public class NekkiWebHelper
{
	public static NekkiWebRequest DownloadFile(string url, string path, Action<NekkiWebRequest> onSuccessful, Action<NekkiWebRequest> onError, Action<NekkiWebRequest> onProgress = null, object data = null, float timeout = 5f, bool checkCert = true)
	{
		NekkiWebDownload nekkiWebDownload = new NekkiWebDownload(path, timeout);
		nekkiWebDownload.OnSuccessful += onSuccessful;
		nekkiWebDownload.OnError += onError;
		nekkiWebDownload.OnProgress += onProgress;
		nekkiWebDownload.SetExternalData(data);
		nekkiWebDownload.Send(url, checkCert);
		return nekkiWebDownload;
	}

	public static NekkiWebRequest SendRequest(string url, Action<NekkiWebRequest> onSuccessful, Action<NekkiWebRequest> onError, Action<NekkiWebRequest> onProgress = null, object data = null, float timeout = 5f, bool checkCert = true)
	{
		NekkiWebRequest nekkiWebRequest = new NekkiWebRequest(timeout);
		nekkiWebRequest.OnSuccessful += onSuccessful;
		nekkiWebRequest.OnError += onError;
		nekkiWebRequest.OnProgress += onProgress;
		nekkiWebRequest.SetExternalData(data);
		nekkiWebRequest.Send(url, checkCert);
		return nekkiWebRequest;
	}
}
