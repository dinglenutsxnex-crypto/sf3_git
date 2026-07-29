using System;
using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	[Serializable]
	public class Mapping : Node, ICollection, IEnumerator, IEnumerable
	{
		private YamlMappingNode _mapping;

		private int indexPosition = -1;

		public List<Node> nodesInside { get; private set; }

		public int Count
		{
			get
			{
				if (nodesInside != null)
				{
					return nodesInside.Count;
				}
				return 0;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

		public object Current
		{
			get
			{
				return nodesInside[indexPosition];
			}
		}

		public Mapping(string keyNew, YamlMappingNode mapping)
		{
			base.typeNode = "Mapping";
			base.key = keyNew;
			base.value = mapping;
			_mapping = (YamlMappingNode)base.value;
			nodesInside = new List<Node>();
			foreach (KeyValuePair<YamlNode, YamlNode> item in _mapping)
			{
				nodesInside.Add(Node.CreateNodeByType(item.Key.ToString(), item.Value));
			}
		}

		public Mapping(Mapping entriesMapping)
			: this(entriesMapping.key, (YamlMappingNode)entriesMapping.value)
		{
		}

		public Mapping(string keyNew, Node[] entries)
		{
			base.typeNode = "Mapping";
			base.key = keyNew;
			base.value = new YamlMappingNode(new YamlNode[0]);
			_mapping = (YamlMappingNode)base.value;
			nodesInside = new List<Node>();
			AddNodes(entries);
		}

		public Mapping(string keyNew, Node entry)
			: this(keyNew, new Node[1] { entry })
		{
		}

		public Mapping(string keyNew, List<Node> entries)
			: this(keyNew, entries.ToArray())
		{
		}

		public int GetNodesSize()
		{
			return nodesInside.Count;
		}

		public Node GetNodesByIndex(int index)
		{
			if (index < nodesInside.Count)
			{
				return nodesInside[index];
			}
			return null;
		}

		public List<Node> GetNodesInside()
		{
			return nodesInside;
		}

		public void Add(Node entry)
		{
			if (entry.value.ToString().Length > 0)
			{
				_mapping.Add(entry.key, entry.value);
				nodesInside.Add(entry);
			}
		}

		public void AddNodes(List<Node> newNodes)
		{
			AddNodes(newNodes.ToArray());
		}

		public void AddNodes(Node[] newNodes)
		{
			foreach (Node entry in newNodes)
			{
				Add(entry);
			}
		}

		public void Remove(string key, string value)
		{
			foreach (Node item in nodesInside)
			{
				if (item.key == key && item.value.ToString() == value)
				{
					nodesInside.Remove(item);
					_mapping.Remove(key, item.value);
					break;
				}
			}
		}

		public void Remove(Node nodeToDelete)
		{
			if (nodeToDelete != null)
			{
				Remove(nodeToDelete.key, nodeToDelete.value.ToString());
			}
		}

		public Mapping GetMapping(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode node = _mapping.GetNode(name);
			Type type = node.GetType();
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(name, (YamlMappingNode)node);
			}
			return null;
		}

		public Mapping GetMappingByPath(string path)
		{
			string[] array = path.Split(':');
			Mapping mapping = this;
			string[] array2 = array;
			foreach (string name in array2)
			{
				mapping = mapping.GetMapping(name);
				if (mapping == null)
				{
					return null;
				}
			}
			return mapping;
		}

		public Sequence GetSequence(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			Sequence result = null;
			foreach (Node item in nodesInside)
			{
				if (item.key.Equals(name) && item is Sequence)
				{
					result = (Sequence)item;
					break;
				}
			}
			return result;
		}

		public Scalar GetText(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode node = _mapping.GetNode(name);
			Type type = node.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(name, (YamlScalarNode)node);
			}
			return null;
		}

		public void SetOrAddText(string nodeName, string text)
		{
			Scalar text2 = GetText(nodeName);
			if (text2 != null)
			{
				text2.SetText(text);
			}
			else
			{
				Add(new Scalar(nodeName, text));
			}
		}

		public Node GetNode(string name)
		{
			if (!_mapping.HasKey(name))
			{
				return null;
			}
			YamlNode node = _mapping.GetNode(name);
			Type type = node.GetType();
			if (type == typeof(YamlScalarNode))
			{
				return new Scalar(name, (YamlScalarNode)node);
			}
			if (type == typeof(YamlSequenceNode))
			{
				return new Sequence(name, (YamlSequenceNode)node);
			}
			if (type == typeof(YamlMappingNode))
			{
				return new Mapping(name, (YamlMappingNode)node);
			}
			return null;
		}

		public override IEnumerator GetEnumerator()
		{
			foreach (Node item in nodesInside)
			{
				yield return item;
			}
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("Array is null.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("Index is less than zero");
			}
			if (array.Length - index < nodesInside.Count)
			{
				throw new ArgumentException("The number of elements in the source ICollection is greater than the available space from index to the end of the destination array.");
			}
			for (int i = index; i < array.Length; i++)
			{
				array.SetValue(nodesInside[i], i);
			}
		}

		public bool MoveNext()
		{
			if (indexPosition == nodesInside.Count - 1)
			{
				Reset();
				return false;
			}
			indexPosition++;
			return true;
		}

		public void Reset()
		{
			indexPosition = -1;
		}
	}
}
