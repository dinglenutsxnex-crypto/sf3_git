using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace Nekki.Yaml
{
	[Serializable]
	public class YamlDocumentNekki
	{
		private YamlStream _yamlStream;

		private YamlDocument _yamlDocument;

		private Mapping _rootMapping;

		private string _content;

		public YamlDocumentNekki()
		{
			_yamlStream = new YamlStream();
		}

		public static YamlDocumentNekki FromYamlFile(string fileName)
		{
			string text = FilesUtil.ReadFileText(fileName);
			if (text.IsNullOrEmpty())
			{
				Debug.LogError("YAML file is not exists!!!");
				return null;
			}
			return FromYamlContent(text);
		}

		public static YamlDocumentNekki FromYamlContent(string yamlContent)
		{
			YamlDocumentNekki yamlDocumentNekki = new YamlDocumentNekki();
			yamlDocumentNekki._yamlStream.Load(new StringReader(yamlContent));
			yamlDocumentNekki._yamlDocument = yamlDocumentNekki._yamlStream.Documents[0];
			if (yamlDocumentNekki._yamlDocument.RootNode is YamlMappingNode)
			{
				yamlDocumentNekki._rootMapping = new Mapping("Root", (YamlMappingNode)yamlDocumentNekki._yamlDocument.RootNode);
			}
			yamlDocumentNekki._content = yamlContent;
			return yamlDocumentNekki;
		}

		public override string ToString()
		{
			return _yamlDocument.ToString();
		}

		public void SaveToFile(string fileName, bool useAnchors = true)
		{
			FilesUtil.CreateTextWriter(fileName, delegate(TextWriter writer)
			{
				_yamlStream.Save(writer, useAnchors);
			});
		}

		public string GetYamlContent()
		{
			StringWriter stringWriter = new StringWriter();
			_yamlStream.Save(stringWriter);
			return stringWriter.ToString();
		}

		public Node GetRoot(string name)
		{
			return _rootMapping.GetNode(name);
		}

		public Mapping GetRoot(int index = 0)
		{
			return _rootMapping;
		}

		public void Serialize(string fileName)
		{
			TextReader input = new StringReader(_content);
			Deserializer deserializer = new Deserializer();
			object obj = deserializer.Deserialize(input);
			if (obj == null)
			{
				return;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using (FileStream serializationStream = File.OpenWrite(fileName))
			{
				binaryFormatter.Serialize(serializationStream, obj);
			}
		}
	}
}
