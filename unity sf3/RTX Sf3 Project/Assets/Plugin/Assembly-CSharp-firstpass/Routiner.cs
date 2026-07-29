using System;
using System.Collections;
using UnityEngine;

public class Routiner : MonoBehaviour
{
	private static Routiner _instance;

	private Action _instanceUpdate;

	private const float EPSILON = 0.0001f;

	public static void Init()
	{
		if (!_instance || !_instance.gameObject)
		{
			_instance = new GameObject("_routine").AddComponent<Routiner>();
			StaticObjectsManager.AddObject(_instance.gameObject, false);
		}
	}

	public static Coroutine Go(IEnumerator routine)
	{
		Init();
		return _instance.StartCoroutine(routine);
	}

	public static void Stop(Coroutine routine)
	{
		if (!(_instance == null) && routine != null)
		{
			_instance.StopCoroutine(routine);
		}
	}

	public static void AddUpdate(Action action)
	{
		Init();
		Routiner instance = _instance;
		instance._instanceUpdate = (Action)Delegate.Combine(instance._instanceUpdate, action);
	}

	private void Update()
	{
		if (_instance._instanceUpdate != null)
		{
			_instance._instanceUpdate();
		}
	}

	public static Coroutine GoDelayed(Action action, float delay)
	{
		Init();
		return _instance.StartCoroutine(_instance._goDelayed(action, delay));
	}

	private IEnumerator _goDelayed(Action action, float delay)
	{
		IEnumerator wait = WaitForRealSeconds(delay);
		while (wait.MoveNext())
		{
			yield return wait.Current;
		}
		action();
	}

	public static Coroutine GoDelayed(IEnumerator routine, float delay)
	{
		Init();
		return _instance.StartCoroutine(_instance._goDelayed(routine, delay));
	}

	private IEnumerator _goDelayed(IEnumerator routine, float delay)
	{
		IEnumerator wait = WaitForRealSeconds(delay);
		while (wait.MoveNext())
		{
			yield return wait.Current;
		}
		while (routine.MoveNext())
		{
			yield return routine.Current;
		}
	}

	private static IEnumerator WaitForRealSeconds(float delay)
	{
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup <= start + delay + 0.0001f)
		{
			yield return null;
		}
	}
}
