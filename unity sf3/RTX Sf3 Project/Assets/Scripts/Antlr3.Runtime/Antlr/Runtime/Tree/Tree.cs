namespace Antlr.Runtime.Tree
{
	public sealed class Tree
	{
		public static readonly ITree INVALID_NODE = new CommonTree(Token.INVALID_TOKEN);
	}
}
