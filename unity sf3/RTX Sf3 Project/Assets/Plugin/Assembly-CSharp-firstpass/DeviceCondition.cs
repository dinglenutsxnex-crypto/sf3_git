using System;

[Serializable]
public class DeviceCondition
{
	[NonSerialized]
	public string name;

	[NonSerialized]
	public DevicePreset preset;

	public string presetName;

	public int priority;

	public string platform;

	public int core;

	public int memory;

	public int resoluitonX;

	public int resoluitonY;

	public DeviceCondition()
	{
		name = string.Empty;
		priority = int.MinValue;
		platform = string.Empty;
		core = 0;
		memory = 0;
		resoluitonX = 0;
		resoluitonY = 0;
	}

	public bool Equal(string platformName, int coreCount, int memorySize)
	{
		return (platform.Equals(string.Empty) || platform.Equals(platformName)) && core < coreCount && memory < memorySize;
	}
}
