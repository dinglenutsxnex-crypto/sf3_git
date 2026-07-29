using System;
using System.Collections.Generic;

public class EventListener<T, U> where T : CallEventArgsBase<U>
{
	private readonly Dictionary<U, List<Action<T>>> _subscription = new Dictionary<U, List<Action<T>>>();

	public void addEventListener(U evt, Action<T> callback)
	{
		if (!_subscription.ContainsKey(evt))
		{
			_subscription.Add(evt, new List<Action<T>>());
		}
		_subscription[evt].Add(callback);
	}

	public void addEventListener(U[] evt, Action<T> callback)
	{
		for (int i = 0; i < evt.Length; i++)
		{
			addEventListener(evt[i], callback);
		}
	}

	public void removeAllEventListener()
	{
		_subscription.Clear();
	}

	public void removeEvent(U evt)
	{
		if (_subscription.ContainsKey(evt))
		{
			_subscription.Remove(evt);
		}
	}

	public void removeEventListener(U evt, Action<T> callback)
	{
		if (_subscription.ContainsKey(evt))
		{
			while (_subscription[evt].Contains(callback))
			{
				_subscription[evt].Remove(callback);
			}
		}
	}

	public void callEvent(U evt, object content = null)
	{
		if (!_subscription.ContainsKey(evt))
		{
			return;
		}
		List<Action<T>> list = new List<Action<T>>(_subscription[evt]);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] != null && _subscription.ContainsKey(evt) && _subscription[evt].Contains(list[i]))
			{
				T obj = Activator.CreateInstance<T>();
				obj.Setup(evt, content, this);
				list[i](obj);
			}
		}
	}
}
