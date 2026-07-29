using System.Collections.Generic;
using UnityEngine;

public class BundlesUtil
{
	private static readonly Dictionary<string, AssetBundle> bundlesChach = new Dictionary<string, AssetBundle>();

	private static readonly List<AssetBundle> bundlesDependencies = new List<AssetBundle>();

	private static BundleConfig currentConfig;

	public static void InitConfig(BundleConfig config)
	{
		currentConfig = config;
		UnloadDependencies();
		LoadDependencies();
	}

	private static void UnloadDependencies()
	{
		foreach (AssetBundle bundlesDependency in bundlesDependencies)
		{
			bundlesDependency.Unload(true);
		}
		bundlesDependencies.Clear();
	}

	private static void LoadDependencies()
	{
		List<string> allDependencies = currentConfig.GetAllDependencies();
		foreach (string item in allDependencies)
		{
			AssetBundle assetBundle = GetAssetBundle(item);
			if ((bool)assetBundle)
			{
				bundlesDependencies.Add(assetBundle);
			}
		}
	}

	public static T[] GetObjects<T>(string path) where T : Object
	{
		AssetBundle assetBundleSafe = GetAssetBundleSafe(ref path);
		if (assetBundleSafe == null)
		{
			return null;
		}
		return assetBundleSafe.LoadAssetWithSubAssets<T>(path);
	}

	public static T GetObject<T>(string path) where T : Object
	{
		AssetBundle assetBundleSafe = GetAssetBundleSafe(ref path);
		if (assetBundleSafe == null)
		{
			return (T)null;
		}
		return assetBundleSafe.LoadAsset<T>(path);
	}

	private static AssetBundle GetAssetBundleSafe(ref string path)
	{
		if (currentConfig != null)
		{
			AssetsData assetsData = currentConfig.GetAssetsData(path);
			if (!path.IsNullOrEmpty() && assetsData != null)
			{
				AssetBundle assetBundle = GetAssetBundle(assetsData.BundleName);
				path = assetsData.Path;
				return assetBundle;
			}
		}
		return null;
	}

	public static void UnloadAsset(Object obj)
	{
		ResourcesUtil.UnloadAsset(obj);
	}

	public static void UnloadUnusedAssets()
	{
	}

	public static AssetBundle GetAssetBundle(string name)
	{
		if (bundlesChach.ContainsKey(name))
		{
			return bundlesChach[name];
		}
		if (currentConfig != null)
		{
			string bundlePath = currentConfig.GetBundlePath(name);
			if (!bundlePath.IsNullOrEmpty())
			{
				AssetBundle assetBundle = AssetBundle.LoadFromFile(bundlePath);
				if ((bool)assetBundle)
				{
					bundlesChach.Add(name, assetBundle);
				}
				return assetBundle;
			}
		}
		return null;
	}
}
