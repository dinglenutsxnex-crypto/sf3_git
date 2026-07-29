using UnityEngine;
using UnityEngine.SceneManagement;

public class DownloadObb : MonoBehaviour
{
	private void Awake()
	{
		if (!GooglePlayDownloader.RunningOnAndroid())
		{
			Continue("not android");
			return;
		}
		string expansionFilePath = GooglePlayDownloader.GetExpansionFilePath();
		if (string.IsNullOrEmpty(expansionFilePath))
		{
			Continue("no obb file");
			return;
		}
		string mainOBBPath = GooglePlayDownloader.GetMainOBBPath(expansionFilePath);
		string patchOBBPath = GooglePlayDownloader.GetPatchOBBPath(expansionFilePath);
		if (mainOBBPath == null || patchOBBPath == null)
		{
			GooglePlayDownloader.FetchOBB();
			Continue("all done");
		}
	}

	private void Continue(string reason)
	{
		Debug.LogWarning(reason);
		SceneManager.LoadScene(1);
	}
}
