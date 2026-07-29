using System;
using System.Linq;
using UnityEngine;

public static class NativeExtensions
{
	public static void InvokeSafe(this Action action)
	{
		if (action != null)
		{
			action();
		}
	}

	public static void InvokeSafe<T>(this Action<T> action, T value)
	{
		if (action != null)
		{
			action(value);
		}
	}

	public static string DelegatesToString<T>(this Action<T> action)
	{
		return "(" + action.GetInvocationList().Length + " total) " + string.Join(", ", (from d in action.GetInvocationList()
			select d.Method.Name).ToArray());
	}

	public static bool IsNullOrEmpty<T>(this Action<T> action)
	{
		return action == null || action.GetInvocationList().Length == 0;
	}

	public static float ToSeconds(this float milliseconds)
	{
		return milliseconds / 1000f;
	}

	public static bool IsEqual(this int number, Enum e)
	{
		try
		{
			return number == Convert.ToInt32(e);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return false;
		}
	}
}
