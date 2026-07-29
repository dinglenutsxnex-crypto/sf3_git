using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;

namespace Jint.Native.Function
{
	public sealed class FunctionPrototype : FunctionInstance
	{
		private FunctionPrototype(Engine engine)
			: base(engine, null, null, false)
		{
		}

		public static FunctionPrototype CreatePrototypeObject(Engine engine)
		{
			FunctionPrototype functionPrototype = new FunctionPrototype(engine);
			functionPrototype.Extensible = true;
			functionPrototype.Prototype = engine.Object.PrototypeObject;
			functionPrototype.FastAddProperty("length", 0.0, false, false, false);
			return functionPrototype;
		}

		public void Configure()
		{
			FastAddProperty("constructor", base.Engine.Function, true, false, true);
			FastAddProperty("toString", new ClrFunctionInstance(base.Engine, ToString), true, false, true);
			FastAddProperty("apply", new ClrFunctionInstance(base.Engine, Apply, 2), true, false, true);
			FastAddProperty("call", new ClrFunctionInstance(base.Engine, CallImpl, 1), true, false, true);
			FastAddProperty("bind", new ClrFunctionInstance(base.Engine, Bind, 1), true, false, true);
		}

		private JsValue Bind(JsValue thisObj, JsValue[] arguments)
		{
			ICallable callable = thisObj.TryCast<ICallable>(delegate
			{
				throw new JavaScriptException(base.Engine.TypeError);
			});
			JsValue boundThis = arguments.At(0);
			BindFunctionInstance bindFunctionInstance = new BindFunctionInstance(base.Engine);
			bindFunctionInstance.Extensible = true;
			BindFunctionInstance bindFunctionInstance2 = bindFunctionInstance;
			bindFunctionInstance2.TargetFunction = thisObj;
			bindFunctionInstance2.BoundThis = boundThis;
			bindFunctionInstance2.BoundArgs = arguments.Skip(1).ToArray();
			bindFunctionInstance2.Prototype = base.Engine.Function.PrototypeObject;
			FunctionInstance functionInstance = callable as FunctionInstance;
			if (functionInstance != null)
			{
				double val = TypeConverter.ToNumber(functionInstance.Get("length")) - (double)(arguments.Length - 1);
				bindFunctionInstance2.FastAddProperty("length", System.Math.Max(val, 0.0), false, false, false);
			}
			else
			{
				bindFunctionInstance2.FastAddProperty("length", 0.0, false, false, false);
			}
			FunctionInstance throwTypeError = base.Engine.Function.ThrowTypeError;
			bindFunctionInstance2.DefineOwnProperty("caller", new PropertyDescriptor(throwTypeError, throwTypeError, false, false), false);
			bindFunctionInstance2.DefineOwnProperty("arguments", new PropertyDescriptor(throwTypeError, throwTypeError, false, false), false);
			return bindFunctionInstance2;
		}

		private JsValue ToString(JsValue thisObj, JsValue[] arguments)
		{
			FunctionInstance functionInstance = thisObj.TryCast<FunctionInstance>();
			if (functionInstance == null)
			{
				throw new JavaScriptException(base.Engine.TypeError, "Function object expected.");
			}
			return string.Format("function() {{ ... }}");
		}

		public JsValue Apply(JsValue thisObject, JsValue[] arguments)
		{
			ICallable callable = thisObject.TryCast<ICallable>();
			JsValue thisObject2 = arguments.At(0);
			JsValue jsValue = arguments.At(1);
			if (callable == null)
			{
				throw new JavaScriptException(base.Engine.TypeError);
			}
			if (jsValue == Null.Instance || jsValue == Undefined.Instance)
			{
				return callable.Call(thisObject2, Arguments.Empty);
			}
			ObjectInstance objectInstance = jsValue.TryCast<ObjectInstance>();
			if (objectInstance == null)
			{
				throw new JavaScriptException(base.Engine.TypeError);
			}
			double num = objectInstance.Get("length").AsNumber();
			uint num2 = TypeConverter.ToUint32(num);
			List<JsValue> list = new List<JsValue>();
			for (int i = 0; i < num2; i++)
			{
				string propertyName = i.ToString();
				JsValue item = objectInstance.Get(propertyName);
				list.Add(item);
			}
			return callable.Call(thisObject2, list.ToArray());
		}

		public JsValue CallImpl(JsValue thisObject, JsValue[] arguments)
		{
			ICallable callable = thisObject.TryCast<ICallable>();
			if (callable == null)
			{
				throw new JavaScriptException(base.Engine.TypeError);
			}
			return callable.Call(arguments.At(0), (arguments.Length != 0) ? arguments.Skip(1).ToArray() : arguments);
		}

		public override JsValue Call(JsValue thisObject, JsValue[] arguments)
		{
			return Undefined.Instance;
		}
	}
}
