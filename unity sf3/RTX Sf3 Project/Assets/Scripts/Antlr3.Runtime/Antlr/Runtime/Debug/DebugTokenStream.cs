using System;

namespace Antlr.Runtime.Debug
{
	public class DebugTokenStream : IIntStream, ITokenStream
	{
		protected internal IDebugEventListener dbg;

		public ITokenStream input;

		protected internal bool initialStreamState = true;

		protected int lastMarker;

		public virtual IDebugEventListener DebugListener
		{
			set
			{
				dbg = value;
			}
		}

		public virtual int Count
		{
			get
			{
				return input.Count;
			}
		}

		public virtual ITokenSource TokenSource
		{
			get
			{
				return input.TokenSource;
			}
		}

		public virtual string SourceName
		{
			get
			{
				return TokenSource.SourceName;
			}
		}

		public DebugTokenStream(ITokenStream input, IDebugEventListener dbg)
		{
			this.input = input;
			DebugListener = dbg;
			input.LT(1);
		}

		public virtual void Consume()
		{
			if (initialStreamState)
			{
				ConsumeInitialHiddenTokens();
			}
			int num = input.Index();
			IToken t = input.LT(1);
			input.Consume();
			int num2 = input.Index();
			dbg.ConsumeToken(t);
			if (num2 > num + 1)
			{
				for (int i = num + 1; i < num2; i++)
				{
					dbg.ConsumeHiddenToken(input.Get(i));
				}
			}
		}

		protected internal virtual void ConsumeInitialHiddenTokens()
		{
			int num = input.Index();
			for (int i = 0; i < num; i++)
			{
				dbg.ConsumeHiddenToken(input.Get(i));
			}
			initialStreamState = false;
		}

		public virtual IToken LT(int i)
		{
			if (initialStreamState)
			{
				ConsumeInitialHiddenTokens();
			}
			dbg.LT(i, input.LT(i));
			return input.LT(i);
		}

		public virtual int LA(int i)
		{
			if (initialStreamState)
			{
				ConsumeInitialHiddenTokens();
			}
			dbg.LT(i, input.LT(i));
			return input.LA(i);
		}

		public virtual IToken Get(int i)
		{
			return input.Get(i);
		}

		public virtual int Mark()
		{
			lastMarker = input.Mark();
			dbg.Mark(lastMarker);
			return lastMarker;
		}

		public virtual int Index()
		{
			return input.Index();
		}

		public virtual void Rewind(int marker)
		{
			dbg.Rewind(marker);
			input.Rewind(marker);
		}

		public virtual void Rewind()
		{
			dbg.Rewind();
			input.Rewind(lastMarker);
		}

		public virtual void Release(int marker)
		{
		}

		public virtual void Seek(int index)
		{
			input.Seek(index);
		}

		[Obsolete("Please use property Count instead.")]
		public virtual int Size()
		{
			return Count;
		}

		public override string ToString()
		{
			return input.ToString();
		}

		public virtual string ToString(int start, int stop)
		{
			return input.ToString(start, stop);
		}

		public virtual string ToString(IToken start, IToken stop)
		{
			return input.ToString(start, stop);
		}
	}
}
