using System;

namespace Antlr.Runtime.Tree
{
	[Serializable]
	public class RewriteEarlyExitException : RewriteCardinalityException
	{
		public RewriteEarlyExitException()
			: base(null)
		{
		}

		public RewriteEarlyExitException(string elementDescription)
			: base(elementDescription)
		{
		}
	}
}
