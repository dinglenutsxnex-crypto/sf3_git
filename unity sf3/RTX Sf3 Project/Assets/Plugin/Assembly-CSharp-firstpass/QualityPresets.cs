using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class QualityPresets
{
	public Dictionary<string, DevicePreset> Presets = new Dictionary<string, DevicePreset>();

	public Dictionary<string, DeviceQuality> Devices = new Dictionary<string, DeviceQuality>();

	public Dictionary<string, DeviceCondition> Conditions = new Dictionary<string, DeviceCondition>();

	public void InitPreset()
	{
		foreach (KeyValuePair<string, DevicePreset> preset in Presets)
		{
			preset.Value.name = preset.Key;
		}
		foreach (KeyValuePair<string, DeviceQuality> device in Devices)
		{
			device.Value.preset = GetPresetByName(device.Value.presetName);
		}
		foreach (KeyValuePair<string, DeviceCondition> condition in Conditions)
		{
			condition.Value.preset = GetPresetByName(condition.Value.presetName);
		}
	}

	public DevicePreset GetConditionPreset()
	{
		string platformName = GetRuntimePlatform().ToLower();
		int processorCount = SystemInfo.processorCount;
		int systemMemorySize = SystemInfo.systemMemorySize;
		DeviceCondition deviceCondition = null;
		foreach (KeyValuePair<string, DeviceCondition> condition in Conditions)
		{
			if (condition.Value.Equal(platformName, processorCount, systemMemorySize) && (deviceCondition == null || deviceCondition.priority < condition.Value.priority))
			{
				deviceCondition = condition.Value;
			}
		}
		return (deviceCondition == null) ? null : deviceCondition.preset;
	}

	public DeviceQuality GetDevice()
	{
		string text = SystemInfo.deviceModel.ToLower();
		if (!text.IsNullOrEmpty() && Devices.ContainsKey(text))
		{
			return Devices[text];
		}
		return null;
	}

	public DevicePreset GetPresetByName(string name)
	{
		if (!name.IsNullOrEmpty() && Presets.ContainsKey(name))
		{
			return Presets[name];
		}
		return null;
	}

	public QualityDeviceTypes GetConditionDeviceType()
	{
		if (Screen.dpi > 0f)
		{
			float f = (float)Screen.width / Screen.dpi;
			float f2 = (float)Screen.height / Screen.dpi;
			float num = Mathf.Sqrt(Mathf.Pow(f, 2f) + Mathf.Pow(f2, 2f));
			if (num > 6.5f)
			{
				return QualityDeviceTypes.TABLET;
			}
		}
		else if ((float)Screen.height > 768f)
		{
			return QualityDeviceTypes.TABLET;
		}
		return QualityDeviceTypes.PHONE;
	}

	private string GetRuntimePlatform()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.Android:
			return "android";
		case RuntimePlatform.IPhonePlayer:
			return "ios";
		case RuntimePlatform.WindowsEditor:
			return "windows";
		case RuntimePlatform.WindowsPlayer:
			return "windows";
		case RuntimePlatform.OSXPlayer:
			return "osx";
		case RuntimePlatform.OSXEditor:
			return "osx";
		default:
			return string.Empty;
		}
	}

	public override string ToString()
	{
		return JsonConvert.SerializeObject(this, Formatting.Indented);
	}
}
