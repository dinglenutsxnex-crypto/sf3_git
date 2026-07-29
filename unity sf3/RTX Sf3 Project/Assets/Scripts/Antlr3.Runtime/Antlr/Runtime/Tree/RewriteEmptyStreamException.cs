using System;

namespace Antlr.Runtime.Tree
{
	[Serializable]
	public class RewriteEmptyStreamException : RewriteCardinalityException
	{
		public RewriteEmptyStreamException(string elementDescription)
			: base(elementDescription)
		{
		}
	}
}
