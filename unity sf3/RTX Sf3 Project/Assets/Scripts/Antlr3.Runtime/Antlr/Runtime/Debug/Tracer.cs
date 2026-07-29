using System;

namespace Antlr.Runtime.Debug
{
	public class Tracer : BlankDebugEventListener
	{
		public IIntStream input;

		protected int level;

		public Tracer(IIntStream input)
		{
			this.input = input;
		}

		public override void EnterRule(string grammarFileName, string ruleName)
		{
			for (int i = 1; i <= level; i++)
			{
				Console.Out.Write(" ");
			}
			Console.Out.WriteLine("> " + grammarFileName + " " + ruleName + " lookahead(1)=" + GetInputSymbol(1));
			level++;
		}

		public override void ExitRule(string grammarFileName, string ruleName)
		{
			level--;
			for (int i = 1; i <= level; i++)
			{
				Console.Out.Write(" ");
			}
			Console.Out.WriteLine("< " + grammarFileName + " " + ruleName + " lookahead(1)=" + GetInputSymbol(1));
		}

		public virtual object GetInputSymbol(int k)
		{
			if (input is ITokenStream)
			{
				return ((ITokenStream)input).LT(k);
			}
			return (char)input.LA(k);
		}
	}
}
