using System;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjectsManager : MonoBehaviour
{
	private static StaticObjectsManager _instance;

	private Transform _transform;

	private HashSet<GameObject> _staticObjects;

	protected static StaticObjectsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject gameObject = new GameObject("STATIC_OBJECTS");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
				_instance = gameObject.AddComponent<StaticObjectsManager>();
				_instance._transform = gameObject.transform;
				_instance._staticObjects = new HashSet<GameObject>();
			}
			return _instance;
		}
	}

	public static event Action OnApplicationPauseEvent;

	public static event Action OnApplicationResumeEvent;

	public static event Action OnApplicationQuitEvent;

	public static void AddObject(GameObject objectValue, bool parented = true)
	{
		if (!Instance._staticObjects.Contains(objectValue))
		{
			if (parented)
			{
				objectValue.transform.parent = Instance._transform;
			}
			else
			{
				UnityEngine.Object.DontDestroyOnLoad(objectValue);
			}
			if (objectValue.name[0] != '_')
			{
				objectValue.name = "_" + objectValue.name;
			}
			Instance._staticObjects.Add(objectValue);
		}
	}

	public static void RemoveObject(GameObject objectValue)
	{
		if (Instance._staticObjects.Contains(objectValue))
		{
			Instance._staticObjects.Remove(objectValue);
			UnityEngine.Object.Destroy(objectValue);
		}
	}

	public static void Clear()
	{
		foreach (GameObject staticObject in Instance._staticObjects)
		{
			if ((bool)staticObject)
			{
				UnityEngine.Object.Destroy(staticObject);
			}
		}
		if ((bool)Instance)
		{
			UnityEngine.Object.Destroy(Instance.gameObject);
		}
		_instance = null;
	}

	private void OnApplicationQuit()
	{
		StaticObjectsManager.OnApplicationQuitEvent.InvokeSafe();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			StaticObjectsManager.OnApplicationPauseEvent.InvokeSafe();
		}
		else
		{
			StaticObjectsManager.OnApplicationResumeEvent.InvokeSafe();
		}
	}
}
