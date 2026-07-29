using System.Collections;

namespace Antlr.Runtime.Debug
{
	public class DebugEventHub : IDebugEventListener
	{
		protected IList listeners = new ArrayList();

		public DebugEventHub(IDebugEventListener listener)
		{
			listeners.Add(listener);
		}

		public DebugEventHub(params IDebugEventListener[] listeners)
		{
			foreach (IDebugEventListener value in listeners)
			{
				this.listeners.Add(value);
			}
		}

		public void AddListener(IDebugEventListener listener)
		{
			listeners.Add(listener);
		}

		public void EnterRule(string grammarFileName, string ruleName)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.EnterRule(grammarFileName, ruleName);
			}
		}

		public void ExitRule(string grammarFileName, string ruleName)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ExitRule(grammarFileName, ruleName);
			}
		}

		public void EnterAlt(int alt)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.EnterAlt(alt);
			}
		}

		public void EnterSubRule(int decisionNumber)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.EnterSubRule(decisionNumber);
			}
		}

		public void ExitSubRule(int decisionNumber)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ExitSubRule(decisionNumber);
			}
		}

		public void EnterDecision(int decisionNumber)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.EnterDecision(decisionNumber);
			}
		}

		public void ExitDecision(int decisionNumber)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ExitDecision(decisionNumber);
			}
		}

		public void Location(int line, int pos)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.Location(line, pos);
			}
		}

		public void ConsumeToken(IToken token)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ConsumeToken(token);
			}
		}

		public void ConsumeHiddenToken(IToken token)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ConsumeHiddenToken(token);
			}
		}

		public void LT(int index, IToken t)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.LT(index, t);
			}
		}

		public void Mark(int index)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.Mark(index);
			}
		}

		public void Rewind(int index)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.Rewind(index);
			}
		}

		public void Rewind()
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.Rewind();
			}
		}

		public void BeginBacktrack(int level)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.BeginBacktrack(level);
			}
		}

		public void EndBacktrack(int level, bool successful)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.EndBacktrack(level, successful);
			}
		}

		public void RecognitionException(RecognitionException e)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.RecognitionException(e);
			}
		}

		public void BeginResync()
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.BeginResync();
			}
		}

		public void EndResync()
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.EndResync();
			}
		}

		public void SemanticPredicate(bool result, string predicate)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.SemanticPredicate(result, predicate);
			}
		}

		public void Commence()
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.Commence();
			}
		}

		public void Terminate()
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.Terminate();
			}
		}

		public void ConsumeNode(object t)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ConsumeNode(t);
			}
		}

		public void LT(int index, object t)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.LT(index, t);
			}
		}

		public void GetNilNode(object t)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.GetNilNode(t);
			}
		}

		public void ErrorNode(object t)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.ErrorNode(t);
			}
		}

		public void CreateNode(object t)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.CreateNode(t);
			}
		}

		public void CreateNode(object node, IToken token)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.CreateNode(node, token);
			}
		}

		public void BecomeRoot(object newRoot, object oldRoot)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.BecomeRoot(newRoot, oldRoot);
			}
		}

		public void AddChild(object root, object child)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.AddChild(root, child);
			}
		}

		public void SetTokenBoundaries(object t, int tokenStartIndex, int tokenStopIndex)
		{
			for (int i = 0; i < listeners.Count; i++)
			{
				IDebugEventListener debugEventListener = (IDebugEventListener)listeners[i];
				debugEventListener.SetTokenBoundaries(t, tokenStartIndex, tokenStopIndex);
			}
		}
	}
}
