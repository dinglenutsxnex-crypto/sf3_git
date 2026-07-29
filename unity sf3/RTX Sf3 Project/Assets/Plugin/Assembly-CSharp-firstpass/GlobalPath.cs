using System;
using System.IO;
using UnityEngine;

public class GlobalPath
{
	private const string PathToResourcesFolder = "Resources";

	public static readonly string PathToLoaderFolder = "assets/gamedata/" + "Resources".ToLower();

	public static string LogsPath
	{
		get
		{
			return GameDataPath + "/Logs";
		}
	}

	public static string ExternalPath
	{
		get
		{
			return GameDataPath + "/Resources";
		}
	}

	public static string ApplicationPath
	{
		get
		{
			return ApplicationGameDataPath + "/Resources";
		}
	}

	public static string GameDataPath
	{
		get
		{
			if (SystemProperties.IsMobilePlatform)
			{
				return Application.persistentDataPath + "/gamedata";
			}
			string text = Application.dataPath.Replace("Assets", string.Empty).TrimEnd('/');
			return text + "/gamedata";
		}
	}

	public static string ApplicationGameDataPath
	{
		get
		{
			return Application.dataPath + "/gamedata";
		}
	}

	public static int GetIndexExtension(string path, int startindex = 0)
	{
		int num = path.LastIndexOf(".", StringComparison.OrdinalIgnoreCase);
		return (num <= -1) ? (path.Length - startindex) : (num - startindex);
	}

	public static string GameDataCombine(string path)
	{
		return Path.Combine(GameDataPath, path);
	}

	private static string GetPath(string key)
	{
		return InternalSettings.GetPath(key);
	}

	public static string GetInternalPath(string key, string name = "")
	{
		return GetPath(key) + name;
	}

	public static string GetResoursesPath(string path)
	{
		return PathToLoaderFolder + "/" + path;
	}
}
