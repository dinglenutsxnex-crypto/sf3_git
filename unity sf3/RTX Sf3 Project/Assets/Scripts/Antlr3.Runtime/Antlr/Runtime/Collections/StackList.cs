using System.Collections;

namespace Antlr.Runtime.Collections
{
	public class StackList : ArrayList
	{
		public void Push(object item)
		{
			Add(item);
		}

		public object Pop()
		{
			object result = this[Count - 1];
			RemoveAt(Count - 1);
			return result;
		}

		public object Peek()
		{
			return this[Count - 1];
		}
	}
}
