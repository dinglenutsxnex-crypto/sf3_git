using System;
using YamlDotNet.Serialization.Utilities;

public class VariedVersionContainer
{
	public enum EqualityVersion
	{
		Equally = 0,
		More = 1,
		Less = 2
	}

	protected readonly int[] _versionSource;

	public VariedVersionContainer(int numberOfVersions)
	{
		_versionSource = new int[numberOfVersions];
		SetAllAsZero();
	}

	public VariedVersionContainer(string version)
	{
		int num = version.CountChars('.');
		_versionSource = new int[num + 1];
		SetVersion(version);
	}

	public void SetVersion(string version)
	{
		version = version.Trim();
		if (string.IsNullOrEmpty(version))
		{
			SetAllAsZero();
			return;
		}
		string[] array = version.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries);
		SetAllAsZero();
		try
		{
			for (int i = 0; i < array.Length; i++)
			{
				_versionSource[i] = int.Parse(array[i]);
			}
		}
		catch (Exception)
		{
			SetAllAsZero();
		}
	}

	private static VariedVersionContainer CreateVersion(string version)
	{
		return new VariedVersionContainer(version);
	}

	protected void SetAllAsZero()
	{
		for (int i = 0; i < _versionSource.Length; i++)
		{
			_versionSource[i] = 0;
		}
	}

	public static bool operator ==(VariedVersionContainer a, VariedVersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.Equally;
	}

	public static bool operator !=(VariedVersionContainer a, VariedVersionContainer b)
	{
		return !(a == b);
	}

	public static bool operator >(VariedVersionContainer a, VariedVersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.More && equalityVersion != EqualityVersion.Equally;
	}

	public static bool operator <(VariedVersionContainer a, VariedVersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.Less && equalityVersion != EqualityVersion.Equally;
	}

	public static bool operator >=(VariedVersionContainer a, VariedVersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.More || equalityVersion == EqualityVersion.Equally;
	}

	public static bool operator >=(VariedVersionContainer a, string b)
	{
		return a >= CreateVersion(b);
	}

	public static bool operator <=(VariedVersionContainer a, VariedVersionContainer b)
	{
		EqualityVersion equalityVersion = Compare(a, b);
		return equalityVersion == EqualityVersion.Less || equalityVersion == EqualityVersion.Equally;
	}

	public static bool operator <=(VariedVersionContainer a, string b)
	{
		return a <= CreateVersion(b);
	}

	public override bool Equals(object obj)
	{
		return obj is VariedVersionContainer && (VariedVersionContainer)obj == this;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public static EqualityVersion Compare(VariedVersionContainer a, VariedVersionContainer b, int? _dimensions = null)
	{
		int num = Math.Min(a._versionSource.Length, b._versionSource.Length);
		if (_dimensions.HasValue)
		{
			num = _dimensions.GetValueOrDefault();
		}
		if (num > a._versionSource.Length || num > b._versionSource.Length)
		{
			throw new IndexOutOfRangeException("Error: diminsions too big");
		}
		int[] versionSource = a._versionSource;
		int[] versionSource2 = b._versionSource;
		for (int i = 0; i < num; i++)
		{
			if (versionSource[i] != versionSource2[i])
			{
				return (versionSource[i] > versionSource2[i]) ? EqualityVersion.More : EqualityVersion.Less;
			}
		}
		return EqualityVersion.Equally;
	}

	public string GetVersionToString(int length = 0)
	{
		if (length == 0)
		{
			length = _versionSource.Length;
		}
		string text = string.Empty;
		for (int i = 0; i < length; i++)
		{
			text = text + _versionSource[i] + ((i >= length - 1) ? string.Empty : ".");
		}
		return text;
	}

	public override string ToString()
	{
		return "VariedVersionContainer [version: " + GetVersionToString() + "]";
	}
}
