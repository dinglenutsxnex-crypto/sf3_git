using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public static class ConfigsSourceResolver
{
	[Serializable]
	private class SF3PreferencesPrimitive
	{
		public bool LocalConfigs;
	}

	public class ProxyObject
	{
		public delegate string UnzipToStringDelegate(string param);

		public delegate byte[] UnzipToBinaryDelegate(string param);

		private readonly UnzipToStringDelegate _unzipToStringDelegate;

		private readonly UnzipToBinaryDelegate _unzipToBinaryDelegate;

		public ProxyObject(UnzipToStringDelegate unzToStr, UnzipToBinaryDelegate unzToBin)
		{
			_unzipToBinaryDelegate = unzToBin;
			_unzipToStringDelegate = unzToStr;
		}

		public string UnzipToString(string path)
		{
			return _unzipToStringDelegate(path);
		}

		public T UnzipToJson<T>(string path) where T : class
		{
			string value = UnzipToString(path);
			if (!string.IsNullOrEmpty(value))
			{
				return JsonConvert.DeserializeObject<T>(value);
			}
			return (T)null;
		}

		public byte[] UnzipToBinary(string path)
		{
			return _unzipToBinaryDelegate(path);
		}
	}

	[Serializable]
	private class ConfigUnitsHolder
	{
		[Serializable]
		public class ConfigUnit
		{
			public string ArchivePath { get; set; }

			public string LocalPath { get; set; }

			public bool IsInternal { get; set; }

			public ConfigUnit FormatForLanguage(SystemLanguage language)
			{
				ConfigUnit configUnit = new ConfigUnit();
				configUnit.ArchivePath = string.Format(ArchivePath, language);
				configUnit.LocalPath = string.Format(LocalPath, language);
				configUnit.IsInternal = IsInternal;
				return configUnit;
			}
		}

		[SerializeField]
		public Dictionary<string, ConfigUnit> Configs { get; set; }

		public ConfigUnit this[string configName]
		{
			get
			{
				return GetConfig(configName);
			}
		}

		public ConfigUnit this[SystemLanguage language]
		{
			get
			{
				return GetConfig(language);
			}
		}

		private ConfigUnit GetConfig(string key)
		{
			if (ContainsKey(key))
			{
				return Configs[key];
			}
			return null;
		}

		private ConfigUnit GetConfig(SystemLanguage language)
		{
			if (ContainsKey("Localization"))
			{
				return Configs["Localization"].FormatForLanguage(language);
			}
			return null;
		}

		public bool ContainsKey(string configName)
		{
			return Configs.ContainsKey(configName);
		}

		public bool ContainsLocalization()
		{
			return Configs.ContainsKey("Localization");
		}
	}

	private static bool _useLocalSerrings;

	private static ConfigUnitsHolder _units;

	private static ProxyObject _proxyObject;

	public static string InternalSettings
	{
		get
		{
			return GetString("InternalSettings", true);
		}
	}

	public static byte[] SplitMoves
	{
		get
		{
			return GetBinary("SplitMoves");
		}
	}

	public static string TacticsSettings
	{
		get
		{
			return GetString("TacticsSettings");
		}
	}

	public static string Quests
	{
		get
		{
			return GetString("Quests");
		}
	}

	public static string FightSettings
	{
		get
		{
			return GetString("FightSettings");
		}
	}

	public static QualityPresets QualitySettings
	{
		get
		{
			return GetJson<QualityPresets>("QualitySettings");
		}
	}

	public static void Init(ProxyObject proxy)
	{
		_proxyObject = proxy;
		_useLocalSerrings = false;
		if (Application.isEditor)
		{
			SF3PreferencesPrimitive sF3PreferencesPrimitive = FilesUtil.ReadFileJson<SF3PreferencesPrimitive>("EditorTools.json");
			if (sF3PreferencesPrimitive != null)
			{
				_useLocalSerrings = sF3PreferencesPrimitive.LocalConfigs;
			}
		}
		_units = GlobalLoad.GetLoadJson<ConfigUnitsHolder>("Configs/configSources");
	}

	public static string GetString(string configName, bool forceLocal = false)
	{
		if (_units.ContainsKey(configName))
		{
			ConfigUnitsHolder.ConfigUnit configUnit = _units[configName];
			string text = _proxyObject.UnzipToString(configUnit.ArchivePath);
			if (text == null || _useLocalSerrings || forceLocal)
			{
				if (configUnit.IsInternal)
				{
					return GlobalLoad.GetLoadTextInternal(configUnit.LocalPath, string.Empty);
				}
				return GlobalLoad.GetLoadText(configUnit.LocalPath);
			}
			return text;
		}
		return null;
	}

	public static string GetLocalization(SystemLanguage language)
	{
		if (_units.ContainsLocalization())
		{
			ConfigUnitsHolder.ConfigUnit configUnit = _units[language];
			string text = _proxyObject.UnzipToString(configUnit.ArchivePath);
			if (text == null || _useLocalSerrings)
			{
				if (configUnit.IsInternal)
				{
					return GlobalLoad.GetLoadTextInternal(configUnit.LocalPath, string.Empty);
				}
				return GlobalLoad.GetLoadText(configUnit.LocalPath);
			}
			return text;
		}
		return null;
	}

	public static T GetJson<T>(string configName) where T : class
	{
		if (_units.ContainsKey(configName))
		{
			ConfigUnitsHolder.ConfigUnit configUnit = _units[configName];
			T val = _proxyObject.UnzipToJson<T>(configUnit.ArchivePath);
			if (val == null || _useLocalSerrings)
			{
				if (configUnit.IsInternal)
				{
					return GlobalLoad.GetLoadJsonInternal<T>(configUnit.LocalPath, string.Empty);
				}
				return GlobalLoad.GetLoadJson<T>(configUnit.LocalPath);
			}
			return val;
		}
		return (T)null;
	}

	public static byte[] GetBinary(string configName)
	{
		if (_units.ContainsKey(configName))
		{
			ConfigUnitsHolder.ConfigUnit configUnit = _units[configName];
			byte[] array = _proxyObject.UnzipToBinary(configUnit.ArchivePath);
			if (array == null || _useLocalSerrings)
			{
				if (configUnit.IsInternal)
				{
					return GlobalLoad.GetLoadBytesInternal(configUnit.LocalPath, string.Empty);
				}
				return GlobalLoad.GetLoadBytes(configUnit.LocalPath);
			}
			return array;
		}
		return null;
	}
}
