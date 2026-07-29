public class VersionContainer : VariedVersionContainer
{
	public static readonly VersionContainer Zero = new VersionContainer();

	public int Production
	{
		get
		{
			return _versionSource[0];
		}
		set
		{
			_versionSource[0] = value;
		}
	}

	public int Major
	{
		get
		{
			return _versionSource[1];
		}
		set
		{
			_versionSource[1] = value;
		}
	}

	public int Minor
	{
		get
		{
			return _versionSource[2];
		}
		set
		{
			_versionSource[2] = value;
		}
	}

	public int DataVersion
	{
		get
		{
			return _versionSource[3];
		}
		set
		{
			_versionSource[3] = value;
		}
	}

	public VersionContainer()
		: base(4)
	{
		SetVersion(0, 0, 0, 0);
	}

	public VersionContainer(string version)
		: base(4)
	{
		SetVersion(version);
	}

	public VersionContainer(int production, int major, int minor, int dataVersion = -1)
		: base(4)
	{
		SetVersion(production, major, minor, dataVersion);
	}

	public void SetVersion(VersionContainer version)
	{
		SetVersion(version.Production, version.Major, version.Minor, version.DataVersion);
	}

	public void SetVersion(int production, int major = -1, int minor = -1, int dataVersion = -1)
	{
		Production = production;
		Major = major;
		Minor = minor;
		DataVersion = dataVersion;
	}

	public static VersionContainer CreateVersion(string version)
	{
		return new VersionContainer(version);
	}

	public static VersionContainer CreateVersion(VersionContainer version)
	{
		return new VersionContainer(version.Production, version.Major, version.Minor, version.DataVersion);
	}

	public static VersionContainer CreateVersion(int production, int major = -1, int minor = -1, int dataVersion = -1)
	{
		return new VersionContainer(production, major, minor, dataVersion);
	}

	public string ToString(bool isDataVersion)
	{
		if (isDataVersion)
		{
			return string.Format("{0}.{1}.{2}.{3}", Production, Major, Minor, DataVersion);
		}
		return string.Format("{0}.{1}.{2}", Production, Major, Minor);
	}

	public override string ToString()
	{
		return ToString(false);
	}

	public static implicit operator string(VersionContainer v)
	{
		return v.ToString(true);
	}

	public bool Empty(bool isDataVersion = false)
	{
		if (Production == 0 && Major == 0 && Minor == 0 && (!isDataVersion || DataVersion == 0))
		{
			return true;
		}
		return false;
	}

	public bool ForCurrentVersion(string dataVersion)
	{
		return ForCurrentVersion(new VersionContainer(dataVersion));
	}

	public bool ForCurrentVersion(VersionContainer dataVersion)
	{
		return VariedVersionContainer.Compare(this, dataVersion, 3) == EqualityVersion.Equally;
	}

	public bool ForNextVersions(VersionContainer dataVersion)
	{
		return VariedVersionContainer.Compare(this, dataVersion, 3) == EqualityVersion.Less;
	}
}
