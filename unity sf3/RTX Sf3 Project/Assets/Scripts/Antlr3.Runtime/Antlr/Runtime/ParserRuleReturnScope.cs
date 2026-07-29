namespace Antlr.Runtime
{
	public class ParserRuleReturnScope : RuleReturnScope
	{
		private IToken start;

		private IToken stop;

		public override object Start
		{
			get
			{
				return start;
			}
			set
			{
				start = (IToken)value;
			}
		}

		public override object Stop
		{
			get
			{
				return stop;
			}
			set
			{
				stop = (IToken)value;
			}
		}
	}
}
