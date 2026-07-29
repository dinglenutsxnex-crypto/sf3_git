namespace Antlr.Runtime
{
	public class UnwantedTokenException : MismatchedTokenException
	{
		public IToken UnexpectedToken
		{
			get
			{
				return token;
			}
		}

		public UnwantedTokenException()
		{
		}

		public UnwantedTokenException(int expecting, IIntStream input)
			: base(expecting, input)
		{
		}

		public override string ToString()
		{
			string text = ", expected " + base.Expecting;
			if (base.Expecting == 0)
			{
				text = string.Empty;
			}
			if (token == null)
			{
				return string.Concat("UnwantedTokenException(found=", null, text, ")");
			}
			return "UnwantedTokenException(found=" + token.Text + text + ")";
		}
	}
}
