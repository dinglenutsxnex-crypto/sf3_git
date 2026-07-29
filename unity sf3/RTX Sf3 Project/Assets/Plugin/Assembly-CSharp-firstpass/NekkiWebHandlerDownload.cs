using System.IO;
using UnityEngine;

public class NekkiWebHandlerDownload : NekkiWebHandler
{
	private readonly string _savePath;

	private readonly string _saveDownloadPath;

	private readonly FileMode _fileMode;

	private readonly FileStream _file;

	public NekkiWebHandlerDownload(NekkiUri uriRequest, string pathRequest, string downloadPathRequest)
		: base(uriRequest)
	{
		_savePath = pathRequest;
		_saveDownloadPath = downloadPathRequest;
		_file = FilesUtil.AppendStreamWriter(_saveDownloadPath);
	}

	protected override void SetBytes(byte[] data, int dataOffset, int dataLength)
	{
		_file.Write(data, 0, dataLength);
	}

	public override void Abort()
	{
		CloseFile();
		base.Abort();
	}

	protected override void SetCompleteContent()
	{
		CloseFile();
		if (FilesUtil.IsFileExists(_savePath) && FilesUtil.IsFileLocked(_savePath))
		{
			Debug.LogError(string.Format("This file is locked: {0} User may have this file open (as a folder or 7z), or there is a bug in the code still holding an open file handler", _savePath));
			return;
		}
		FilesUtil.DeleteFile(_savePath);
		FilesUtil.MoveFiles(_saveDownloadPath, _savePath);
	}

	private void CloseFile()
	{
		_file.Close();
	}
}
