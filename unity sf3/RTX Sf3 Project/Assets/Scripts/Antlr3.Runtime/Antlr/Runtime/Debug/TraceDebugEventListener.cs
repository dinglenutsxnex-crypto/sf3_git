using System;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class TraceDebugEventListener : BlankDebugEventListener
	{
		private ITreeAdaptor adaptor;

		public TraceDebugEventListener(ITreeAdaptor adaptor)
		{
			this.adaptor = adaptor;
		}

		public override void EnterRule(string grammarFileName, string ruleName)
		{
			Console.Out.WriteLine("EnterRule " + grammarFileName + " " + ruleName);
		}

		public override void ExitRule(string grammarFileName, string ruleName)
		{
			Console.Out.WriteLine("ExitRule " + grammarFileName + " " + ruleName);
		}

		public override void EnterSubRule(int decisionNumber)
		{
			Console.Out.WriteLine("EnterSubRule");
		}

		public override void ExitSubRule(int decisionNumber)
		{
			Console.Out.WriteLine("ExitSubRule");
		}

		public override void Location(int line, int pos)
		{
			Console.Out.WriteLine("Location " + line + ":" + pos);
		}

		public override void ConsumeNode(object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string nodeText = adaptor.GetNodeText(t);
			int nodeType = adaptor.GetNodeType(t);
			Console.Out.WriteLine("ConsumeNode " + uniqueID + " " + nodeText + " " + nodeType);
		}

		public override void LT(int i, object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string nodeText = adaptor.GetNodeText(t);
			int nodeType = adaptor.GetNodeType(t);
			Console.Out.WriteLine("LT " + i + " " + uniqueID + " " + nodeText + " " + nodeType);
		}

		public override void GetNilNode(object t)
		{
			Console.Out.WriteLine("GetNilNode " + adaptor.GetUniqueID(t));
		}

		public override void CreateNode(object t)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			string nodeText = adaptor.GetNodeText(t);
			int nodeType = adaptor.GetNodeType(t);
			Console.Out.WriteLine("Create " + uniqueID + ": " + nodeText + ", " + nodeType);
		}

		public override void CreateNode(object t, IToken token)
		{
			int uniqueID = adaptor.GetUniqueID(t);
			int tokenIndex = token.TokenIndex;
			Console.Out.WriteLine("Create " + uniqueID + ": " + tokenIndex);
		}

		public override void BecomeRoot(object newRoot, object oldRoot)
		{
			Console.Out.WriteLine("BecomeRoot " + adaptor.GetUniqueID(newRoot) + ", " + adaptor.GetUniqueID(oldRoot));
		}

		public override void AddChild(object root, object child)
		{
			Console.Out.WriteLine("AddChild " + adaptor.GetUniqueID(root) + ", " + adaptor.GetUniqueID(child));
		}

		public override void SetTokenBoundaries(object t, int tokenStartIndex, int tokenStopIndex)
		{
			Console.Out.WriteLine("SetTokenBoundaries " + adaptor.GetUniqueID(t) + ", " + tokenStartIndex + ", " + tokenStopIndex);
		}
	}
}
