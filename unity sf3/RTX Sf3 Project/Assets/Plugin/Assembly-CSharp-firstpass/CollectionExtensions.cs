using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
	public static bool IsEmpty<T>(this List<T> list)
	{
		return list.Count == 0;
	}

	public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
	{
		return en.Select((T t) => new KeyValuePair<T, R>(t, evaluate(t))).Aggregate((KeyValuePair<T, R> max, KeyValuePair<T, R> next) => (next.Value.CompareTo(max.Value) <= 0) ? max : next).Key;
	}

	public static T MinBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
	{
		return en.Select((T t) => new KeyValuePair<T, R>(t, evaluate(t))).Aggregate((KeyValuePair<T, R> max, KeyValuePair<T, R> next) => (next.Value.CompareTo(max.Value) >= 0) ? max : next).Key;
	}

	public static void Resize<T>(this List<T> list, int sz, T c)
	{
		int count = list.Count;
		if (sz < count)
		{
			list.RemoveRange(sz, count - sz);
		}
		else if (sz > count)
		{
			if (sz > list.Capacity)
			{
				list.Capacity = sz;
			}
			list.AddRange(Enumerable.Repeat(c, sz - count));
		}
	}

	public static void Resize<T>(this List<T> list, int sz) where T : new()
	{
		list.Resize(sz, new T());
	}

	public static Dictionary<K, List<C>> DeapCopy<K, C>(Dictionary<K, List<C>> source) where C : ICloneable
	{
		Dictionary<K, List<C>> dictionary = new Dictionary<K, List<C>>(source);
		List<K> list = dictionary.Keys.ToList();
		foreach (K item in list)
		{
			List<C> list2 = new List<C>(source[item]);
			for (int i = 0; i < list2.Count; i++)
			{
				list2[i] = (C)source[item][i].Clone();
			}
			dictionary[item] = list2;
		}
		return dictionary;
	}

	public static Dictionary<K, C> DeapCopy<K, C>(Dictionary<K, C> source) where C : ICloneable
	{
		Dictionary<K, C> dictionary = new Dictionary<K, C>(source);
		List<K> list = dictionary.Keys.ToList();
		foreach (K item in list)
		{
			dictionary[item] = (C)source[item].Clone();
		}
		return dictionary;
	}
}
