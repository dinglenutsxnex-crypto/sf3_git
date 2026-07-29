using Jint.Runtime;
using Jint.Runtime.Interop;

namespace Jint.Native.Boolean
{
	public sealed class BooleanPrototype : BooleanInstance
	{
		private BooleanPrototype(Engine engine)
			: base(engine)
		{
		}

		public static BooleanPrototype CreatePrototypeObject(Engine engine, BooleanConstructor booleanConstructor)
		{
			BooleanPrototype booleanPrototype = new BooleanPrototype(engine);
			booleanPrototype.Prototype = engine.Object.PrototypeObject;
			booleanPrototype.PrimitiveValue = false;
			booleanPrototype.Extensible = true;
			booleanPrototype.FastAddProperty("constructor", booleanConstructor, true, false, true);
			return booleanPrototype;
		}

		public void Configure()
		{
			FastAddProperty("toString", new ClrFunctionInstance(base.Engine, ToBooleanString), true, false, true);
			FastAddProperty("valueOf", new ClrFunctionInstance(base.Engine, ValueOf), true, false, true);
		}

		private JsValue ValueOf(JsValue thisObj, JsValue[] arguments)
		{
			if (thisObj.IsBoolean())
			{
				return thisObj;
			}
			BooleanInstance booleanInstance = thisObj.TryCast<BooleanInstance>();
			if (booleanInstance != null)
			{
				return booleanInstance.PrimitiveValue;
			}
			throw new JavaScriptException(base.Engine.TypeError);
		}

		private JsValue ToBooleanString(JsValue thisObj, JsValue[] arguments)
		{
			JsValue jsValue = ValueOf(thisObj, Arguments.Empty);
			return (!jsValue.AsBoolean()) ? "false" : "true";
		}
	}
}
