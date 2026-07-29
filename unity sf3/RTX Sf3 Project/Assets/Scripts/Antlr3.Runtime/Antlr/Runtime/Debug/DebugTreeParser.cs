using System.IO;
using Antlr.Runtime.Misc;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class DebugTreeParser : TreeParser
	{
		protected IDebugEventListener dbg;

		public bool isCyclicDecision;

		public IDebugEventListener DebugListener
		{
			get
			{
				return dbg;
			}
			set
			{
				if (input is DebugTreeNodeStream)
				{
					((DebugTreeNodeStream)input).SetDebugListener(value);
				}
				dbg = value;
			}
		}

		public DebugTreeParser(ITreeNodeStream input, IDebugEventListener dbg, RecognizerSharedState state)
			: base((!(input is DebugTreeNodeStream)) ? new DebugTreeNodeStream(input, dbg) : input, state)
		{
			DebugListener = dbg;
		}

		public DebugTreeParser(ITreeNodeStream input, RecognizerSharedState state)
			: base((!(input is DebugTreeNodeStream)) ? new DebugTreeNodeStream(input, null) : input, state)
		{
		}

		public DebugTreeParser(ITreeNodeStream input, IDebugEventListener dbg)
			: this((!(input is DebugTreeNodeStream)) ? new DebugTreeNodeStream(input, dbg) : input, dbg, null)
		{
		}

		public virtual void ReportError(IOException e)
		{
			ErrorManager.InternalError(e);
		}

		public override void ReportError(RecognitionException e)
		{
			dbg.RecognitionException(e);
		}

		protected override object GetMissingSymbol(IIntStream input, RecognitionException e, int expectedTokenType, BitSet follow)
		{
			object missingSymbol = base.GetMissingSymbol(input, e, expectedTokenType, follow);
			dbg.ConsumeNode(missingSymbol);
			return missingSymbol;
		}

		public override void BeginResync()
		{
			dbg.BeginResync();
		}

		public override void EndResync()
		{
			dbg.EndResync();
		}

		public override void BeginBacktrack(int level)
		{
			dbg.BeginBacktrack(level);
		}

		public override void EndBacktrack(int level, bool successful)
		{
			dbg.EndBacktrack(level, successful);
		}
	}
}
