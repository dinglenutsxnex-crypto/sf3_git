using System;
using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
	public delegate void LanguageSwitchEventHandler();

	private static Func<string, LocaleImport.LocaleString> OverridenGetLocaleFunc;

	private static readonly Dictionary<SystemLanguage, Dictionary<string, LocaleImport.LocaleString>> Data;

	public static SystemLanguage Language { get; private set; }

	public static SystemLanguage DefaultLanguage { get; private set; }

	public static Dictionary<string, LocaleImport.LocaleString> GetCurrentLocaleData
	{
		get
		{
			if (Data.ContainsKey(Language))
			{
				return Data[Language];
			}
			if (Data.ContainsKey(DefaultLanguage))
			{
				return Data[DefaultLanguage];
			}
			return null;
		}
	}

	public static event LanguageSwitchEventHandler LanguageSwitched;

	static Localization()
	{
		Data = new Dictionary<SystemLanguage, Dictionary<string, LocaleImport.LocaleString>>();
		Language = LoadProp("CurrentLanguage", Application.systemLanguage);
		DefaultLanguage = LoadProp("DefaultLanguage", SystemLanguage.English);
		InitLocales();
	}

	public static void Init(SystemLanguage? language, SystemLanguage? defaultLanguage = null, Func<string, LocaleImport.LocaleString> overridenGetLocaleFunc = null)
	{
		OverridenGetLocaleFunc = overridenGetLocaleFunc;
		if (language.HasValue && (Language != language.GetValueOrDefault() || !language.HasValue))
		{
			Language = language.Value;
			SaveProp("CurrentLanguage", Language);
			Import(Language);
			OnLanguageSwitched();
		}
		if (defaultLanguage.HasValue && DefaultLanguage != defaultLanguage.Value)
		{
			DefaultLanguage = defaultLanguage.Value;
			SaveProp("DefaultLanguage", DefaultLanguage);
			Import(DefaultLanguage);
			if (!Data.ContainsKey(Language))
			{
				OnLanguageSwitched();
			}
		}
	}

	public static void Reset()
	{
		Data.Clear();
		InitLocales();
	}

	private static void OnLanguageSwitched()
	{
		LanguageSwitchEventHandler languageSwitched = Localization.LanguageSwitched;
		if (languageSwitched != null)
		{
			languageSwitched();
		}
	}

	private static SystemLanguage LoadProp(string alias, SystemLanguage def)
	{
		if (PlayerPrefs.HasKey(alias))
		{
			return (SystemLanguage)Enum.Parse(typeof(SystemLanguage), PlayerPrefs.GetString(alias));
		}
		return def;
	}

	private static void SaveProp(string alias, SystemLanguage value)
	{
		PlayerPrefs.SetString(alias, value.ToString());
		PlayerPrefs.Save();
	}

	private static void InitLocales()
	{
		Import(Language);
		Import(DefaultLanguage);
		OnLanguageSwitched();
	}

	private static void Import(SystemLanguage language)
	{
		if (!Data.ContainsKey(language))
		{
			LocaleImport localeImport = new LocaleImport(language);
			Data.Add(localeImport, localeImport);
		}
	}

	public static LocaleImport.LocaleString Get(string key)
	{
		if (!Data.ContainsKey(Language))
		{
			return new LocaleImport.LocaleString(string.Format("err_{0}", Language));
		}
		LocaleImport.LocaleString localeString = ((OverridenGetLocaleFunc != null) ? OverridenGetLocaleFunc(key) : GetLocaleDefault(key));
		if (localeString == null)
		{
			return new LocaleImport.LocaleString(string.Format("err_{0}", key));
		}
		return localeString;
	}

	public static LocaleImport.LocaleString Get(string key, int symbolsInLine, string[] lastReplacement)
	{
		LocaleImport.LocaleString localeString = Get(key);
		return localeString.SplitByRows(symbolsInLine, lastReplacement);
	}

	public static LocaleImport.LocaleString GetLocale(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return null;
		}
		if (Data[Language].ContainsKey(key))
		{
			return Data[Language][key];
		}
		if (Data[DefaultLanguage].ContainsKey(key))
		{
			return Data[DefaultLanguage][key];
		}
		return null;
	}

	public static bool HasKey(string key)
	{
		return Data[Language].ContainsKey(key) || Data[DefaultLanguage].ContainsKey(key);
	}

	private static LocaleImport.LocaleString GetLocaleDefault(string key)
	{
		return GetLocale(key);
	}

	public static string Format(string key, params object[] replacements)
	{
		return string.Format(Get(key), replacements);
	}

	public static bool Contains(string alias, bool checkInDefaultLanguage = false)
	{
		return (Data.ContainsKey(Language) && Data[Language].ContainsKey(alias)) || (checkInDefaultLanguage && Data.ContainsKey(DefaultLanguage) && Data[DefaultLanguage].ContainsKey(alias));
	}

	public static void SetTargetLanguage(SystemLanguage language)
	{
		if (Language != language)
		{
			Language = language;
			InitLocales();
			SaveProp("CurrentLanguage", Language);
		}
	}

	public static void SetDefaultLanguage(SystemLanguage language)
	{
		if (DefaultLanguage != language)
		{
			DefaultLanguage = language;
			InitLocales();
			SaveProp("DefaultLanguage", DefaultLanguage);
		}
	}
}
