using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandLineReader
{
	private const string CUSTOM_ARGS_PREFIX = "-CustomArgs:";

	private const char CUSTOM_ARGS_SEPARATOR = ';';

	public static string[] GetCommandLineArgs()
	{
		return Environment.GetCommandLineArgs();
	}

	public static string GetCommandLine()
	{
		string[] commandLineArgs = GetCommandLineArgs();
		if (commandLineArgs.Length > 0)
		{
			return string.Join(" ", commandLineArgs);
		}
		Debug.LogError("CommandLineReader.cs - GetCommandLine() - Can't find any command line arguments!");
		return string.Empty;
	}

	public static Dictionary<string, string> GetCustomArguments()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		string[] commandLineArgs = GetCommandLineArgs();
		string empty = string.Empty;
		try
		{
			empty = commandLineArgs.Where((string row) => row.Contains("-CustomArgs:")).Single();
		}
		catch (Exception)
		{
			return dictionary;
		}
		empty = empty.Replace("-CustomArgs:", string.Empty);
		string[] array = empty.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(new char[1] { '=' }, 2);
			if (array3.Length == 2)
			{
				array3[1] = array3[1].Replace("$", "=");
				dictionary.Add(array3[0], array3[1]);
			}
		}
		return dictionary;
	}

	public static string GetCustomArgument(string argumentName)
	{
		Dictionary<string, string> customArguments = GetCustomArguments();
		if (customArguments.ContainsKey(argumentName))
		{
			return customArguments[argumentName];
		}
		return string.Empty;
	}
}
