using System.Collections;
using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace Nekki.Yaml
{
	public class Sequence : Node
	{
		private YamlSequenceNode _sequence;

		public List<Node> nodesInside { get; private set; }

		public Sequence(string keyNew, YamlSequenceNode sequenceNew)
		{
			base.typeNode = "Sequence";
			base.key = keyNew;
			base.value = sequenceNew;
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			foreach (YamlNode item in _sequence)
			{
				nodesInside.Add(Node.CreateNodeByType(base.key, item));
			}
		}

		public Sequence(string keyNew, Node[] mappingInside)
		{
			base.typeNode = "Sequence";
			base.key = keyNew;
			base.value = new YamlSequenceNode(new YamlNode[0]);
			_sequence = (YamlSequenceNode)base.value;
			nodesInside = new List<Node>();
			AddNodes(mappingInside);
		}

		public Sequence(string keyNew, List<Node> mappingInside)
			: this(keyNew, mappingInside.ToArray())
		{
		}

		public Sequence(string keyNew, Node node)
			: this(keyNew, new Node[1] { node })
		{
		}

		public void UpdateNodeAtIndex(int index, Node newNode)
		{
			_sequence.UpdateNode(nodesInside[index].value, newNode.value);
			nodesInside[index] = newNode;
		}

		public void Replace(int nodeIndex, Node newValue)
		{
			if (nodeIndex < nodesInside.Count && newValue != null)
			{
				_sequence.Replace(newValue.value, (YamlNode e) => e.Equals(nodesInside[nodeIndex].value));
				nodesInside[nodeIndex] = newValue;
			}
		}

		public void ReplaceAll(List<Node> newNodes)
		{
			RemoveNodes();
			foreach (Node newNode in newNodes)
			{
				_sequence.Add(newNode.value);
			}
			nodesInside = newNodes;
		}

		public void RemoveNodes()
		{
			foreach (Node item in nodesInside)
			{
				_sequence.Remove(item.value);
			}
			nodesInside.Clear();
		}

		public void Remove(Node newNode)
		{
			_sequence.Remove(newNode.value);
			nodesInside.Remove(newNode);
		}

		public void AddNode(Node newNode)
		{
			_sequence.Add(newNode.value);
			nodesInside.Add(newNode);
		}

		public void AddNodes(Node[] newNodes)
		{
			foreach (Node newNode in newNodes)
			{
				AddNode(newNode);
			}
		}

		public void AddNodes(List<Node> newNodes)
		{
			AddNodes(newNodes.ToArray());
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

		public override IEnumerator GetEnumerator()
		{
			foreach (Node item in nodesInside)
			{
				yield return item;
			}
		}
	}
}
