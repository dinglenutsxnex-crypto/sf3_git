using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeviceQuality
{
	public const float TABLET_DPI = 6.5f;

	public const float TABLET_RESOLUTION = 768f;

	[NonSerialized]
	public string name;

	public string presetName;

	public QualityDeviceTypes type;

	[NonSerialized]
	public DevicePreset preset;

	private static Dictionary<string, QualityDeviceTypes> _types;

	public static Dictionary<string, QualityDeviceTypes> types
	{
		get
		{
			if (_types == null)
			{
				_types = new Dictionary<string, QualityDeviceTypes>();
				_types.Add("phone", QualityDeviceTypes.PHONE);
				_types.Add("tablet", QualityDeviceTypes.TABLET);
				_types.Add("console", QualityDeviceTypes.CONSOLE);
				_types.Add("standalone", QualityDeviceTypes.STANDALONE);
			}
			return _types;
		}
	}

	public DeviceQuality()
	{
		name = SystemInfo.deviceModel.ToLower();
		preset = new DevicePreset();
		type = QualityDeviceTypes.TABLET;
	}

	public void SetType(string type)
	{
		this.type = GetType(type);
	}

	public static QualityDeviceTypes GetType(string type)
	{
		if (types.ContainsKey(type))
		{
			return types[type];
		}
		return QualityDeviceTypes.TABLET;
	}

	public static string GetType(QualityDeviceTypes value)
	{
		foreach (KeyValuePair<string, QualityDeviceTypes> type in types)
		{
			if (type.Value == value)
			{
				return type.Key;
			}
		}
		return string.Empty;
	}
}
