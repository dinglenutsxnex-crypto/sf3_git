namespace Antlr.Runtime.Debug
{
	public class BlankDebugEventListener : IDebugEventListener
	{
		public virtual void EnterRule(string grammarFileName, string ruleName)
		{
		}

		public virtual void ExitRule(string grammarFileName, string ruleName)
		{
		}

		public virtual void EnterAlt(int alt)
		{
		}

		public virtual void EnterSubRule(int decisionNumber)
		{
		}

		public virtual void ExitSubRule(int decisionNumber)
		{
		}

		public virtual void EnterDecision(int decisionNumber)
		{
		}

		public virtual void ExitDecision(int decisionNumber)
		{
		}

		public virtual void Location(int line, int pos)
		{
		}

		public virtual void ConsumeToken(IToken token)
		{
		}

		public virtual void ConsumeHiddenToken(IToken token)
		{
		}

		public virtual void LT(int i, IToken t)
		{
		}

		public virtual void Mark(int i)
		{
		}

		public virtual void Rewind(int i)
		{
		}

		public virtual void Rewind()
		{
		}

		public virtual void BeginBacktrack(int level)
		{
		}

		public virtual void EndBacktrack(int level, bool successful)
		{
		}

		public virtual void RecognitionException(RecognitionException e)
		{
		}

		public virtual void BeginResync()
		{
		}

		public virtual void EndResync()
		{
		}

		public virtual void SemanticPredicate(bool result, string predicate)
		{
		}

		public virtual void Commence()
		{
		}

		public virtual void Terminate()
		{
		}

		public virtual void ConsumeNode(object t)
		{
		}

		public virtual void LT(int i, object t)
		{
		}

		public virtual void GetNilNode(object t)
		{
		}

		public virtual void ErrorNode(object t)
		{
		}

		public virtual void CreateNode(object t)
		{
		}

		public virtual void CreateNode(object node, IToken token)
		{
		}

		public virtual void BecomeRoot(object newRoot, object oldRoot)
		{
		}

		public virtual void AddChild(object root, object child)
		{
		}

		public virtual void SetTokenBoundaries(object t, int tokenStartIndex, int tokenStopIndex)
		{
		}
	}
}
