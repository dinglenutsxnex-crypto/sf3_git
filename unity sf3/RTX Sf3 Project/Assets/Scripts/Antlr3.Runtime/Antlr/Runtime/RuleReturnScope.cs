using System;

namespace Antlr.Runtime
{
	public class RuleReturnScope
	{
		public virtual object Start
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException("Setter has not been defined for this property.");
			}
		}

		public virtual object Stop
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException("Setter has not been defined for this property.");
			}
		}

		public virtual object Tree
		{
			get
			{
				return null;
			}
			set
			{
				throw new NotSupportedException("Setter has not been defined for this property.");
			}
		}

		public virtual object Template
		{
			get
			{
				return null;
			}
		}
	}
}
