using System;
using System.Collections;
using UnityEngine;

public class ExtentionBehaviour : MonoBehaviour
{
	private GameObject _gameObject;

	private Transform _transform;

	private Animator _animator;

	private EventListener<CallEventArgs, int> eventListener = new EventListener<CallEventArgs, int>();

	public new GameObject gameObject
	{
		get
		{
			if (!_gameObject)
			{
				_gameObject = base.gameObject;
			}
			return _gameObject;
		}
	}

	public new Transform transform
	{
		get
		{
			if (!_transform)
			{
				_transform = base.transform;
			}
			return _transform;
		}
	}

	public Animator animator
	{
		get
		{
			if (!_animator)
			{
				_animator = GetComponent<Animator>();
			}
			return _animator;
		}
	}

	protected void Log(object message, UnityEngine.Object source = null)
	{
		Debug.Log(message, source ?? this);
	}

	protected void LogWarning(object message, UnityEngine.Object source = null)
	{
		Debug.LogWarning(message, source ?? this);
	}

	protected void LogError(object message, UnityEngine.Object source = null)
	{
		Debug.LogError(message, source ?? this);
	}

	protected void LogException(Exception ex, UnityEngine.Object source = null)
	{
		Debug.LogException(ex, source ?? this);
	}

	protected void Invoke(Action action, float t)
	{
		StartCoroutine(InvokeActionRoutine(action, t));
	}

	private IEnumerator InvokeActionRoutine(Action action, float t)
	{
		yield return new WaitForSeconds(t);
		action();
	}

	protected virtual void OnDestroy()
	{
		eventListener.removeAllEventListener();
	}

	public void addEventListener(int evt, Action<CallEventArgs> callback)
	{
		eventListener.addEventListener(evt, callback);
	}

	public void addEventListener(int[] evt, Action<CallEventArgs> callback)
	{
		eventListener.addEventListener(evt, callback);
	}

	private void removeAllEventListener()
	{
		eventListener.removeAllEventListener();
	}

	public void removeEvent(int evt)
	{
		eventListener.removeEvent(evt);
	}

	public void removeEventListener(int evt, Action<CallEventArgs> callback)
	{
		eventListener.removeEventListener(evt, callback);
	}

	public void callEvent(int evt, object content = null)
	{
		eventListener.callEvent(evt, content);
	}

	public void StopCoroutineSafe(Coroutine routine)
	{
		if (routine != null)
		{
			StopCoroutine(routine);
		}
	}
}
