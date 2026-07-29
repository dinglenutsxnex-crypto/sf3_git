using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class StringExtensions
{
	public static bool IsNullOrEmpty(this string value)
	{
		return string.IsNullOrEmpty(value);
	}

	public static string AsRpn(this string value)
	{
		return new RpnParser.Formula(value).calculate().ToString();
	}

	public static IEnumerable<string> SplitAndKeep(this string s, char[] delims)
	{
		int start = 0;
		while (true)
		{
			int num;
			int index = (num = s.IndexOfAny(delims, start));
			if (num == -1)
			{
				break;
			}
			if (index - start > 0)
			{
				yield return s.Substring(start, index - start);
			}
			yield return s.Substring(index, 1);
			start = index + 1;
		}
		if (start < s.Length)
		{
			yield return s.Substring(start);
		}
	}

	public static void CLog(this object text, string color = "green")
	{
		Debug.Log(string.Concat("$<color=", color, "><b>", text, "</b></color>"));
	}

	public static void CLogRed(this object text)
	{
		text.CLog("red");
	}

	public static void CLogBlue(this object text)
	{
		text.CLog("blue");
	}

	public static string ToColored(this string text, string colorCode)
	{
		return "<color=#" + colorCode + ">" + text + "</color>";
	}

	public static string ToColored(this int text, string colorCode)
	{
		return text.ToString().ToColored(colorCode);
	}

	public static string ToColored(this float text, string colorCode)
	{
		return text.ToString(CultureInfo.InvariantCulture).ToColored(colorCode);
	}
}
