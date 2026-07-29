using System;
using System.Text.RegularExpressions;

namespace YamlDotNet.Core.Events
{
	public abstract class NodeEvent : ParsingEvent
	{
		internal static readonly Regex anchorValidator = new Regex("^[0-9a-zA-Z_\\-]+$", RegexOptions.None);

		private readonly string anchor;

		private readonly string tag;

		public string Anchor
		{
			get
			{
				return anchor;
			}
		}

		public string Tag
		{
			get
			{
				return tag;
			}
		}

		public abstract bool IsCanonical { get; }

		protected NodeEvent(string anchor, string tag, Mark start, Mark end)
			: base(start, end)
		{
			if (anchor != null && anchor.Length != 0 && !anchorValidator.IsMatch(anchor))
			{
				throw new ArgumentException("Anchor value must contain alphanumerical characters only.", "anchor");
			}
			this.anchor = anchor;
			this.tag = tag;
		}

		protected NodeEvent(string anchor, string tag)
			: this(anchor, tag, Mark.Empty, Mark.Empty)
		{
		}
	}
}
