using System.Collections;
using System.Collections.Generic;

namespace Jint.Runtime
{
	public class MruPropertyCache<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable
	{
		private IDictionary<TKey, TValue> _dictionary = new Dictionary<TKey, TValue>();

		private LinkedList<KeyValuePair<TKey, TValue>> _list;

		public TValue this[TKey key]
		{
			get
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> result;
				if (Find(key, out result))
				{
					return result.Value.Value;
				}
				return _dictionary[key];
			}
			set
			{
				LinkedListNode<KeyValuePair<TKey, TValue>> result;
				if (!Find(key, out result))
				{
					_list.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
					_list.RemoveLast();
				}
				else
				{
					result.Value = new KeyValuePair<TKey, TValue>(key, value);
				}
				_dictionary[key] = value;
			}
		}

		public int Count
		{
			get
			{
				return _dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _dictionary.IsReadOnly;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return _dictionary.Keys;
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return _dictionary.Values;
			}
		}

		public MruPropertyCache(uint length)
		{
			_list = new LinkedList<KeyValuePair<TKey, TValue>>();
			for (int i = 0; i < length; i++)
			{
				_list.AddLast(new KeyValuePair<TKey, TValue>(default(TKey), default(TValue)));
			}
		}

		private bool Find(TKey key, out LinkedListNode<KeyValuePair<TKey, TValue>> result)
		{
			for (result = _list.First; result != null; result = result.Next)
			{
				if (key.Equals(result.Value.Key))
				{
					return true;
				}
			}
			return false;
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (!Find(item.Key, out result))
			{
				_list.AddFirst(item);
				_list.RemoveLast();
			}
			_dictionary.Add(item);
		}

		public void Add(TKey key, TValue value)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (!Find(key, out result))
			{
				_list.AddFirst(new KeyValuePair<TKey, TValue>(key, value));
				_list.RemoveLast();
			}
			_dictionary.Add(key, value);
		}

		public void Clear()
		{
			_list.Clear();
			_dictionary.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (Find(item.Key, out result))
			{
				return true;
			}
			return _dictionary.Contains(item);
		}

		public bool ContainsKey(TKey key)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (Find(key, out result))
			{
				return true;
			}
			return _dictionary.ContainsKey(key);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			_dictionary.CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (Find(item.Key, out result))
			{
				_list.Remove(result);
			}
			return _dictionary.Remove(item);
		}

		public bool Remove(TKey key)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (Find(key, out result))
			{
				_list.Remove(result);
			}
			return _dictionary.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> result;
			if (Find(key, out result))
			{
				value = result.Value.Value;
				return true;
			}
			return _dictionary.TryGetValue(key, out value);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _dictionary.GetEnumerator();
		}
	}
}
