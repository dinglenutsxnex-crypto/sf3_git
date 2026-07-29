using System;
using System.IO;
using JetBrains.Annotations;

public class NekkiUri : Uri
{
	private readonly string _fileName;

	private readonly string _fileFullName;

	private readonly string _fileExtension;

	public string FileName
	{
		get
		{
			return _fileName;
		}
	}

	public string FileFullName
	{
		get
		{
			return _fileFullName;
		}
	}

	public string FileExtension
	{
		get
		{
			return _fileExtension;
		}
	}

	public NekkiUri([NotNull] string uriString)
		: base(uriString)
	{
		_fileName = Path.GetFileNameWithoutExtension(uriString);
		_fileFullName = Path.GetFileName(base.LocalPath);
		_fileExtension = Path.GetExtension(base.LocalPath);
	}

	public override string ToString()
	{
		return "NekkiUri [" + base.OriginalString + "]";
	}
}
