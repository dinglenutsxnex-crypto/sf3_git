using System;
using System.Collections.Generic;
using System.Text;

public static class NekkiConsole
{
	public delegate string CommandFunction(params string[] args);

	private const string _CommandArgsSeparator = " ";

	private static readonly char[] _ArgsSeparators = new char[1] { ' ' };

	private static readonly Dictionary<string, CommandFunction> _Commands = new Dictionary<string, CommandFunction>();

	private static StringBuilder _descriptions = new StringBuilder();

	public static string Descriptions
	{
		get
		{
			return _descriptions.ToString();
		}
	}

	public static bool HasCommand(string name)
	{
		name = name.ToLower();
		return _Commands.ContainsKey(name) && _Commands[name] != null;
	}

	public static string ExecuteCommand(string command)
	{
		if (!string.IsNullOrEmpty(command))
		{
			command = command.ToLower().Trim(' ');
			int num = ((!command.Contains(" ")) ? command.Length : command.IndexOf(" ", StringComparison.Ordinal));
			string text = command.Substring(0, num);
			command = command.Remove(0, num);
			command = command.Trim(' ');
			string[] args = command.Split(_ArgsSeparators, StringSplitOptions.RemoveEmptyEntries);
			if (HasCommand(text))
			{
				return _Commands[text](args);
			}
		}
		return "Unknown command!";
	}

	public static void RegisterCommand(string name, CommandFunction commandFunction, string description = "")
	{
		_Commands[name.ToLower()] = commandFunction;
		if (description.Length > 0)
		{
			_descriptions.AppendLine(description);
		}
		else
		{
			_descriptions.AppendLine(name);
		}
	}

	public static void UnregisterCommand(string name)
	{
		_Commands.Remove(name.ToLower());
	}
}
