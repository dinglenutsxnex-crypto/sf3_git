using Jint.Native.Function;
using Jint.Native.Object;
using Jint.Runtime;

namespace Jint.Native.Number
{
	public sealed class NumberConstructor : FunctionInstance, IConstructor
	{
		public NumberPrototype PrototypeObject { get; private set; }

		public NumberConstructor(Engine engine)
			: base(engine, null, null, false)
		{
		}

		public static NumberConstructor CreateNumberConstructor(Engine engine)
		{
			NumberConstructor numberConstructor = new NumberConstructor(engine);
			numberConstructor.Extensible = true;
			numberConstructor.Prototype = engine.Function.PrototypeObject;
			numberConstructor.PrototypeObject = NumberPrototype.CreatePrototypeObject(engine, numberConstructor);
			numberConstructor.FastAddProperty("length", 1.0, false, false, false);
			numberConstructor.FastAddProperty("prototype", numberConstructor.PrototypeObject, false, false, false);
			return numberConstructor;
		}

		public void Configure()
		{
			FastAddProperty("MAX_VALUE", double.MaxValue, false, false, false);
			FastAddProperty("MIN_VALUE", double.Epsilon, false, false, false);
			FastAddProperty("NaN", double.NaN, false, false, false);
			FastAddProperty("NEGATIVE_INFINITY", double.NegativeInfinity, false, false, false);
			FastAddProperty("POSITIVE_INFINITY", double.PositiveInfinity, false, false, false);
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			if (arguments.Length == 0)
			{
				return 0.0;
			}
			return TypeConverter.ToNumber(arguments[0]);
		}

		public ObjectInstance Construct(JsValue[] arguments)
		{
			return Construct((arguments.Length <= 0) ? 0.0 : TypeConverter.ToNumber(arguments[0]));
		}

		public NumberInstance Construct(double value)
		{
			NumberInstance numberInstance = new NumberInstance(base.Engine);
			numberInstance.Prototype = PrototypeObject;
			numberInstance.PrimitiveValue = value;
			numberInstance.Extensible = true;
			return numberInstance;
		}
	}
}
