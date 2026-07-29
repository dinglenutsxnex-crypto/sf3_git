using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Interop;

namespace Jint.Native.String
{
	public sealed class StringConstructor : FunctionInstance, IConstructor
	{
		public StringPrototype PrototypeObject { get; private set; }

		public StringConstructor(Engine engine)
			: base(engine, null, null, false)
		{
		}

		public static StringConstructor CreateStringConstructor(Engine engine)
		{
			StringConstructor stringConstructor = new StringConstructor(engine);
			stringConstructor.Extensible = true;
			stringConstructor.Prototype = engine.Function.PrototypeObject;
			stringConstructor.PrototypeObject = StringPrototype.CreatePrototypeObject(engine, stringConstructor);
			stringConstructor.FastAddProperty("length", 1.0, false, false, false);
			stringConstructor.FastAddProperty("prototype", stringConstructor.PrototypeObject, false, false, false);
			return stringConstructor;
		}

		public void Configure()
		{
			FastAddProperty("fromCharCode", new ClrFunctionInstance(base.Engine, FromCharCode, 1), true, false, true);
		}

		private static JsValue FromCharCode(JsValue thisObj, JsValue[] arguments)
		{
			char[] array = new char[arguments.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (char)TypeConverter.ToUint16(arguments[i]);
			}
			return new string(array);
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			if (arguments.Length == 0)
			{
				return string.Empty;
			}
			return TypeConverter.ToString(arguments[0]);
		}

		public ObjectInstance Construct(JsValue[] arguments)
		{
			return Construct((arguments.Length <= 0) ? string.Empty : TypeConverter.ToString(arguments[0]));
		}

		public StringInstance Construct(string value)
		{
			StringInstance stringInstance = new StringInstance(base.Engine);
			stringInstance.Prototype = PrototypeObject;
			stringInstance.PrimitiveValue = value;
			stringInstance.Extensible = true;
			stringInstance.FastAddProperty("length", value.Length, false, false, false);
			return stringInstance;
		}
	}
}
