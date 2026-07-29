using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Interop;

namespace Jint.Native.Error
{
	public sealed class ErrorPrototype : ErrorInstance
	{
		private ErrorPrototype(Engine engine, string name)
			: base(engine, name)
		{
		}

		public static ErrorPrototype CreatePrototypeObject(Engine engine, ErrorConstructor errorConstructor, string name)
		{
			ErrorPrototype errorPrototype = new ErrorPrototype(engine, name);
			errorPrototype.Extensible = true;
			ErrorPrototype errorPrototype2 = errorPrototype;
			errorPrototype2.FastAddProperty("constructor", errorConstructor, true, false, true);
			errorPrototype2.FastAddProperty("message", string.Empty, true, false, true);
			if (name != "Error")
			{
				errorPrototype2.Prototype = engine.Error.PrototypeObject;
			}
			else
			{
				errorPrototype2.Prototype = engine.Object.PrototypeObject;
			}
			return errorPrototype2;
		}

		public void Configure()
		{
			FastAddProperty("toString", new ClrFunctionInstance(base.Engine, ToString), true, false, true);
		}

		public JsValue ToString(JsValue thisObject, JsValue[] arguments)
		{
			ObjectInstance objectInstance = thisObject.TryCast<ObjectInstance>();
			if (objectInstance == null)
			{
				throw new JavaScriptException(base.Engine.TypeError);
			}
			string text = TypeConverter.ToString(objectInstance.Get("name"));
			JsValue jsValue = objectInstance.Get("message");
			string text2 = ((!(jsValue == Undefined.Instance)) ? TypeConverter.ToString(jsValue) : string.Empty);
			if (text == string.Empty)
			{
				return text2;
			}
			if (text2 == string.Empty)
			{
				return text;
			}
			return text + ": " + text2;
		}
	}
}
