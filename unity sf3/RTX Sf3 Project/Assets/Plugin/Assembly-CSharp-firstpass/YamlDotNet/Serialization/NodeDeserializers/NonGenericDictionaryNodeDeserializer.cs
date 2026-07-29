using System;
using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace YamlDotNet.Serialization.NodeDeserializers
{
	public sealed class NonGenericDictionaryNodeDeserializer : INodeDeserializer
	{
		private readonly IObjectFactory _objectFactory;

		public NonGenericDictionaryNodeDeserializer(IObjectFactory objectFactory)
		{
			_objectFactory = objectFactory;
		}

		bool INodeDeserializer.Deserialize(EventReader reader, Type expectedType, Func<EventReader, Type, object> nestedObjectDeserializer, out object value)
		{
			if (!typeof(IDictionary).IsAssignableFrom(expectedType))
			{
				value = false;
				return false;
			}
			reader.Expect<MappingStart>();
			IDictionary dictionary = (IDictionary)_objectFactory.Create(expectedType);
			while (!reader.Accept<MappingEnd>())
			{
				object key = nestedObjectDeserializer(reader, typeof(object));
				IValuePromise valuePromise = key as IValuePromise;
				object keyValue = nestedObjectDeserializer(reader, typeof(object));
				IValuePromise valuePromise2 = keyValue as IValuePromise;
				if (valuePromise == null)
				{
					if (valuePromise2 == null)
					{
						dictionary.Add(key, keyValue);
						continue;
					}
					valuePromise2.ValueAvailable += delegate(object v)
					{
						dictionary.Add(key, v);
					};
					continue;
				}
				if (valuePromise2 == null)
				{
					valuePromise.ValueAvailable += delegate(object v)
					{
						dictionary.Add(v, keyValue);
					};
					continue;
				}
				bool hasFirstPart = false;
				valuePromise.ValueAvailable += delegate(object v)
				{
					if (hasFirstPart)
					{
						dictionary.Add(v, keyValue);
					}
					else
					{
						key = v;
						hasFirstPart = true;
					}
				};
				valuePromise2.ValueAvailable += delegate(object v)
				{
					if (hasFirstPart)
					{
						dictionary.Add(key, v);
					}
					else
					{
						keyValue = v;
						hasFirstPart = true;
					}
				};
			}
			value = dictionary;
			reader.Expect<MappingEnd>();
			return true;
		}
	}
}
