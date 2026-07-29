using System.Collections;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class ParseTreeBuilder : BlankDebugEventListener
	{
		public static readonly string EPSILON_PAYLOAD = "<epsilon>";

		private Stack callStack = new Stack();

		private IList hiddenTokens = new ArrayList();

		private int backtracking;

		public ParseTree Tree
		{
			get
			{
				return (ParseTree)callStack.Peek();
			}
		}

		public ParseTreeBuilder(string grammarName)
		{
			ParseTree obj = Create("<grammar " + grammarName + ">");
			callStack.Push(obj);
		}

		public ParseTree Create(object payload)
		{
			return new ParseTree(payload);
		}

		public ParseTree EpsilonNode()
		{
			return Create(EPSILON_PAYLOAD);
		}

		public override void EnterDecision(int d)
		{
			backtracking++;
		}

		public override void ExitDecision(int i)
		{
			backtracking--;
		}

		public override void EnterRule(string filename, string ruleName)
		{
			if (backtracking <= 0)
			{
				ParseTree parseTree = (ParseTree)callStack.Peek();
				ParseTree parseTree2 = Create(ruleName);
				parseTree.AddChild(parseTree2);
				callStack.Push(parseTree2);
			}
		}

		public override void ExitRule(string filename, string ruleName)
		{
			if (backtracking <= 0)
			{
				ParseTree parseTree = (ParseTree)callStack.Peek();
				if (parseTree.ChildCount == 0)
				{
					parseTree.AddChild(EpsilonNode());
				}
				callStack.Pop();
			}
		}

		public override void ConsumeToken(IToken token)
		{
			if (backtracking <= 0)
			{
				ParseTree parseTree = (ParseTree)callStack.Peek();
				ParseTree parseTree2 = Create(token);
				parseTree2.hiddenTokens = hiddenTokens;
				hiddenTokens = new ArrayList();
				parseTree.AddChild(parseTree2);
			}
		}

		public override void ConsumeHiddenToken(IToken token)
		{
			if (backtracking <= 0)
			{
				hiddenTokens.Add(token);
			}
		}

		public override void RecognitionException(RecognitionException e)
		{
			if (backtracking <= 0)
			{
				ParseTree parseTree = (ParseTree)callStack.Peek();
				ParseTree t = Create(e);
				parseTree.AddChild(t);
			}
		}
	}
}
