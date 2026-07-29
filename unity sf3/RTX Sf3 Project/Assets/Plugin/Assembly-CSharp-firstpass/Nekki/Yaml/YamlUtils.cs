using System.Globalization;
using UnityEngine;

namespace Nekki.Yaml
{
	public class YamlUtils
	{
		public static float GetFloat(Node mapp, string name, float defValue = 0f)
		{
			string text = GetText(mapp, name);
			return (text == null) ? defValue : float.Parse(text, CultureInfo.InvariantCulture);
		}

		public static bool GetBool(Node mapp, string name, bool defValue = false)
		{
			int @int = GetInt(mapp, name, defValue ? 1 : 0);
			return @int > 0;
		}

		public static int GetInt(Node mapp, string name, int defValue = 0)
		{
			string text = GetText(mapp, name);
			return (text == null) ? defValue : int.Parse(text);
		}

		public static string GetText(Node mapp, string name, string defValue = null)
		{
			Node node = GetNode(mapp, name);
			return (node == null) ? defValue : ((Scalar)node).text;
		}

		public static Node GetNode(Node node, string name)
		{
			if (node is Mapping)
			{
				return ((Mapping)node).GetNode(name);
			}
			if (node is Scalar)
			{
				if (node.key.Equals(name))
				{
					return node;
				}
			}
			else if (node is Sequence)
			{
				foreach (Node item in ((Sequence)node).nodesInside)
				{
					Node node2 = GetNode(item, name);
					if (node2 != null)
					{
						return node2;
					}
				}
			}
			return null;
		}

		public static bool TryGetString(out string result, Node node, string nameToSearch, string defaultValueToSet = null)
		{
			result = GetText(node, nameToSearch);
			if (result != null)
			{
				return true;
			}
			result = defaultValueToSet;
			return false;
		}

		public static bool TryGetFloat(out float result, Node node, string nameToSearch, float defaultValueToSet = 0f)
		{
			string result2;
			if (TryGetString(out result2, node, nameToSearch) && float.TryParse(result2, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
			{
				return true;
			}
			result = defaultValueToSet;
			return false;
		}

		public static bool TryGetInt(out int result, Node node, string nameToSearch, int defaultValueToSet = 0)
		{
			string result2;
			if (TryGetString(out result2, node, nameToSearch) && int.TryParse(result2, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				return true;
			}
			result = defaultValueToSet;
			return false;
		}

		public static bool TryGetBool(out bool result, Node node, string nameToSearch, bool defaultValueToSet = false)
		{
			string result2;
			if (TryGetString(out result2, node, nameToSearch))
			{
				result = result2.Equals("1") || result2.ToUpper().Equals("TRUE");
				return true;
			}
			result = defaultValueToSet;
			return false;
		}

		public static bool TryParseVector2(out Vector2 result, Node node, string nameToSearch, Vector2 defaultValueToSet)
		{
			Node node2 = GetNode(node, nameToSearch);
			if (node2 != null)
			{
				result = new Vector2(GetFloat(node2, "X"), GetFloat(node2, "Y"));
				return true;
			}
			result = defaultValueToSet;
			return false;
		}
	}
}
