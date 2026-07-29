using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class FilesUtil
{
	public static void WriteFileBytes(string path, byte[] bytes)
	{
		CreateDirectory(Path.GetDirectoryName(path));
		File.WriteAllBytes(path, bytes);
	}

	public static void WriteFileText(string path, string content = "")
	{
		AppendFileText(path, content, true);
	}

	public static void AppendFileText(string path, string content = "", bool isReplace = false)
	{
		CreateDirectory(Path.GetDirectoryName(path));
		if (!isReplace && IsFileExists(path))
		{
			File.AppendAllText(path, content);
		}
		else
		{
			File.WriteAllText(path, content);
		}
	}

	public static void CreateTextWriter(string path, Action<TextWriter> action, bool append = false)
	{
		using (TextWriter value = CreateTextWriter(path, append))
		{
			action.InvokeSafe(value);
		}
	}

	public static TextWriter CreateTextWriter(string path, bool append = false)
	{
		CreateDirectory(Path.GetDirectoryName(path));
		return new StreamWriter(path, append, Encoding.UTF8);
	}

	public static FileStream AppendStreamWriter(string path)
	{
		FileMode mode = ((!IsFileExists(path)) ? FileMode.OpenOrCreate : FileMode.Append);
		return CreateStreamWriter(path, mode);
	}

	public static void CreateStreamWriter(string path, Action<FileStream> action, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.Write)
	{
		using (FileStream value = CreateStreamWriter(path, mode, access))
		{
			action.InvokeSafe(value);
		}
	}

	public static FileStream CreateStreamWriter(string path, FileMode mode = FileMode.OpenOrCreate, FileAccess access = FileAccess.Write)
	{
		CreateDirectory(Path.GetDirectoryName(path));
		return new FileStream(path, mode, access);
	}

	public static void SaveJSON(string path, object obj, Formatting formatting = Formatting.None)
	{
		try
		{
			string content = JsonConvert.SerializeObject(obj, formatting);
			WriteFileText(path, content);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error SaveJSON config [" + ex.Message + "]");
		}
	}

	public static T ReadFileJson<T>(string path) where T : class
	{
		string value = ReadFileText(path);
		if (!value.IsNullOrEmpty())
		{
			try
			{
				return JsonConvert.DeserializeObject<T>(value);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error ReadFileJson [" + path + " " + ex.Message + "]");
			}
		}
		return (T)null;
	}

	public static string ReadFileText(string path)
	{
		if (IsFileExists(path))
		{
			return File.ReadAllText(path);
		}
		return null;
	}

	public static string[] ReadFileLines(string path)
	{
		if (IsFileExists(path))
		{
			return File.ReadAllLines(path);
		}
		return null;
	}

	public static byte[] ReadFileBytes(string path)
	{
		if (IsFileExists(path))
		{
			return File.ReadAllBytes(path);
		}
		return null;
	}

	public static bool IsFileExists(string path)
	{
		return File.Exists(path);
	}

	public static bool IsDirectoryExists(string path)
	{
		return Directory.Exists(path);
	}

	public static bool IsFileLocked(string path)
	{
		FileInfo fileInfo = new FileInfo(path);
		FileStream fileStream = null;
		try
		{
			fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
		}
		catch (FileNotFoundException)
		{
			return false;
		}
		catch (IOException)
		{
			return true;
		}
		finally
		{
			if (fileStream != null)
			{
				fileStream.Close();
			}
		}
		return false;
	}

	public static bool DeleteFile(string path)
	{
		if (IsFileExists(path) && !IsFileLocked(path))
		{
			File.Delete(path);
			return true;
		}
		return false;
	}

	public static bool MoveFiles(string pathfrom, string pathto)
	{
		if (IsFileExists(pathfrom) && !IsFileLocked(pathfrom))
		{
			DeleteFile(pathto);
			File.Move(pathfrom, pathto);
			return true;
		}
		return false;
	}

	public static bool CopyFile(string pathfrom, string pathto)
	{
		if (IsFileExists(pathfrom))
		{
			CreateDirectory(Path.GetDirectoryName(pathto));
			File.Copy(pathfrom, pathto);
			return true;
		}
		return false;
	}

	public static bool DeleteDirectory(string path)
	{
		if (IsDirectoryExists(path))
		{
			Directory.Delete(path, true);
			return true;
		}
		return false;
	}

	public static void CreateDirectory(string path)
	{
		if (!IsDirectoryExists(path))
		{
			Directory.CreateDirectory(path);
		}
	}
}
