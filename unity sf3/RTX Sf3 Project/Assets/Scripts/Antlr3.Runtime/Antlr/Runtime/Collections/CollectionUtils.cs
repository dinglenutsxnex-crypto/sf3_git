using System.Collections;
using System.Text;

namespace Antlr.Runtime.Collections
{
	public class CollectionUtils
	{
		public static string ListToString(IList coll)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (coll != null)
			{
				stringBuilder.Append("[");
				for (int i = 0; i < coll.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(", ");
					}
					object obj = coll[i];
					if (obj == null)
					{
						stringBuilder.Append("null");
					}
					else if (obj is IDictionary)
					{
						stringBuilder.Append(DictionaryToString((IDictionary)obj));
					}
					else if (obj is IList)
					{
						stringBuilder.Append(ListToString((IList)obj));
					}
					else
					{
						stringBuilder.Append(obj.ToString());
					}
				}
				stringBuilder.Append("]");
			}
			else
			{
				stringBuilder.Insert(0, "null");
			}
			return stringBuilder.ToString();
		}

		public static string DictionaryToString(IDictionary dict)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (dict != null)
			{
				stringBuilder.Append("{");
				int num = 0;
				foreach (object item in dict)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)item;
					if (num > 0)
					{
						stringBuilder.Append(", ");
					}
					if (dictionaryEntry.Value is IDictionary)
					{
						stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), DictionaryToString((IDictionary)dictionaryEntry.Value));
					}
					else if (dictionaryEntry.Value is IList)
					{
						stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), ListToString((IList)dictionaryEntry.Value));
					}
					else
					{
						stringBuilder.AppendFormat("{0}={1}", dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());
					}
					num++;
				}
				stringBuilder.Append("}");
			}
			else
			{
				stringBuilder.Insert(0, "null");
			}
			return stringBuilder.ToString();
		}
	}
}
