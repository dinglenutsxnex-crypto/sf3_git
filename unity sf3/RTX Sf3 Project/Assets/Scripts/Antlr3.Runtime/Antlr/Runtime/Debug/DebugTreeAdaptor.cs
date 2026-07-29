using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class DebugTreeAdaptor : ITreeAdaptor
	{
		protected IDebugEventListener dbg;

		protected ITreeAdaptor adaptor;

		public IDebugEventListener DebugListener
		{
			get
			{
				return dbg;
			}
			set
			{
				dbg = value;
			}
		}

		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return adaptor;
			}
		}

		public DebugTreeAdaptor(IDebugEventListener dbg, ITreeAdaptor adaptor)
		{
			this.dbg = dbg;
			this.adaptor = adaptor;
		}

		public object Create(IToken payload)
		{
			if (payload.TokenIndex < 0)
			{
				return Create(payload.Type, payload.Text);
			}
			object obj = adaptor.Create(payload);
			dbg.CreateNode(obj, payload);
			return obj;
		}

		public object ErrorNode(ITokenStream input, IToken start, IToken stop, RecognitionException e)
		{
			object obj = adaptor.ErrorNode(input, start, stop, e);
			if (obj != null)
			{
				dbg.ErrorNode(obj);
			}
			return obj;
		}

		public object DupTree(object tree)
		{
			object obj = adaptor.DupTree(tree);
			SimulateTreeConstruction(obj);
			return obj;
		}

		protected void SimulateTreeConstruction(object t)
		{
			dbg.CreateNode(t);
			int childCount = adaptor.GetChildCount(t);
			for (int i = 0; i < childCount; i++)
			{
				object child = adaptor.GetChild(t, i);
				SimulateTreeConstruction(child);
				dbg.AddChild(t, child);
			}
		}

		public object DupNode(object treeNode)
		{
			object obj = adaptor.DupNode(treeNode);
			dbg.CreateNode(obj);
			return obj;
		}

		public object GetNilNode()
		{
			object nilNode = adaptor.GetNilNode();
			dbg.GetNilNode(nilNode);
			return nilNode;
		}

		public bool IsNil(object tree)
		{
			return adaptor.IsNil(tree);
		}

		public void AddChild(object t, object child)
		{
			if (t != null && child != null)
			{
				adaptor.AddChild(t, child);
				dbg.AddChild(t, child);
			}
		}

		public object BecomeRoot(object newRoot, object oldRoot)
		{
			object result = adaptor.BecomeRoot(newRoot, oldRoot);
			dbg.BecomeRoot(newRoot, oldRoot);
			return result;
		}

		public object RulePostProcessing(object root)
		{
			return adaptor.RulePostProcessing(root);
		}

		public void AddChild(object t, IToken child)
		{
			object child2 = Create(child);
			AddChild(t, child2);
		}

		public object BecomeRoot(IToken newRoot, object oldRoot)
		{
			object obj = Create(newRoot);
			adaptor.BecomeRoot(obj, oldRoot);
			dbg.BecomeRoot(newRoot, oldRoot);
			return obj;
		}

		public object Create(int tokenType, IToken fromToken)
		{
			object obj = adaptor.Create(tokenType, fromToken);
			dbg.CreateNode(obj);
			return obj;
		}

		public object Create(int tokenType, IToken fromToken, string text)
		{
			object obj = adaptor.Create(tokenType, fromToken, text);
			dbg.CreateNode(obj);
			return obj;
		}

		public object Create(int tokenType, string text)
		{
			object obj = adaptor.Create(tokenType, text);
			dbg.CreateNode(obj);
			return obj;
		}

		public int GetNodeType(object t)
		{
			return adaptor.GetNodeType(t);
		}

		public void SetNodeType(object t, int type)
		{
			adaptor.SetNodeType(t, type);
		}

		public string GetNodeText(object t)
		{
			return adaptor.GetNodeText(t);
		}

		public void SetNodeText(object t, string text)
		{
			adaptor.SetNodeText(t, text);
		}

		public IToken GetToken(object treeNode)
		{
			return adaptor.GetToken(treeNode);
		}

		public void SetTokenBoundaries(object t, IToken startToken, IToken stopToken)
		{
			adaptor.SetTokenBoundaries(t, startToken, stopToken);
			if (t != null && startToken != null && stopToken != null)
			{
				dbg.SetTokenBoundaries(t, startToken.TokenIndex, stopToken.TokenIndex);
			}
		}

		public int GetTokenStartIndex(object t)
		{
			return adaptor.GetTokenStartIndex(t);
		}

		public int GetTokenStopIndex(object t)
		{
			return adaptor.GetTokenStopIndex(t);
		}

		public object GetChild(object t, int i)
		{
			return adaptor.GetChild(t, i);
		}

		public void SetChild(object t, int i, object child)
		{
			adaptor.SetChild(t, i, child);
		}

		public object DeleteChild(object t, int i)
		{
			return adaptor.DeleteChild(t, i);
		}

		public int GetChildCount(object t)
		{
			return adaptor.GetChildCount(t);
		}

		public int GetUniqueID(object node)
		{
			return adaptor.GetUniqueID(node);
		}

		public object GetParent(object t)
		{
			return adaptor.GetParent(t);
		}

		public int GetChildIndex(object t)
		{
			return adaptor.GetChildIndex(t);
		}

		public void SetParent(object t, object parent)
		{
			adaptor.SetParent(t, parent);
		}

		public void SetChildIndex(object t, int index)
		{
			adaptor.SetChildIndex(t, index);
		}

		public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			adaptor.ReplaceChildren(parent, startChildIndex, stopChildIndex, t);
		}
	}
}
