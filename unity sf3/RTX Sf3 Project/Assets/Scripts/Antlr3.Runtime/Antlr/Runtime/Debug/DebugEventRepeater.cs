namespace Antlr.Runtime.Debug
{
	public class DebugEventRepeater : IDebugEventListener
	{
		protected IDebugEventListener listener;

		public DebugEventRepeater(IDebugEventListener listener)
		{
			this.listener = listener;
		}

		public void EnterRule(string grammarFileName, string ruleName)
		{
			listener.EnterRule(grammarFileName, ruleName);
		}

		public void ExitRule(string grammarFileName, string ruleName)
		{
			listener.ExitRule(grammarFileName, ruleName);
		}

		public void EnterAlt(int alt)
		{
			listener.EnterAlt(alt);
		}

		public void EnterSubRule(int decisionNumber)
		{
			listener.EnterSubRule(decisionNumber);
		}

		public void ExitSubRule(int decisionNumber)
		{
			listener.ExitSubRule(decisionNumber);
		}

		public void EnterDecision(int decisionNumber)
		{
			listener.EnterDecision(decisionNumber);
		}

		public void ExitDecision(int decisionNumber)
		{
			listener.ExitDecision(decisionNumber);
		}

		public void Location(int line, int pos)
		{
			listener.Location(line, pos);
		}

		public void ConsumeToken(IToken token)
		{
			listener.ConsumeToken(token);
		}

		public void ConsumeHiddenToken(IToken token)
		{
			listener.ConsumeHiddenToken(token);
		}

		public void LT(int i, IToken t)
		{
			listener.LT(i, t);
		}

		public void Mark(int i)
		{
			listener.Mark(i);
		}

		public void Rewind(int i)
		{
			listener.Rewind(i);
		}

		public void Rewind()
		{
			listener.Rewind();
		}

		public void BeginBacktrack(int level)
		{
			listener.BeginBacktrack(level);
		}

		public void EndBacktrack(int level, bool successful)
		{
			listener.EndBacktrack(level, successful);
		}

		public void RecognitionException(RecognitionException e)
		{
			listener.RecognitionException(e);
		}

		public void BeginResync()
		{
			listener.BeginResync();
		}

		public void EndResync()
		{
			listener.EndResync();
		}

		public void SemanticPredicate(bool result, string predicate)
		{
			listener.SemanticPredicate(result, predicate);
		}

		public void Commence()
		{
			listener.Commence();
		}

		public void Terminate()
		{
			listener.Terminate();
		}

		public void ConsumeNode(object t)
		{
			listener.ConsumeNode(t);
		}

		public void LT(int i, object t)
		{
			listener.LT(i, t);
		}

		public void GetNilNode(object t)
		{
			listener.GetNilNode(t);
		}

		public void ErrorNode(object t)
		{
			listener.ErrorNode(t);
		}

		public void CreateNode(object t)
		{
			listener.CreateNode(t);
		}

		public void CreateNode(object node, IToken token)
		{
			listener.CreateNode(node, token);
		}

		public void BecomeRoot(object newRoot, object oldRoot)
		{
			listener.BecomeRoot(newRoot, oldRoot);
		}

		public void AddChild(object root, object child)
		{
			listener.AddChild(root, child);
		}

		public void SetTokenBoundaries(object t, int tokenStartIndex, int tokenStopIndex)
		{
			listener.SetTokenBoundaries(t, tokenStartIndex, tokenStopIndex);
		}
	}
}
