using System;
using System.Collections.Generic;
using UnityEngine;

public class iOSBridge : MonoBehaviour
{
	private Dictionary<string, Action<string>> callbacks = new Dictionary<string, Action<string>>();

	public static iOSBridge create(string goName)
	{
		GameObject gameObject = new GameObject(goName);
		return gameObject.AddComponent<iOSBridge>();
	}

	public static iOSBridge createWithGameobject(GameObject go)
	{
		return go.AddComponent<iOSBridge>();
	}

	public void addCallback(string tag, Action<string> callback)
	{
		callbacks.Add(tag, callback);
	}

	public void removeCallback(string tag)
	{
		callbacks.Remove(tag);
	}

	public void removeInstance()
	{
		UnityEngine.Object.Destroy(this);
	}

	public void call(string tag)
	{
		Action<string> value;
		if (callbacks.TryGetValue(tag, out value))
		{
			value(string.Empty);
		}
		else
		{
			Debug.LogError("iOSBridge error: cant find tag: " + tag);
		}
	}

	public void callString(string tagAndString)
	{
		int num = tagAndString.IndexOf("(");
		int num2 = tagAndString.LastIndexOf(")");
		if (num == -1 || num2 == -1)
		{
			Debug.LogError("iOSBridge error: cant find passed string in tag: " + tagAndString);
			return;
		}
		string text = tagAndString.Substring(0, num);
		string obj = tagAndString.Substring(num + 1, num2 - num - 1);
		Action<string> value;
		if (callbacks.TryGetValue(text, out value))
		{
			value(obj);
		}
		else
		{
			Debug.LogError("iOSBridge error: cant find tag: " + text);
		}
	}

	private void Awake()
	{
		StaticObjectsManager.AddObject(base.gameObject, false);
	}
}
