using System.Collections;
using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Interop;

namespace Jint.Native.Array
{
	public sealed class ArrayConstructor : FunctionInstance, IConstructor
	{
		public ArrayPrototype PrototypeObject { get; private set; }

		private ArrayConstructor(Engine engine)
			: base(engine, null, null, false)
		{
		}

		public static ArrayConstructor CreateArrayConstructor(Engine engine)
		{
			ArrayConstructor arrayConstructor = new ArrayConstructor(engine);
			arrayConstructor.Extensible = true;
			arrayConstructor.Prototype = engine.Function.PrototypeObject;
			arrayConstructor.PrototypeObject = ArrayPrototype.CreatePrototypeObject(engine, arrayConstructor);
			arrayConstructor.FastAddProperty("length", 1.0, false, false, false);
			arrayConstructor.FastAddProperty("prototype", arrayConstructor.PrototypeObject, false, false, false);
			return arrayConstructor;
		}

		public void Configure()
		{
			FastAddProperty("isArray", new ClrFunctionInstance(base.Engine, IsArray, 1), true, false, true);
		}

		private JsValue IsArray(JsValue thisObj, JsValue[] arguments)
		{
			if (arguments.Length == 0)
			{
				return false;
			}
			JsValue jsValue = arguments.At(0);
			return jsValue.IsObject() && jsValue.AsObject().Class == "Array";
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			return Construct(arguments);
		}

		public ObjectInstance Construct(JsValue[] arguments)
		{
			ArrayInstance arrayInstance = new ArrayInstance(base.Engine);
			arrayInstance.Prototype = PrototypeObject;
			arrayInstance.Extensible = true;
			if (arguments.Length == 1 && arguments.At(0).IsNumber())
			{
				uint num = TypeConverter.ToUint32(arguments.At(0));
				if (!TypeConverter.ToNumber(arguments[0]).Equals(num))
				{
					throw new JavaScriptException(base.Engine.RangeError, "Invalid array length");
				}
				arrayInstance.FastAddProperty("length", num, true, false, false);
			}
			else if (arguments.Length == 1 && arguments.At(0).IsObject() && arguments.At(0).As<ObjectWrapper>() != null)
			{
				IEnumerable enumerable = arguments.At(0).As<ObjectWrapper>().Target as IEnumerable;
				if (enumerable != null)
				{
					ObjectInstance objectInstance = base.Engine.Array.Construct(Arguments.Empty);
					{
						foreach (object item in enumerable)
						{
							JsValue jsValue = JsValue.FromObject(base.Engine, item);
							base.Engine.Array.PrototypeObject.Push(objectInstance, Arguments.From(jsValue));
						}
						return objectInstance;
					}
				}
			}
			else
			{
				arrayInstance.FastAddProperty("length", 0.0, true, false, false);
				PrototypeObject.Push(arrayInstance, arguments);
			}
			return arrayInstance;
		}
	}
}
