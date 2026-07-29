using System;

namespace Antlr.Runtime.Tree
{
	[Serializable]
	public class RewriteCardinalityException : Exception
	{
		public string elementDescription;

		public override string Message
		{
			get
			{
				if (elementDescription != null)
				{
					return elementDescription;
				}
				return null;
			}
		}

		public RewriteCardinalityException(string elementDescription)
		{
			this.elementDescription = elementDescription;
		}
	}
}
