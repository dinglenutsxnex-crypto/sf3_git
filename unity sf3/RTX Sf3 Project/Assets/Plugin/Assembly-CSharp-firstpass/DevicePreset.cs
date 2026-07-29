using System;

[Serializable]
public class DevicePreset
{
	[NonSerialized]
	public string name;

	[NonSerialized]
	public SubTypes subType;

	public int priority;

	public TextureResolutionTypes textureResolution;

	public ShadowsTypes shadows;

	public bool shadowFormGlow;

	public bool anisotropicFiltering;

	public bool antiAliasing;

	public bool cloth;

	public bool dissolve;

	public bool saturation;

	public bool gamma;

	public bool matcap;

	public int fpsCap;

	public DevicePreset()
	{
		name = "low";
		subType = SubTypes.CONDITION;
		priority = 0;
		shadows = ShadowsTypes.OFF;
		shadowFormGlow = false;
		textureResolution = TextureResolutionTypes.QUARTER;
		anisotropicFiltering = false;
		antiAliasing = false;
		saturation = true;
		gamma = true;
		cloth = true;
		dissolve = true;
	}

	public bool IsPriority(DevicePreset preset)
	{
		return priority > preset.priority;
	}
}
