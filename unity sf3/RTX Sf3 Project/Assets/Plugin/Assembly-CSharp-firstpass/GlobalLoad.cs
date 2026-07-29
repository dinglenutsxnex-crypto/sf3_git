using System;
using Newtonsoft.Json;
using UnityEngine;

public class GlobalLoad : GlobalPath
{
	private static Sprite _noImage;

	public static Sprite NoImageSprite
	{
		get
		{
			if (_noImage == null)
			{
				_noImage = GetLoadObject<Sprite>(InternalSettings.NoImageTexture);
			}
			return _noImage;
		}
	}

	public static Texture2D NoImageTexture
	{
		get
		{
			return NoImageSprite.texture;
		}
	}

	public static GameObject GetPrefabInstanceInternal(string internalAlias, string appendedPath = "")
	{
		GameObject prefabInternal = GetPrefabInternal(internalAlias, appendedPath);
		return GetGameObjectInstance(prefabInternal);
	}

	public static GameObject GetPrefabInstance(string path)
	{
		GameObject prefab = GetPrefab(path);
		return GetGameObjectInstance(prefab);
	}

	private static GameObject GetGameObjectInstance(GameObject gameObject)
	{
		if (gameObject != null)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
			gameObject2.name = gameObject2.name.Replace("(Clone)", string.Empty);
			return gameObject2;
		}
		return null;
	}

	public static GameObject GetPrefabInternal(string internalAlias, string appendedPath = "")
	{
		return GetLoadObjectInternal<GameObject>(internalAlias, appendedPath);
	}

	public static GameObject GetPrefab(string path)
	{
		return GetLoadObject<GameObject>(path);
	}

	public static AudioClip GetLoadAudioClipInternal(string internalAlias, string appendedPath = "")
	{
		return GetLoadObjectInternal<AudioClip>(internalAlias, appendedPath);
	}

	public static AudioClip GetLoadAudioClip(string path)
	{
		return GetLoadObject<AudioClip>(path);
	}

	public static Texture2D GetLoadTexture2DInternal(string internalAlias, string appendedPath = "")
	{
		Texture2D loadObjectInternal = GetLoadObjectInternal<Texture2D>(internalAlias, appendedPath);
		return ObjecOrDefault(loadObjectInternal, NoImageTexture);
	}

	public static Texture2D GetLoadTexture2D(string path)
	{
		Texture2D loadObject = GetLoadObject<Texture2D>(path);
		return ObjecOrDefault(loadObject, NoImageTexture);
	}

	public static Sprite GetLoadSpriteInternal(string internalAlias, string appendedPath = "")
	{
		Sprite loadObjectInternal = GetLoadObjectInternal<Sprite>(internalAlias, appendedPath);
		return ObjecOrDefault(loadObjectInternal, NoImageSprite);
	}

	public static Sprite GetLoadSprite(string path)
	{
		Sprite loadObject = GetLoadObject<Sprite>(path);
		return ObjecOrDefault(loadObject, NoImageSprite);
	}

	public static Sprite GetLoadSpriteFromTextureInternal(string internalAlias, string appendedPath = "")
	{
		Texture2D loadTexture2DInternal = GetLoadTexture2DInternal(internalAlias, appendedPath);
		return TexturesUtils.CreateSprite(loadTexture2DInternal);
	}

	public static Sprite GetLoadSpriteFromTexture(string path)
	{
		Texture2D loadTexture2D = GetLoadTexture2D(path);
		return TexturesUtils.CreateSprite(loadTexture2D);
	}

	public static Sprite GetLoadSpriteFromAtlas(string internalName, string atlasPath, string spriteName)
	{
		return TexturesUtils.GetSpriteFromAtlas(internalName, atlasPath, spriteName);
	}

	public static Sprite GetLoadSpriteFromAtlas(string atlasPath, string spriteName)
	{
		return TexturesUtils.GetSpriteFromAtlas(atlasPath, spriteName);
	}

	public static byte[] GetLoadBytesInternal(string internalAlias, string appendedPath = "")
	{
		TextAsset loadObjectInternal = GetLoadObjectInternal<TextAsset>(internalAlias, appendedPath);
		return (!(loadObjectInternal == null)) ? loadObjectInternal.bytes : null;
	}

	public static byte[] GetLoadBytes(string path)
	{
		TextAsset loadObject = GetLoadObject<TextAsset>(path);
		return (!(loadObject == null)) ? loadObject.bytes : null;
	}

	public static string GetLoadTextInternal(string internalAlias, string appendedPath = "")
	{
		TextAsset loadObjectInternal = GetLoadObjectInternal<TextAsset>(internalAlias, appendedPath);
		return (!(loadObjectInternal == null)) ? loadObjectInternal.text : null;
	}

	public static string GetLoadText(string path)
	{
		TextAsset loadObject = GetLoadObject<TextAsset>(path);
		return (!(loadObject == null)) ? loadObject.text : null;
	}

	public static T GetLoadJsonInternal<T>(string internalAlias, string appendedPath = "") where T : class
	{
		string loadTextInternal = GetLoadTextInternal(internalAlias, appendedPath);
		return GetJsonFromText<T>(loadTextInternal);
	}

	public static T GetLoadJson<T>(string path) where T : class
	{
		string loadText = GetLoadText(path);
		return GetJsonFromText<T>(loadText);
	}

	private static T GetJsonFromText<T>(string content) where T : class
	{
		if (!content.IsNullOrEmpty())
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(content);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error GetLoadJson [" + ex.Message + "]");
			}
		}
		return (T)null;
	}

	private static T ObjecOrDefault<T>(T obj, T defaultObj = null) where T : UnityEngine.Object
	{
		return obj ?? defaultObj;
	}

	public static T[] GetLoadObjectsInternal<T>(string internalAlias, string appendedPath = "") where T : UnityEngine.Object
	{
		return LoadAll<T>(GlobalPath.GetInternalPath(internalAlias, appendedPath));
	}

	public static T[] GetLoadObjects<T>(string fullPath) where T : UnityEngine.Object
	{
		return LoadAll<T>(GlobalPath.GetResoursesPath(fullPath));
	}

	public static T GetLoadObjectInternal<T>(string internalAlias, string appendedPath = "") where T : UnityEngine.Object
	{
		return Load<T>(GlobalPath.GetInternalPath(internalAlias, appendedPath));
	}

	public static T GetLoadObject<T>(string fullPath) where T : UnityEngine.Object
	{
		return Load<T>(GlobalPath.GetResoursesPath(fullPath));
	}

	private static TResult FirstOf<TResult, Arg>(Arg arg, params Func<Arg, TResult>[] funcs)
	{
		foreach (Func<Arg, TResult> func in funcs)
		{
			TResult val = func(arg);
			if (val != null)
			{
				return val;
			}
		}
		return default(TResult);
	}

	private static T[] LoadAll<T>(string path) where T : UnityEngine.Object
	{
		T[] array = FirstOf<T[], string>(path, GetBundles<T>, GetResources<T>);
		if (array == null)
		{
			Debug.LogError("LoadObject Not Found - " + path);
		}
		return array;
	}

	private static T Load<T>(string path) where T : UnityEngine.Object
	{
		T val = FirstOf<T, string>(path, GetBundle<T>, GetResource<T>);
		if (val == null)
		{
			Debug.LogWarning("LoadObject Not Found - " + path);
		}
		return val;
	}

	private static T[] GetResources<T>(string path) where T : UnityEngine.Object
	{
		return ResourcesUtil.GetResources<T>(path);
	}

	private static T GetResource<T>(string path) where T : UnityEngine.Object
	{
		return ResourcesUtil.GetResource<T>(path);
	}

	private static T[] GetBundles<T>(string path) where T : UnityEngine.Object
	{
		return BundlesUtil.GetObjects<T>(path);
	}

	private static T GetBundle<T>(string path) where T : UnityEngine.Object
	{
		return BundlesUtil.GetObject<T>(path);
	}

	public static void Unload(UnityEngine.Object obj, bool immediate = true)
	{
		if (!(obj == null))
		{
			if (obj.GetInstanceID() <= 0)
			{
				Destroy(obj, immediate);
			}
			else
			{
				ResourcesUtil.UnloadAsset(obj, immediate);
			}
		}
	}

	public static void Destroy(UnityEngine.Object obj, bool immediate = true)
	{
		try
		{
			if (!immediate)
			{
				UnityEngine.Object.Destroy(obj);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public static void UnloadUnusedAssets()
	{
		BundlesUtil.UnloadUnusedAssets();
		ResourcesUtil.UnloadUnusedAssets();
	}

	public static void GCCollectImmediately()
	{
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	public static string GetFileOrResourcesText(string path, string local, string name = "")
	{
		string text = FilesUtil.ReadFileText(path);
		if (!text.IsNullOrEmpty())
		{
			return text;
		}
		return GetLoadTextInternal(local, name);
	}
}
