using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NekkiUIRootModules : MonoBehaviour
{
	[Serializable]
	private class ModuleLayer
	{
		public string moduleName = string.Empty;

		public int layer;
	}

	private static NekkiUIRootModules _instance;

	private Dictionary<string, NekkiUIModule> _mounted = new Dictionary<string, NekkiUIModule>();

	[SerializeField]
	private string[] _mountOnStart;

	[SerializeField]
	private ModuleLayer[] _modulesLayersArray;

	private Dictionary<string, int> _modulesLayers;

	private Dictionary<string, GameObject> _preCachedPrefabs = new Dictionary<string, GameObject>();

	[SerializeField]
	private string[] _preCachePrefabsNames;

	public static NekkiUIRootModules Instance
	{
		get
		{
			return _instance;
		}
	}

	private int GetLayerForModule(string moduleName)
	{
		if (_modulesLayers == null)
		{
			ParseModulesLayersArray();
		}
		if (!_modulesLayers.ContainsKey(moduleName))
		{
			return 0;
		}
		return _modulesLayers[moduleName];
	}

	private void ParseModulesLayersArray()
	{
		_modulesLayers = new Dictionary<string, int>();
		ModuleLayer[] modulesLayersArray = _modulesLayersArray;
		foreach (ModuleLayer moduleLayer in modulesLayersArray)
		{
			_modulesLayers.Add(moduleLayer.moduleName, moduleLayer.layer);
		}
	}

	private void Awake()
	{
		_instance = this;
	}

	public void ForceClearCache()
	{
		foreach (KeyValuePair<string, GameObject> preCachedPrefab in _preCachedPrefabs)
		{
			UnityEngine.Object.Destroy(preCachedPrefab.Value);
		}
		_preCachedPrefabs = new Dictionary<string, GameObject>();
	}

	public void CacheModules()
	{
		_preCachedPrefabs = new Dictionary<string, GameObject>();
		string[] preCachePrefabsNames = _preCachePrefabsNames;
		foreach (string text in preCachePrefabsNames)
		{
			GameObject prefabInstance = GlobalLoad.GetPrefabInstance(text);
			_preCachedPrefabs.Add(text, prefabInstance);
			prefabInstance.SetActive(false);
		}
	}

	public void PerformMountOnStart()
	{
		if (_mountOnStart != null)
		{
			for (int i = 0; i < _mountOnStart.Length; i++)
			{
				MountNativeModule(_mountOnStart[i]);
			}
		}
	}

	public NekkiUIModule MountNGUIModule(string moduleName, object settings = null)
	{
		string fullPath = "UI/Modules/" + moduleName;
		return MountModule(moduleName, fullPath, settings);
	}

	public NekkiUIModule MountNativeModule(string moduleName, object settings = null)
	{
		string fullPath = "NativeUI/Modules/" + moduleName;
		return MountModule(moduleName, fullPath, settings);
	}

	private NekkiUIModule MountModule(string moduleName, string fullPath, object settings = null)
	{
		NekkiUIModule nekkiUIModule = null;
		if (!_mounted.ContainsKey(moduleName))
		{
			GameObject gameObject;
			if (_preCachedPrefabs.ContainsKey(moduleName))
			{
				gameObject = _preCachedPrefabs[moduleName];
				gameObject.SetActive(true);
			}
			else
			{
				gameObject = GlobalLoad.GetPrefabInstance(fullPath);
			}
			nekkiUIModule = gameObject.GetComponent<NekkiUIModule>();
			nekkiUIModule.name = nekkiUIModule.name.Replace("(Clone)", string.Empty);
			_mounted.Add(moduleName, nekkiUIModule);
			if (nekkiUIModule.isChildModule)
			{
				return nekkiUIModule;
			}
			if (nekkiUIModule.isNative)
			{
				nekkiUIModule.transform.SetParent(NekkiUIRoot.Instance.canvas.transform, false);
				RefreshLayer(nekkiUIModule);
			}
			else
			{
				GameObject gameObject2 = nekkiUIModule.gameObject;
				gameObject2.transform.parent = NekkiUIRoot.Instance.GetAnchor(nekkiUIModule.Side).transform;
				gameObject2.transform.localPosition = nekkiUIModule.Position;
				gameObject2.transform.localEulerAngles = nekkiUIModule.Rotation;
				gameObject2.transform.localScale = nekkiUIModule.Scale;
			}
			nekkiUIModule.Setup(settings);
		}
		else
		{
			nekkiUIModule = _mounted[moduleName];
		}
		return nekkiUIModule;
	}

	public bool UnmountModule(string modelName)
	{
		if (_mounted.ContainsKey(modelName))
		{
			if (!_preCachedPrefabs.ContainsKey(modelName))
			{
				GlobalLoad.Unload(_mounted[modelName].gameObject);
			}
			_mounted.Remove(modelName);
			return true;
		}
		NekkiCanvasRoot.instance.Unmount(modelName);
		return false;
	}

	public IEnumerator UnmountModuleAsync(string moduleName)
	{
		yield return new WaitForEndOfFrame();
		if (_mounted.ContainsKey(moduleName))
		{
			if (!_preCachedPrefabs.ContainsKey(moduleName))
			{
				GlobalLoad.Unload(_mounted[moduleName].gameObject);
			}
			_mounted.Remove(moduleName);
		}
		else
		{
			NekkiCanvasRoot.instance.Unmount(moduleName);
		}
	}

	public bool UnmountModule(NekkiUIModule module)
	{
		return UnmountModule(module.name);
	}

	public T MountNGUIModule<T>(string moduleName, object settings = null) where T : MonoBehaviour
	{
		NekkiUIModule nekkiUIModule = MountNGUIModule(moduleName, settings);
		if (nekkiUIModule != null)
		{
			return nekkiUIModule.GetComponent<T>();
		}
		return (T)null;
	}

	public T MountNativeModule<T>(string moduleName, object settings = null) where T : MonoBehaviour
	{
		NekkiUIModule nekkiUIModule = MountNativeModule(moduleName, settings);
		if (nekkiUIModule != null)
		{
			return nekkiUIModule.GetComponent<T>();
		}
		return (T)null;
	}

	public void RefreshLayer(NekkiUIModule module)
	{
		module.Layer = GetLayerForModule(module.ModuleName);
		module.transform.SetSiblingIndex(GetSiblingIndexForModule(module));
	}

	public void SetModuleLayer(NekkiUIModule module, int layerIndex)
	{
		foreach (KeyValuePair<string, NekkiUIModule> item in _mounted)
		{
			if (item.Value.Layer == layerIndex)
			{
				Debug.LogWarning("Trying to set module in already occupied by module \"" + item.Key + "\" layer #" + layerIndex);
			}
		}
		module.transform.SetSiblingIndex(layerIndex);
	}

	private int GetSiblingIndexForModule(NekkiUIModule module)
	{
		Transform transform = NekkiUIRoot.Instance.canvas.transform;
		int i = 0;
		int childCount = transform.childCount;
		int num = int.MinValue;
		for (; i < childCount; i++)
		{
			Transform child = transform.GetChild(i);
			NekkiUIModule component = child.GetComponent<NekkiUIModule>();
			if (component != null && component.ModuleName != module.ModuleName)
			{
				if (module.Layer <= component.Layer && module.Layer > num)
				{
					return i;
				}
				num = component.Layer;
			}
		}
		return transform.childCount - 1;
	}

	public NekkiUIModule GetModule(string moduleName)
	{
		if (_mounted.ContainsKey(moduleName))
		{
			return _mounted[moduleName];
		}
		return null;
	}

	public void EnableModule(NekkiUIModule moduleEnable)
	{
		EnableModules(moduleEnable.ModuleName);
	}

	public void EnableModules(params string[] modulesName)
	{
		for (int i = 0; i < modulesName.Length; i++)
		{
			if (_mounted.ContainsKey(modulesName[i]))
			{
				_mounted[modulesName[i]].gameObject.SetActive(true);
			}
		}
	}

	public void DisableModules(params string[] modulesName)
	{
		for (int i = 0; i < modulesName.Length; i++)
		{
			if (_mounted.ContainsKey(modulesName[i]))
			{
				_mounted[modulesName[i]].gameObject.SetActive(false);
			}
		}
	}

	public void DisableModulesExcept(params string[] moduleName)
	{
		foreach (KeyValuePair<string, NekkiUIModule> item in _mounted)
		{
			if (Array.IndexOf(moduleName, item.Key) == -1)
			{
				item.Value.gameObject.SetActive(false);
			}
		}
	}

	public virtual bool HasMountedModule(string moduleName)
	{
		if (_mounted.ContainsKey(moduleName))
		{
			return true;
		}
		return false;
	}

	public virtual List<string> GetEnabledModulesNames()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, NekkiUIModule> item in _mounted)
		{
			if (item.Value.gameObject.activeSelf)
			{
				list.Add(item.Key);
			}
		}
		return list;
	}

	public List<T> GetModulesWithComponent<T>() where T : Component
	{
		List<T> list = new List<T>();
		foreach (KeyValuePair<string, NekkiUIModule> item in _mounted)
		{
			T component = item.Value.GetComponent<T>();
			if (component != null)
			{
				list.Add(component);
			}
		}
		return list;
	}

	public void Clear()
	{
		int num;
		for (num = 0; num < _mounted.Count; num++)
		{
			UnmountModule(_mounted.ElementAt(num).Key);
			num--;
		}
	}
}
