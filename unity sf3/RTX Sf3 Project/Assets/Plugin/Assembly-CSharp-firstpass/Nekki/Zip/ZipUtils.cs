using System.Collections.Generic;
using System.IO;
using Ionic.Crc;
using Ionic.Zip;
using Ionic.Zlib;
using UnityEngine;
using CompressionLevel = Ionic.Zlib.CompressionLevel;

namespace Nekki.Zip
{
	public static class ZipUtils
	{
		public static Dictionary<string, string> UnzipToStrings(string path, List<string> fileNames = null)
		{
			ZipFile zipFile = new ZipFile(path);
			return UnzipToStrings(zipFile, fileNames);
		}

		public static Dictionary<string, string> UnzipToStrings(Stream stream, List<string> fileNames = null)
		{
			ZipFile zipFile = ZipFile.Read(stream);
			return UnzipToStrings(zipFile, fileNames);
		}

		private static Dictionary<string, string> UnzipToStrings(ZipFile zipFile, List<string> fileNames = null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (ZipEntry item in zipFile)
			{
				if (fileNames == null || fileNames.Contains(item.FileName))
				{
					CrcCalculatorStream stream = item.OpenReader();
					using (StreamReader streamReader = new StreamReader(stream))
					{
						dictionary.Add(item.FileName, streamReader.ReadToEnd());
					}
				}
			}
			zipFile.Dispose();
			return dictionary;
		}

		public static string UnzipToString(string path, string filename)
		{
			List<string> list = new List<string>();
			list.Add(filename);
			List<string> fileNames = list;
			Dictionary<string, string> dictionary = UnzipToStrings(path, fileNames);
			if (dictionary.ContainsKey(filename))
			{
				return dictionary[filename];
			}
			Debug.LogError("Unzip File not found - " + filename);
			return null;
		}

		public static string UnzipToString(Stream stream, string filename)
		{
			List<string> list = new List<string>();
			list.Add(filename);
			List<string> fileNames = list;
			Dictionary<string, string> dictionary = UnzipToStrings(stream, fileNames);
			return dictionary[filename];
		}

		public static Dictionary<string, Stream> UnzipToStreams(string path, List<string> fileNames = null)
		{
			Dictionary<string, Stream> dictionary = new Dictionary<string, Stream>();
			ZipFile zipFile = new ZipFile(path);
			foreach (ZipEntry item in zipFile)
			{
				if (fileNames == null || fileNames.Contains(item.FileName))
				{
					CrcCalculatorStream value = item.OpenReader();
					dictionary.Add(item.FileName, value);
				}
			}
			zipFile.Dispose();
			return dictionary;
		}

		public static byte[] UnzipToBinary(string path, string filename)
		{
			List<string> list = new List<string>();
			list.Add(filename);
			List<string> fileNames = list;
			Dictionary<string, byte[]> dictionary = UnzipToBinaries(path, fileNames);
			if (dictionary.ContainsKey(filename))
			{
				return dictionary[filename];
			}
			Debug.LogError("Unzip File not found - " + filename);
			return null;
		}

		public static Dictionary<string, byte[]> UnzipToBinaries(string path, List<string> fileNames = null)
		{
			Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
			ZipFile zipFile = new ZipFile(path);
			foreach (ZipEntry item in zipFile)
			{
				if (fileNames == null || fileNames.Contains(item.FileName))
				{
					CrcCalculatorStream input = item.OpenReader();
					dictionary.Add(item.FileName, ReadStreamToBinary(input));
				}
			}
			zipFile.Dispose();
			return dictionary;
		}

		public static byte[] ReadStreamToBinary(Stream input)
		{
			byte[] array = new byte[16384];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				int count;
				while ((count = input.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, count);
				}
				return memoryStream.ToArray();
			}
		}

		public static void SaveZipFromDirectory(string pathDirectory, string savePath, CompressionLevel level = CompressionLevel.Default)
		{
			ZipFile zipFile = new ZipFile();
			zipFile.CompressionLevel = level;
			zipFile.AddDirectory(pathDirectory);
			zipFile.Save(savePath);
		}
	}
}
