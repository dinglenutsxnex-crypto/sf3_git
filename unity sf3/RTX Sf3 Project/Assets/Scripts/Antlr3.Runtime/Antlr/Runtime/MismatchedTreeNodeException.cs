using System;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime
{
	[Serializable]
	public class MismatchedTreeNodeException : RecognitionException
	{
		public int expecting;

		public MismatchedTreeNodeException()
		{
		}

		public MismatchedTreeNodeException(int expecting, ITreeNodeStream input)
			: base(input)
		{
			this.expecting = expecting;
		}

		public override string ToString()
		{
			return "MismatchedTreeNodeException(" + UnexpectedType + "!=" + expecting + ")";
		}
	}
}
