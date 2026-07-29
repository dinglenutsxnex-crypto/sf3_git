using System.Text.RegularExpressions;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace Jint.Native.RegExp
{
	public sealed class RegExpPrototype : RegExpInstance
	{
		private RegExpPrototype(Engine engine)
			: base(engine)
		{
		}

		public static RegExpPrototype CreatePrototypeObject(Engine engine, RegExpConstructor regExpConstructor)
		{
			RegExpPrototype regExpPrototype = new RegExpPrototype(engine);
			regExpPrototype.Prototype = engine.Object.PrototypeObject;
			regExpPrototype.Extensible = true;
			regExpPrototype.FastAddProperty("constructor", regExpConstructor, true, false, true);
			return regExpPrototype;
		}

		public void Configure()
		{
			FastAddProperty("toString", new ClrFunctionInstance(base.Engine, ToRegExpString), true, false, true);
			FastAddProperty("exec", new ClrFunctionInstance(base.Engine, Exec, 1), true, false, true);
			FastAddProperty("test", new ClrFunctionInstance(base.Engine, Test, 1), true, false, true);
			FastAddProperty("global", false, false, false, false);
			FastAddProperty("ignoreCase", false, false, false, false);
			FastAddProperty("multiline", false, false, false, false);
			FastAddProperty("source", "(?:)", false, false, false);
			FastAddProperty("lastIndex", 0.0, true, false, false);
		}

		private JsValue ToRegExpString(JsValue thisObj, JsValue[] arguments)
		{
			RegExpInstance regExpInstance = thisObj.TryCast<RegExpInstance>();
			return "/" + regExpInstance.Source + "/" + ((!regExpInstance.Flags.Contains("g")) ? string.Empty : "g") + ((!regExpInstance.Flags.Contains("i")) ? string.Empty : "i") + ((!regExpInstance.Flags.Contains("m")) ? string.Empty : "m");
		}

		private JsValue Test(JsValue thisObj, JsValue[] arguments)
		{
			ObjectInstance objectInstance = TypeConverter.ToObject(base.Engine, thisObj);
			if (objectInstance.Class != "RegExp")
			{
				throw new JavaScriptException(base.Engine.TypeError);
			}
			JsValue jsValue = Exec(objectInstance, arguments);
			return jsValue != Null.Instance;
		}

		internal JsValue Exec(JsValue thisObj, JsValue[] arguments)
		{
			RegExpInstance regExpInstance = TypeConverter.ToObject(base.Engine, thisObj) as RegExpInstance;
			if (regExpInstance == null)
			{
				throw new JavaScriptException(base.Engine.TypeError);
			}
			string text = TypeConverter.ToString(arguments.At(0));
			int length = text.Length;
			double num = TypeConverter.ToNumber(regExpInstance.Get("lastIndex"));
			double num2 = TypeConverter.ToInteger(num);
			bool global = regExpInstance.Global;
			if (!global)
			{
				num2 = 0.0;
			}
			if (regExpInstance.Source == "(?:)")
			{
				ObjectInstance objectInstance = InitReturnValueArray(base.Engine.Array.Construct(Arguments.Empty), text, 1, 0);
				objectInstance.DefineOwnProperty("0", new PropertyDescriptor(string.Empty, true, true, true), true);
				return objectInstance;
			}
			Match match = null;
			if (num2 < 0.0 || num2 > (double)length)
			{
				regExpInstance.Put("lastIndex", 0.0, true);
				return Null.Instance;
			}
			match = regExpInstance.Match(text, num2);
			if (!match.Success)
			{
				regExpInstance.Put("lastIndex", 0.0, true);
				return Null.Instance;
			}
			int num3 = match.Index + match.Length;
			if (global)
			{
				regExpInstance.Put("lastIndex", num3, true);
			}
			int count = match.Groups.Count;
			int index = match.Index;
			ObjectInstance objectInstance2 = InitReturnValueArray(base.Engine.Array.Construct(Arguments.Empty), text, count, index);
			for (int i = 0; i < count; i++)
			{
				Group group = match.Groups[i];
				JsValue value = ((!group.Success) ? Undefined.Instance : ((JsValue)group.Value));
				objectInstance2.DefineOwnProperty(i.ToString(), new PropertyDescriptor(value, true, true, true), true);
			}
			return objectInstance2;
		}

		private static ObjectInstance InitReturnValueArray(ObjectInstance array, string inputValue, int lengthValue, int indexValue)
		{
			array.DefineOwnProperty("index", new PropertyDescriptor(indexValue, true, true, true), true);
			array.DefineOwnProperty("input", new PropertyDescriptor(inputValue, true, true, true), true);
			array.DefineOwnProperty("length", new PropertyDescriptor(lengthValue, true, false, false), true);
			return array;
		}
	}
}
