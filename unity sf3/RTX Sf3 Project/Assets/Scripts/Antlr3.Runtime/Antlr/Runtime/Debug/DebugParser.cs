using System.IO;
using Antlr.Runtime.Misc;

namespace Antlr.Runtime.Debug
{
	public class DebugParser : Parser
	{
		protected internal IDebugEventListener dbg;

		public bool isCyclicDecision;

		public virtual IDebugEventListener DebugListener
		{
			get
			{
				return dbg;
			}
			set
			{
				if (input is DebugTokenStream)
				{
					((DebugTokenStream)input).DebugListener = value;
				}
				dbg = value;
			}
		}

		public DebugParser(ITokenStream input, IDebugEventListener dbg, RecognizerSharedState state)
			: base((!(input is DebugTokenStream)) ? new DebugTokenStream(input, dbg) : input, state)
		{
			DebugListener = dbg;
		}

		public DebugParser(ITokenStream input, RecognizerSharedState state)
			: base((!(input is DebugTokenStream)) ? new DebugTokenStream(input, null) : input, state)
		{
		}

		public DebugParser(ITokenStream input, IDebugEventListener dbg)
			: this((!(input is DebugTokenStream)) ? new DebugTokenStream(input, dbg) : input, dbg, null)
		{
		}

		public virtual void ReportError(IOException e)
		{
			ErrorManager.InternalError(e);
		}

		public override void BeginResync()
		{
			dbg.BeginResync();
		}

		public override void EndResync()
		{
			dbg.EndResync();
		}

		public override void ReportError(RecognitionException e)
		{
			dbg.RecognitionException(e);
		}
	}
}
