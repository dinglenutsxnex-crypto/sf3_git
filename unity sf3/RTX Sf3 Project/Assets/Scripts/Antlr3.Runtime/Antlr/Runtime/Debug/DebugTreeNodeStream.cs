using System;
using Antlr.Runtime.Tree;

namespace Antlr.Runtime.Debug
{
	public class DebugTreeNodeStream : IIntStream, ITreeNodeStream
	{
		protected IDebugEventListener dbg;

		protected ITreeAdaptor adaptor;

		protected ITreeNodeStream input;

		protected bool initialStreamState = true;

		protected int lastMarker;

		public ITokenStream TokenStream
		{
			get
			{
				return input.TokenStream;
			}
		}

		public string SourceName
		{
			get
			{
				return TokenStream.SourceName;
			}
		}

		public ITreeAdaptor TreeAdaptor
		{
			get
			{
				return adaptor;
			}
		}

		public int Count
		{
			get
			{
				return input.Count;
			}
		}

		public object TreeSource
		{
			get
			{
				return input;
			}
		}

		public virtual bool HasUniqueNavigationNodes
		{
			set
			{
				input.HasUniqueNavigationNodes = value;
			}
		}

		public DebugTreeNodeStream(ITreeNodeStream input, IDebugEventListener dbg)
		{
			this.input = input;
			adaptor = input.TreeAdaptor;
			this.input.HasUniqueNavigationNodes = true;
			SetDebugListener(dbg);
		}

		public void SetDebugListener(IDebugEventListener dbg)
		{
			this.dbg = dbg;
		}

		public void Consume()
		{
			object t = input.LT(1);
			input.Consume();
			dbg.ConsumeNode(t);
		}

		public object Get(int i)
		{
			return input.Get(i);
		}

		public object LT(int i)
		{
			object obj = input.LT(i);
			dbg.LT(i, obj);
			return obj;
		}

		public int LA(int i)
		{
			object t = input.LT(i);
			int nodeType = adaptor.GetNodeType(t);
			dbg.LT(i, t);
			return nodeType;
		}

		public int Mark()
		{
			lastMarker = input.Mark();
			dbg.Mark(lastMarker);
			return lastMarker;
		}

		public int Index()
		{
			return input.Index();
		}

		public void Rewind(int marker)
		{
			dbg.Rewind(marker);
			input.Rewind(marker);
		}

		public void Rewind()
		{
			dbg.Rewind();
			input.Rewind(lastMarker);
		}

		public void Release(int marker)
		{
		}

		public void Seek(int index)
		{
			input.Seek(index);
		}

		[Obsolete("Please use property Count instead.")]
		public int Size()
		{
			return Count;
		}

		public void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
		{
			input.ReplaceChildren(parent, startChildIndex, stopChildIndex, t);
		}

		public string ToString(object start, object stop)
		{
			return input.ToString(start, stop);
		}
	}
}
