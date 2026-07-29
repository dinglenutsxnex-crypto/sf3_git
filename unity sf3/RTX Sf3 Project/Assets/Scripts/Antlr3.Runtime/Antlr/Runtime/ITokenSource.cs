namespace Antlr.Runtime
{
	public interface ITokenSource
	{
		string SourceName { get; }

		IToken NextToken();
	}
}
