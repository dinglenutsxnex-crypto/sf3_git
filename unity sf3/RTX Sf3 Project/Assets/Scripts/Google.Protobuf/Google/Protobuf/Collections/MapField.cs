using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Google.Protobuf.Reflection;

namespace Google.Protobuf.Collections
{
	public sealed class MapField<TKey, TValue> : IDeepCloneable<MapField<TKey, TValue>>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IEquatable<MapField<TKey, TValue>>, IDictionary, ICollection
	{
		private class DictionaryEnumerator : IDictionaryEnumerator, IEnumerator
		{
			private readonly IEnumerator<KeyValuePair<TKey, TValue>> enumerator;

			public object Current
			{
				get
				{
					return Entry;
				}
			}

			public DictionaryEntry Entry
			{
				get
				{
					return new DictionaryEntry(Key, Value);
				}
			}

			public object Key
			{
				get
				{
					return enumerator.Current.Key;
				}
			}

			public object Value
			{
				get
				{
					return enumerator.Current.Value;
				}
			}

			internal DictionaryEnumerator(IEnumerator<KeyValuePair<TKey, TValue>> enumerator)
			{
				this.enumerator = enumerator;
			}

			public bool MoveNext()
			{
				return enumerator.MoveNext();
			}

			public void Reset()
			{
				enumerator.Reset();
			}
		}

		public sealed class Codec
		{
			internal class MessageAdapter : IMessage
			{
				private static readonly byte[] ZeroLengthMessageStreamData = new byte[1];

				private readonly Codec codec;

				internal TKey Key { get; set; }

				internal TValue Value { get; set; }

				MessageDescriptor IMessage.Descriptor
				{
					get
					{
						return null;
					}
				}

				internal MessageAdapter(Codec codec)
				{
					this.codec = codec;
				}

				internal void Reset()
				{
					Key = codec.keyCodec.DefaultValue;
					Value = codec.valueCodec.DefaultValue;
				}

				public void MergeFrom(CodedInputStream input)
				{
					uint num;
					while ((num = input.ReadTag()) != 0)
					{
						if (num == codec.keyCodec.Tag)
						{
							Key = codec.keyCodec.Read(input);
						}
						else if (num == codec.valueCodec.Tag)
						{
							Value = codec.valueCodec.Read(input);
						}
						else
						{
							input.SkipLastField();
						}
					}
					if (Value == null)
					{
						Value = codec.valueCodec.Read(new CodedInputStream(ZeroLengthMessageStreamData));
					}
				}

				public void WriteTo(CodedOutputStream output)
				{
					codec.keyCodec.WriteTagAndValue(output, Key);
					codec.valueCodec.WriteTagAndValue(output, Value);
				}

				public int CalculateSize()
				{
					return codec.keyCodec.CalculateSizeWithTag(Key) + codec.valueCodec.CalculateSizeWithTag(Value);
				}
			}

			private readonly FieldCodec<TKey> keyCodec;

			private readonly FieldCodec<TValue> valueCodec;

			private readonly uint mapTag;

			internal uint MapTag
			{
				get
				{
					return mapTag;
				}
			}

			public Codec(FieldCodec<TKey> keyCodec, FieldCodec<TValue> valueCodec, uint mapTag)
			{
				this.keyCodec = keyCodec;
				this.valueCodec = valueCodec;
				this.mapTag = mapTag;
			}
		}

		private class MapView<T> : ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
		{
			private readonly MapField<TKey, TValue> parent;

			private readonly Func<KeyValuePair<TKey, TValue>, T> projection;

			private readonly Func<T, bool> containsCheck;

			public int Count
			{
				get
				{
					return parent.Count;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public bool IsSynchronized
			{
				get
				{
					return false;
				}
			}

			public object SyncRoot
			{
				get
				{
					return parent;
				}
			}

			internal MapView(MapField<TKey, TValue> parent, Func<KeyValuePair<TKey, TValue>, T> projection, Func<T, bool> containsCheck)
			{
				this.parent = parent;
				this.projection = projection;
				this.containsCheck = containsCheck;
			}

			public void Add(T item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(T item)
			{
				return containsCheck(item);
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}
				if (arrayIndex + Count >= array.Length)
				{
					throw new ArgumentException("Not enough space in the array", "array");
				}
				using (IEnumerator<T> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T current = enumerator.Current;
						array[arrayIndex++] = current;
					}
				}
			}

			public IEnumerator<T> GetEnumerator()
			{
				return parent.list.Select(projection).GetEnumerator();
			}

			public bool Remove(T item)
			{
				throw new NotSupportedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void CopyTo(Array array, int index)
			{
				if (index < 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				if (index + Count >= array.Length)
				{
					throw new ArgumentException("Not enough space in the array", "array");
				}
				using (IEnumerator<T> enumerator = GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T current = enumerator.Current;
						array.SetValue(current, index++);
					}
				}
			}
		}

		private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> map = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>();

		private readonly LinkedList<KeyValuePair<TKey, TValue>> list = new LinkedList<KeyValuePair<TKey, TValue>>();

		public TValue this[TKey key]
		{
			get
			{
				ProtoPreconditions.CheckNotNullUnconstrained(key, "key");
				TValue value;
				if (TryGetValue(key, out value))
				{
					return value;
				}
				throw new KeyNotFoundException();
			}
			set
			{
				ProtoPreconditions.CheckNotNullUnconstrained(key, "key");
				if (value == null)
				{
					ProtoPreconditions.CheckNotNullUnconstrained(value, "value");
				}
				KeyValuePair<TKey, TValue> value2 = new KeyValuePair<TKey, TValue>(key, value);
				LinkedListNode<KeyValuePair<TKey, TValue>> value3;
				if (map.TryGetValue(key, out value3))
				{
					value3.Value = value2;
					return;
				}
				value3 = list.AddLast(value2);
				map[key] = value3;
			}
		}

		public ICollection<TKey> Keys
		{
			get
			{
				return new MapView<TKey>(this, (KeyValuePair<TKey, TValue> pair) => pair.Key, ContainsKey);
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				return new MapView<TValue>(this, (KeyValuePair<TKey, TValue> pair) => pair.Value, ContainsValue);
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return (ICollection)Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return (ICollection)Values;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				ProtoPreconditions.CheckNotNull(key, "key");
				if (!(key is TKey))
				{
					return null;
				}
				TValue value;
				TryGetValue((TKey)key, out value);
				return value;
			}
			set
			{
				this[(TKey)key] = (TValue)value;
			}
		}

		public MapField<TKey, TValue> Clone()
		{
			MapField<TKey, TValue> mapField = new MapField<TKey, TValue>();
			if (typeof(IDeepCloneable<TValue>).IsAssignableFrom(typeof(TValue)))
			{
				foreach (KeyValuePair<TKey, TValue> item in list)
				{
					mapField.Add(item.Key, ((IDeepCloneable<TValue>)(object)item.Value).Clone());
				}
			}
			else
			{
				mapField.Add(this);
			}
			return mapField;
		}

		public void Add(TKey key, TValue value)
		{
			if (ContainsKey(key))
			{
				throw new ArgumentException("Key already exists in map", "key");
			}
			this[key] = value;
		}

		public bool ContainsKey(TKey key)
		{
			ProtoPreconditions.CheckNotNullUnconstrained(key, "key");
			return map.ContainsKey(key);
		}

		private bool ContainsValue(TValue value)
		{
			EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;
			return list.Any((KeyValuePair<TKey, TValue> pair) => comparer.Equals(pair.Value, value));
		}

		public bool Remove(TKey key)
		{
			ProtoPreconditions.CheckNotNullUnconstrained(key, "key");
			LinkedListNode<KeyValuePair<TKey, TValue>> value;
			if (map.TryGetValue(key, out value))
			{
				map.Remove(key);
				value.List.Remove(value);
				return true;
			}
			return false;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> value2;
			if (map.TryGetValue(key, out value2))
			{
				value = value2.Value.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		public void Add(IDictionary<TKey, TValue> entries)
		{
			ProtoPreconditions.CheckNotNull(entries, "entries");
			foreach (KeyValuePair<TKey, TValue> entry in entries)
			{
				Add(entry.Key, entry.Value);
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public void Clear()
		{
			list.Clear();
			map.Clear();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue value;
			if (TryGetValue(item.Key, out value))
			{
				return EqualityComparer<TValue>.Default.Equals(item.Value, value);
			}
			return false;
		}

		void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			if (item.Key == null)
			{
				throw new ArgumentException("Key is null", "item");
			}
			LinkedListNode<KeyValuePair<TKey, TValue>> value;
			if (map.TryGetValue(item.Key, out value) && EqualityComparer<TValue>.Default.Equals(item.Value, value.Value.Value))
			{
				map.Remove(item.Key);
				value.List.Remove(value);
				return true;
			}
			return false;
		}

		public override bool Equals(object other)
		{
			return Equals(other as MapField<TKey, TValue>);
		}

		public override int GetHashCode()
		{
			EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			int num = 0;
			foreach (KeyValuePair<TKey, TValue> item in list)
			{
				num ^= item.Key.GetHashCode() * 31 + @default.GetHashCode(item.Value);
			}
			return num;
		}

		public bool Equals(MapField<TKey, TValue> other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (other.Count != Count)
			{
				return false;
			}
			EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
			using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<TKey, TValue> current = enumerator.Current;
					TValue value;
					if (!other.TryGetValue(current.Key, out value))
					{
						return false;
					}
					if (!@default.Equals(value, current.Value))
					{
						return false;
					}
				}
			}
			return true;
		}

		public void AddEntriesFrom(CodedInputStream input, Codec codec)
		{
			Codec.MessageAdapter messageAdapter = new Codec.MessageAdapter(codec);
			do
			{
				messageAdapter.Reset();
				input.ReadMessage(messageAdapter);
				this[messageAdapter.Key] = messageAdapter.Value;
			}
			while (input.MaybeConsumeTag(codec.MapTag));
		}

		public void WriteTo(CodedOutputStream output, Codec codec)
		{
			Codec.MessageAdapter messageAdapter = new Codec.MessageAdapter(codec);
			foreach (KeyValuePair<TKey, TValue> item in list)
			{
				messageAdapter.Key = item.Key;
				messageAdapter.Value = item.Value;
				output.WriteTag(codec.MapTag);
				output.WriteMessage(messageAdapter);
			}
		}

		public int CalculateSize(Codec codec)
		{
			if (Count == 0)
			{
				return 0;
			}
			Codec.MessageAdapter messageAdapter = new Codec.MessageAdapter(codec);
			int num = 0;
			foreach (KeyValuePair<TKey, TValue> item in list)
			{
				messageAdapter.Key = item.Key;
				messageAdapter.Value = item.Value;
				num += CodedOutputStream.ComputeRawVarint32Size(codec.MapTag);
				num += CodedOutputStream.ComputeMessageSize(messageAdapter);
			}
			return num;
		}

		public override string ToString()
		{
			StringWriter stringWriter = new StringWriter();
			JsonFormatter.Default.WriteDictionary(stringWriter, this);
			return stringWriter.ToString();
		}

		void IDictionary.Add(object key, object value)
		{
			Add((TKey)key, (TValue)value);
		}

		bool IDictionary.Contains(object key)
		{
			if (!(key is TKey))
			{
				return false;
			}
			return ContainsKey((TKey)key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new DictionaryEnumerator(GetEnumerator());
		}

		void IDictionary.Remove(object key)
		{
			ProtoPreconditions.CheckNotNull(key, "key");
			if (key is TKey)
			{
				Remove((TKey)key);
			}
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.Select((KeyValuePair<TKey, TValue> pair) => new DictionaryEntry(pair.Key, pair.Value)).ToList()).CopyTo(array, index);
		}
	}
}
