using System;
using System.IO;
using UnityEngine;

public class KeyChainBinding
{
	private const char SEPARATOR_CHAR = '|';

	private const string LOG_PREFIX = "KeyChainBinding -> {0}";

	private const string SET_LOG = "Set keychain values";

	private const string GET_LOG = "Retrieved applicationKey={0} applicationData={1}";

	private const string DELETE_LOG = "Cleared keychain values";

	private const string NEKKI_DIR = ".nekki";

	private const string KEYCHAIN_FILE_NAME = "keyChainStore.dat";

	private static string AppDataPath
	{
		get
		{
			return Path.Combine(Application.persistentDataPath, "keyChainStore.dat");
		}
	}

	private static string ExternalPath
	{
		get
		{
			return (Application.platform != RuntimePlatform.Android) ? null : Path.Combine(ExternalDir, "keyChainStore.dat");
		}
	}

	private static string ExternalDir
	{
		get
		{
			return Path.Combine(GetAndroidContextExternalFilesDir(), ".nekki");
		}
	}

	private static string UniqueIdentifier { get; set; }

	private static string GetAndroidContextExternalFilesDir()
	{
		string result = string.Empty;
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Environment"))
			{
				AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory", new object[0]);
				result = androidJavaObject.Call<string>("getAbsolutePath", new object[0]);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Error fetching native Android external storage dir: " + ex.Message);
		}
		return result;
	}

	public static void GetKeyChainData(out string applicationKey, out string data)
	{
		if (string.IsNullOrEmpty(UniqueIdentifier))
		{
			UniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		}
		string text = KeyChainFind();
		if (text != null)
		{
			text = KeyChainCrypto.Decrypt(text, UniqueIdentifier);
			string[] array = text.Split('|');
			applicationKey = array[0];
			data = text.Remove(0, applicationKey.Length + 1);
		}
		else
		{
			applicationKey = string.Empty;
			data = string.Empty;
		}
		LogFormat("Retrieved applicationKey={0} applicationData={1}", applicationKey, data);
	}

	private static string KeyChainFind()
	{
		string text = FilesUtil.ReadFileText(AppDataPath);
		if (text != null)
		{
			return text;
		}
		if (Application.platform != RuntimePlatform.Android)
		{
			return null;
		}
		try
		{
			text = FilesUtil.ReadFileText(ExternalPath);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
		return text;
	}

	public static void SetKeyChainData(string applicationKey, string data)
	{
		if (string.IsNullOrEmpty(UniqueIdentifier))
		{
			UniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
		}
		string text = string.Format("{0}{1}{2}", applicationKey, '|', data);
		string content = KeyChainCrypto.Crypt(text, UniqueIdentifier);
		FilesUtil.WriteFileText(AppDataPath, content);
		if (Application.platform == RuntimePlatform.Android)
		{
			try
			{
				FilesUtil.WriteFileText(ExternalPath, content);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		Log("Set keychain values");
	}

	public static void DeleteKeyChainData()
	{
		FilesUtil.DeleteFile(AppDataPath);
		if (Application.platform == RuntimePlatform.Android)
		{
			try
			{
				FilesUtil.DeleteFile(ExternalPath);
				FilesUtil.DeleteDirectory(ExternalDir);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		Log("Cleared keychain values");
	}

	private static void Log(string log)
	{
	}

	private static void LogFormat(string format, params object[] parms)
	{
	}
}
