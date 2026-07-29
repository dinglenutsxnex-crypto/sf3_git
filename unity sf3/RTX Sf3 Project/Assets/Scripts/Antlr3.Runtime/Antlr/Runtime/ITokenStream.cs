namespace Antlr.Runtime
{
	public interface ITokenStream : IIntStream
	{
		ITokenSource TokenSource { get; }

		IToken LT(int k);

		IToken Get(int i);

		string ToString(int start, int stop);

		string ToString(IToken start, IToken stop);
	}
}
