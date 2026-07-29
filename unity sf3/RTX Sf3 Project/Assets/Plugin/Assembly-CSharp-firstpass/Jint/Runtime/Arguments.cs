using Jint.Native;

namespace Jint.Runtime
{
	public static class Arguments
	{
		public static JsValue[] Empty = new JsValue[0];

		public static JsValue[] From(params JsValue[] o)
		{
			return o;
		}

		public static JsValue At(this JsValue[] args, int index, JsValue undefinedValue)
		{
			return (args.Length <= index) ? undefinedValue : args[index];
		}

		public static JsValue At(this JsValue[] args, int index)
		{
			return args.At(index, Undefined.Instance);
		}
	}
}
