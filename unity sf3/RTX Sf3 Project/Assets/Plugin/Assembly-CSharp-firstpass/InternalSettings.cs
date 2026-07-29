using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[Serializable]
public class InternalSettings
{
	private static InternalSettings Instance;

	public const string FileConfig = "Configs/internalSettings";

	public Dictionary<string, string> ExternalPaths = new Dictionary<string, string>();

	public Dictionary<string, string> StoreURLs = new Dictionary<string, string>();

	public LocalInternalSettings LocalSettings;

	public JObject ServerSettings;

	public static LocalInternalSettings Local
	{
		get
		{
			return Instance.LocalSettings;
		}
	}

	public static string NoImageTexture
	{
		get
		{
			return Local.NoImageTexture;
		}
	}

	public static bool IsDebug
	{
		get
		{
			return Local.Debug;
		}
	}

	public static string BundlesPath
	{
		get
		{
			return Local.BundlesPath;
		}
	}

	public static string StoreUrl
	{
		get
		{
			string key = string.Empty;
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				key = "iOS";
				break;
			case RuntimePlatform.Android:
				key = "Android";
				break;
			}
			if (Instance.StoreURLs != null && Instance.StoreURLs.ContainsKey(key))
			{
				return Instance.StoreURLs[key];
			}
			return string.Empty;
		}
	}

	static InternalSettings()
	{
		Init(GlobalLoad.GetLoadText("Configs/internalSettings"));
	}

	protected static void Init(string content)
	{
		Instance = JsonConvert.DeserializeObject<InternalSettings>(content);
		Instance.ConvertPath(GlobalPath.PathToLoaderFolder);
	}

	public static T GetServerObject<T>(string name) where T : class
	{
		JToken jToken = Instance.ServerSettings[name];
		if (jToken != null)
		{
			return jToken.ToObject<T>();
		}
		return (T)null;
	}

	public static string GetPath(string name)
	{
		if (Instance != null && Instance.ExternalPaths.ContainsKey(name))
		{
			return Instance.ExternalPaths[name];
		}
		Debug.LogError("Internal Alias not found: " + name);
		return null;
	}

	private void ConvertPath(string replacepath)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (KeyValuePair<string, string> externalPath in ExternalPaths)
		{
			dictionary.Add(externalPath.Key, externalPath.Value.Replace("EXTERNAL_PATH", replacepath));
		}
		ExternalPaths = dictionary;
		string noImageTexture = LocalSettings.NoImageTexture.Replace("EXTERNAL_PATH", replacepath);
		LocalSettings.NoImageTexture = noImageTexture;
	}

	public static string GetString()
	{
		return Instance.ToString();
	}

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this, Formatting.Indented);
	}
}
