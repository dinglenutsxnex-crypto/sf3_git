using System;
using System.Collections.Generic;
using System.Xml;
using Nekki.UI;
using UnityEngine;

public class LocaleImport
{
	[Serializable]
	public class LocaleString
	{
		public enum SectionTypes
		{
			Text = 0,
			Image = 1
		}

		[Serializable]
		public class Section
		{
			[Serializable]
			public class PositionInfo
			{
				[SerializeField]
				public int Index;

				[SerializeField]
				public int Scale;

				[SerializeField]
				public bool ScaleToRow;

				[SerializeField]
				public string Content;

				public PositionInfo(int index, Section section)
				{
					Content = section.Content;
					ScaleToRow = section.ScaleToRow;
					Scale = section.Scale;
					Index = index;
				}
			}

			[SerializeField]
			public int ID;

			[SerializeField]
			public SectionTypes SectionType;

			[SerializeField]
			public string Content;

			[SerializeField]
			public int Scale;

			[SerializeField]
			public bool ScaleToRow;

			[SerializeField]
			public List<PositionInfo> Positions = new List<PositionInfo>();

			public Section(string content)
			{
				Positions = new List<PositionInfo>();
				Content = content;
				ID = -1;
				ScaleToRow = false;
				Scale = 100;
				SectionType = (content.Contains("image") ? SectionTypes.Image : SectionTypes.Text);
				try
				{
					if (content.Contains(":"))
					{
						string[] array = content.Trim('{', '}', ' ').Split(new string[1] { ":" }, StringSplitOptions.RemoveEmptyEntries);
						if (array.Length != 2 || !array[0].Contains("image"))
						{
							return;
						}
						if (array[1].ToLower().Equals("row"))
						{
							ScaleToRow = true;
						}
						else
						{
							try
							{
								Scale = int.Parse(array[1]);
							}
							catch (Exception)
							{
								Scale = 100;
							}
						}
						ID = int.Parse(array[0].Replace("image", string.Empty).Trim('{', '}'));
					}
					else
					{
						ID = int.Parse(content.Replace("image", string.Empty).Trim('{', '}'));
					}
				}
				catch (Exception)
				{
					ID = -1;
				}
			}

			public bool ContainsPosition(int index)
			{
				for (int i = 0; i < Positions.Count; i++)
				{
					if (Positions[i].Index == index)
					{
						return true;
					}
				}
				return false;
			}

			public PositionInfo GetPositionAtIndex(int index)
			{
				for (int i = 0; i < Positions.Count; i++)
				{
					if (Positions[i].Index == index)
					{
						return Positions[i];
					}
				}
				return null;
			}

			public void ReplacePositionAtIndex(int index, PositionInfo info)
			{
				for (int i = 0; i < Positions.Count; i++)
				{
					if (Positions[i].Index == index)
					{
						Positions[i] = info;
					}
				}
			}
		}

		private class SplitUnit
		{
			public bool newLine;

			public string content;

			public static List<SplitUnit> Split(string source)
			{
				List<SplitUnit> list = new List<SplitUnit>();
				source = source.Trim('\n', ' ');
				while (source.Contains("\n "))
				{
					source = source.Replace("\n ", "\n");
				}
				string[] array = source.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (text.Contains("\n"))
					{
						string[] array3 = text.Split(new char[1] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
						for (int j = 0; j < array3.Length; j++)
						{
							list.Add(new SplitUnit
							{
								content = array3[j],
								newLine = (j < array3.Length - 1)
							});
						}
					}
					else
					{
						list.Add(new SplitUnit
						{
							content = text,
							newLine = false
						});
					}
				}
				return list;
			}
		}

		[SerializeField]
		public bool Simple;

		[SerializeField]
		public bool ContainsImageRef;

		[SerializeField]
		public string String;

		[SerializeField]
		public List<Section> Sections = new List<Section>();

		[SerializeField]
		public List<string> ImgSplit = new List<string>();

		public LocaleString(string souce)
		{
			Simple = true;
			souce = souce.Trim();
			String = souce;
			if (string.IsNullOrEmpty(String))
			{
				return;
			}
			int num = -1;
			int num2 = ((souce[0] == '{') ? (-1) : 0);
			for (int i = 0; i < souce.Length; i++)
			{
				if (num == -1 && souce[i].Equals('{'))
				{
					num = i;
					Simple = false;
				}
				if (!souce[i].Equals('}'))
				{
					continue;
				}
				Simple = false;
				Section section = new Section(souce.Substring(num, i - num + 1));
				if (section.ID > -1)
				{
					if (!ContainsSection(section.ID))
					{
						Sections.Add(section);
					}
					if (section.SectionType == SectionTypes.Image)
					{
						Section sectionByID = GetSectionByID(section.ID);
						if (sectionByID.ContainsPosition(num2))
						{
							sectionByID.ReplacePositionAtIndex(num2, new Section.PositionInfo(num2, section));
						}
						else
						{
							sectionByID.Positions.Add(new Section.PositionInfo(num2, section));
						}
						num2++;
						if (!ImgSplit.Contains(section.Content))
						{
							ContainsImageRef = true;
							ImgSplit.Add(section.Content);
						}
					}
				}
				num = -1;
			}
		}

		public bool ContainsSection(int id)
		{
			for (int i = 0; i < Sections.Count; i++)
			{
				if (Sections[i].ID == id)
				{
					return true;
				}
			}
			return false;
		}

		public Section GetSectionByID(int id)
		{
			for (int i = 0; i < Sections.Count; i++)
			{
				if (Sections[i].ID == id)
				{
					return Sections[i];
				}
			}
			return null;
		}

		public void ReplaceSectionWithID(int id, Section section)
		{
			for (int i = 0; i < Sections.Count; i++)
			{
				if (Sections[i].ID == id)
				{
					Sections[i] = section;
				}
			}
		}

		public static implicit operator string(LocaleString i)
		{
			return i.String;
		}

		public string[] CompileWith(object[] replacement, NekkiUILabel label)
		{
			string text = String;
			for (int i = 0; i < replacement.Length; i++)
			{
				if (ContainsSection(i))
				{
					Section sectionByID = GetSectionByID(i);
					if (sectionByID.SectionType == SectionTypes.Text)
					{
						text = text.Replace(sectionByID.Content, replacement[i].ToString());
					}
				}
			}
			if (!ContainsImageRef)
			{
				return new string[1] { text };
			}
			return text.Split(ImgSplit.ToArray(), StringSplitOptions.RemoveEmptyEntries);
		}

		public LocaleString SplitByRows(int symbolsInLine, string[] lastReplacement)
		{
			string text = String.Replace("\\n", "\n");
			List<int> list = ExtractTextKeys(String, lastReplacement.Length);
			foreach (int item in list)
			{
				text = text.Replace("{" + item + "}", lastReplacement[item]);
			}
			text = UpdateImageKeys(text);
			List<SplitUnit> list2 = SplitUnit.Split(text);
			string text2 = " ";
			int num = 0;
			for (int i = 0; i < list2.Count; i++)
			{
				if (num + list2[i].content.Length + 1 <= symbolsInLine)
				{
					text2 = text2 + list2[i].content + ' ';
					num += list2[i].content.Length + 1;
					if (list2[i].newLine)
					{
						text2 = text2.TrimEnd(' ');
						text2 += '\n';
						num = 0;
					}
				}
				else
				{
					text2 = text2.TrimEnd(' ');
					string text3 = text2;
					text2 = string.Concat(text3, "\n", list2[i], ' ');
					num = list2[i].content.Length + 1;
				}
			}
			text2 = text2.TrimEnd(' ');
			for (int j = 0; j < lastReplacement.Length; j++)
			{
				text2 = text2.Replace("{" + j + "}", "{image" + j + ":row}");
			}
			return new LocaleString(text2);
		}

		private List<int> ExtractTextKeys(string source, int replacementCount)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < replacementCount; i++)
			{
				if (source.Contains("{" + i + "}"))
				{
					list.Add(i);
				}
			}
			return list;
		}

		private string UpdateImageKeys(string source)
		{
			int num = -1;
			int num2 = -1;
			List<string> list = new List<string>();
			for (int i = 0; i < source.Length; i++)
			{
				if (source[i] == '{')
				{
					num = i;
				}
				if (source[i] == '}')
				{
					num2 = i;
				}
				if (num >= 0 && num2 > 0)
				{
					list.Add(source.Substring(num, num2 - num + 1));
					num = -1;
					num2 = -1;
				}
			}
			foreach (string item in list)
			{
				string[] array = item.Trim().Trim('}', '{').Replace("image", string.Empty)
					.Split(':');
				source = source.Replace(item, "{" + array[0] + "}");
			}
			return source;
		}
	}

	public SystemLanguage Language { get; private set; }

	public Dictionary<string, LocaleString> Data { get; private set; }

	public LocaleImport(SystemLanguage language)
	{
		Language = language;
		Data = new Dictionary<string, LocaleString>();
		Load();
	}

	private void Load()
	{
		string localization = ConfigsSourceResolver.GetLocalization(Language);
		if (string.IsNullOrEmpty(localization))
		{
			return;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(localization);
		foreach (object childNode in xmlDocument.ChildNodes)
		{
			if (!(childNode is XmlElement) || !((XmlElement)childNode).Name.Equals("Localization"))
			{
				continue;
			}
			XmlNode firstChild = ((XmlElement)childNode).FirstChild;
			foreach (object childNode2 in firstChild.ChildNodes)
			{
				XmlElement xmlElement = childNode2 as XmlElement;
				if (xmlElement != null)
				{
					string attribute = xmlElement.GetAttribute("Title");
					string innerText = xmlElement.InnerText;
					if (!Data.ContainsKey(attribute))
					{
						Data.Add(attribute, new LocaleString(innerText));
					}
					else
					{
						Debug.LogError(string.Format("key [{0}] is not unique", attribute));
					}
				}
			}
		}
	}

	public static implicit operator SystemLanguage(LocaleImport i)
	{
		return i.Language;
	}

	public static implicit operator Dictionary<string, LocaleString>(LocaleImport i)
	{
		return i.Data;
	}
}
