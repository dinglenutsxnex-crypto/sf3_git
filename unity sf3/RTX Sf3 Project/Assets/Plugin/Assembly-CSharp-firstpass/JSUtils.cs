using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class JSUtils
{
	public static string JSToJson(string jsContents, string rootKey, string[] skippedNames, string[] skippedClasses)
	{
		char[] delims = new char[5] { '{', '}', '[', ']', ',' };
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("{");
		stringBuilder.AppendLine("\"" + rootKey + "\": {");
		bool flag = false;
		string[] array = jsContents.Split('\n');
		int num = ((!(array[array.Length - 1].Trim() == "};")) ? (array.Length - 2) : (array.Length - 1));
		for (int i = 1; i < num; i++)
		{
			string text = array[i].Trim();
			if (string.IsNullOrEmpty(text) || IsComment(text) || IsSkippingName(text, skippedNames))
			{
				continue;
			}
			if (flag)
			{
				if (text.Equals("],"))
				{
					flag = false;
				}
				continue;
			}
			if (skippedClasses.Contains(GetJsVariableName(text)))
			{
				flag = true;
				continue;
			}
			IEnumerable<string> enumerable = array[i].SplitAndKeep(delims);
			foreach (string item in enumerable)
			{
				text = item.Trim();
				if (text.Contains(":"))
				{
					text = AddQuotes(text);
				}
				stringBuilder.AppendLine(text);
			}
		}
		stringBuilder.AppendLine("}");
		return stringBuilder.ToString();
	}

	private static bool IsComment(string line)
	{
		if (line.StartsWith("//"))
		{
			return true;
		}
		return false;
	}

	private static bool IsSkippingName(string line, string[] names)
	{
		return names.Contains(GetJsVariableName(line));
	}

	public static string GetJsVariableName(string line)
	{
		int num = line.IndexOf(':');
		if (num < 0)
		{
			return string.Empty;
		}
		return line.Substring(0, num);
	}

	private static string AddQuotes(string line)
	{
		int num = line.IndexOf(':');
		string text = AddQuotesIfNeeded(line.Substring(0, num)) + ":";
		int num2 = line.Length - num - 1;
		if (num2 >= 0)
		{
			return text + AddQuotesIfNeeded(line.Substring(num + 1, num2).Trim());
		}
		return text;
	}

	private static string AddQuotesIfNeeded(string str)
	{
		string text = "\"";
		if (string.IsNullOrEmpty(str) && str.StartsWith(text) && str.EndsWith(text))
		{
			return str;
		}
		return text + str + text;
	}
}
