using System;
using System.Collections.Generic;
using System.Linq;

namespace Nekki
{
	public class EnumsCompliancer
	{
		private struct EnumData
		{
			private readonly Type _enumType;

			private readonly Dictionary<int, string> _intStringData;

			private readonly Dictionary<string, int> _stringIntData;

			private readonly Dictionary<int, object> _intEnumeratorData;

			private readonly Dictionary<string, object> _stringEnumeratorData;

			private readonly List<object> _enumeratorsData;

			public EnumData(Type enumType)
			{
				_enumType = enumType;
				_intStringData = new Dictionary<int, string>();
				_stringIntData = new Dictionary<string, int>();
				_intEnumeratorData = new Dictionary<int, object>();
				_stringEnumeratorData = new Dictionary<string, object>();
				_enumeratorsData = new List<object>();
			}

			public Dictionary<int, string> GetIntStringData()
			{
				if (_intStringData.Count == 0)
				{
					string[] names = Enum.GetNames(_enumType);
					int[] array = Enum.GetValues(_enumType).Cast<int>().ToArray();
					for (int i = 0; i < names.Length; i++)
					{
						if (!_intStringData.ContainsKey(array[i]))
						{
							_intStringData.Add(array[i], names[i]);
						}
					}
				}
				return _intStringData;
			}

			public Dictionary<string, int> GetStringIntData()
			{
				if (_stringIntData.Count == 0)
				{
					string[] names = Enum.GetNames(_enumType);
					int[] array = Enum.GetValues(_enumType).Cast<int>().ToArray();
					for (int i = 0; i < names.Length; i++)
					{
						_stringIntData.Add(names[i], array[i]);
					}
				}
				return _stringIntData;
			}

			public List<object> GetEnumeratorsData()
			{
				if (_enumeratorsData.Count == 0)
				{
					string[] names = Enum.GetNames(_enumType);
					for (int i = 0; i < names.Length; i++)
					{
						_enumeratorsData.Add(Enum.Parse(_enumType, names[i]));
					}
				}
				return _enumeratorsData;
			}

			public Dictionary<string, object> GetStringEnumeratorData()
			{
				if (_stringEnumeratorData.Count == 0)
				{
					string[] names = Enum.GetNames(_enumType);
					for (int i = 0; i < names.Length; i++)
					{
						_stringEnumeratorData.Add(names[i], Enum.Parse(_enumType, names[i]));
					}
				}
				return _stringEnumeratorData;
			}

			public Dictionary<int, object> GetIntEnumeratorData()
			{
				if (_intEnumeratorData.Count == 0)
				{
					string[] names = Enum.GetNames(_enumType);
					for (int i = 0; i < names.Length; i++)
					{
						object obj = Enum.Parse(_enumType, names[i]);
						if (!_intEnumeratorData.ContainsKey((int)obj))
						{
							_intEnumeratorData.Add((int)obj, obj);
						}
					}
				}
				return _intEnumeratorData;
			}
		}

		private static EnumsCompliancer _instance;

		private readonly Dictionary<string, EnumData> _enumsData = new Dictionary<string, EnumData>();

		private static EnumsCompliancer Instance
		{
			get
			{
				return _instance ?? (_instance = new EnumsCompliancer());
			}
		}

		private static EnumData GetEnumData(Type enumType)
		{
			string key = enumType.ToString();
			if (!Instance._enumsData.ContainsKey(key))
			{
				Instance._enumsData.Add(key, new EnumData(enumType));
			}
			return Instance._enumsData[key];
		}

		public static Dictionary<int, string> GetEnumDictionary<T>() where T : struct, IConvertible
		{
			return typeof(T).IsEnum ? GetEnumData(typeof(T)).GetIntStringData() : null;
		}

		public static Dictionary<string, int> GetEnumDictionaryInverted<T>() where T : struct, IConvertible
		{
			return (!typeof(T).IsEnum) ? null : GetEnumData(typeof(T)).GetStringIntData();
		}

		public static List<T> GetEnumerators<T>() where T : struct, IConvertible
		{
			return (!typeof(T).IsEnum) ? null : GetEnumData(typeof(T)).GetEnumeratorsData().Cast<T>().ToList();
		}

		public static Dictionary<int, object> GetIntEnumeratorDictionary<T>() where T : struct, IConvertible
		{
			return (!typeof(T).IsEnum) ? null : GetEnumData(typeof(T)).GetIntEnumeratorData();
		}

		public static Dictionary<string, object> GetStringEnumeratorDictionary<T>() where T : struct, IConvertible
		{
			return (!typeof(T).IsEnum) ? null : GetEnumData(typeof(T)).GetStringEnumeratorData();
		}

		public static string GetEnumeratorName<T>(int enumeratorValue) where T : struct, IConvertible
		{
			Dictionary<int, string> enumDictionary = GetEnumDictionary<T>();
			return (enumDictionary == null || !enumDictionary.ContainsKey(enumeratorValue)) ? null : enumDictionary[enumeratorValue];
		}

		public static int? GetEnumeratorValue<T>(string enumeratorName) where T : struct, IConvertible
		{
			Dictionary<string, int> enumDictionaryInverted = GetEnumDictionaryInverted<T>();
			return (!enumDictionaryInverted.ContainsKey(enumeratorName)) ? null : new int?(enumDictionaryInverted[enumeratorName]);
		}

		public static T GetEnumerator<T>(string enumeratorName) where T : struct, IConvertible
		{
			Dictionary<string, object> stringEnumeratorData = GetEnumData(typeof(T)).GetStringEnumeratorData();
			return (!stringEnumeratorData.ContainsKey(enumeratorName)) ? default(T) : ((T)stringEnumeratorData[enumeratorName]);
		}

		public static T GetEnumerator<T>(int enumeratorValue) where T : struct, IConvertible
		{
			Dictionary<int, object> intEnumeratorData = GetEnumData(typeof(T)).GetIntEnumeratorData();
			return (!intEnumeratorData.ContainsKey(enumeratorValue)) ? default(T) : ((T)intEnumeratorData[enumeratorValue]);
		}

		public static void Clear()
		{
			if (_instance != null)
			{
				_instance._enumsData.Clear();
			}
		}
	}
}
