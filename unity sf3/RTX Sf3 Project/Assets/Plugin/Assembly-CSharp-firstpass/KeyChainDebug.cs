using UnityEngine;

public class KeyChainDebug : MonoBehaviour
{
	private string appKey = "application key";

	private string data = "user data";

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, Screen.height));
		GUILayout.Label("Application Key:");
		appKey = GUILayout.TextField(appKey);
		GUILayout.Label("User Data:");
		data = GUILayout.TextField(data);
		if (GUILayout.Button("Set"))
		{
			KeyChainBinding.SetKeyChainData(appKey, data);
		}
		if (GUILayout.Button("Get"))
		{
			string applicationKey;
			string arg;
			KeyChainBinding.GetKeyChainData(out applicationKey, out arg);
			Debug.Log(string.Format("retrieved key: \"{0}\" retrieved data: \"{1}\"", applicationKey, arg));
		}
		if (GUILayout.Button("Reset"))
		{
			KeyChainBinding.DeleteKeyChainData();
		}
		GUILayout.EndArea();
	}
}
