using System;
using System.Collections.Generic;
using System.Diagnostics;
using Jint.Native.Array;
using Jint.Native.Boolean;
using Jint.Native.Date;
using Jint.Native.Function;
using Jint.Native.Number;
using Jint.Native.Object;
using Jint.Native.RegExp;
using Jint.Native.String;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Interop;
using UnityEngine;

namespace Jint.Native
{
	[DebuggerTypeProxy(typeof(JsValueDebugView))]
	public class JsValue : IEquatable<JsValue>
	{
		internal class JsValueDebugView
		{
			public string Value;

			public JsValueDebugView(JsValue value)
			{
				switch (value.Type)
				{
				case Jint.Runtime.Types.None:
					Value = "None";
					break;
				case Jint.Runtime.Types.Undefined:
					Value = "undefined";
					break;
				case Jint.Runtime.Types.Null:
					Value = "null";
					break;
				case Jint.Runtime.Types.Boolean:
					Value = value.AsBoolean() + " (bool)";
					break;
				case Jint.Runtime.Types.String:
					Value = value.AsString() + " (string)";
					break;
				case Jint.Runtime.Types.Number:
					Value = value.AsNumber() + " (number)";
					break;
				case Jint.Runtime.Types.Object:
					Value = value.AsObject().GetType().Name;
					break;
				default:
					Value = "Unknown";
					break;
				}
			}
		}

		public static readonly JsValue Undefined = new JsValue(Jint.Runtime.Types.Undefined);

		public static readonly JsValue Null = new JsValue(Jint.Runtime.Types.Null);

		public static readonly JsValue False = new JsValue(false);

		public static readonly JsValue True = new JsValue(true);

		private readonly double _double;

		private readonly object _object;

		private readonly Jint.Runtime.Types _type;

		public Jint.Runtime.Types Type
		{
			get
			{
				return _type;
			}
		}

		public JsValue(bool value)
		{
			_double = ((!value) ? 0.0 : 1.0);
			_object = null;
			_type = Jint.Runtime.Types.Boolean;
		}

		public JsValue(double value)
		{
			_object = null;
			_type = Jint.Runtime.Types.Number;
			_double = value;
		}

		public JsValue(string value)
		{
			_double = double.NaN;
			_object = value;
			_type = Jint.Runtime.Types.String;
		}

		public JsValue(ObjectInstance value)
		{
			_double = double.NaN;
			_type = Jint.Runtime.Types.Object;
			_object = value;
		}

		private JsValue(Jint.Runtime.Types type)
		{
			_double = double.NaN;
			_object = null;
			_type = type;
		}

		public T TryCast<T>(Action<JsValue> fail = null) where T : class
		{
			if (IsObject())
			{
				ObjectInstance objectInstance = AsObject();
				T val = objectInstance as T;
				if (val != null)
				{
					return val;
				}
			}
			if (fail != null)
			{
				fail(this);
			}
			return (T)null;
		}

		public T As<T>() where T : ObjectInstance
		{
			return _object as T;
		}

		public bool Is<T>()
		{
			return IsObject() && AsObject() is T;
		}

		public bool IsPrimitive()
		{
			return _type != Jint.Runtime.Types.Object && _type != Jint.Runtime.Types.None;
		}

		public bool IsUndefined()
		{
			return _type == Jint.Runtime.Types.Undefined;
		}

		public bool IsArray()
		{
			return IsObject() && AsObject() is ArrayInstance;
		}

		public bool IsDate()
		{
			return IsObject() && AsObject() is DateInstance;
		}

		public bool IsRegExp()
		{
			return IsObject() && AsObject() is RegExpInstance;
		}

		public bool IsObject()
		{
			return _type == Jint.Runtime.Types.Object;
		}

		public bool IsString()
		{
			return _type == Jint.Runtime.Types.String;
		}

		public bool IsNumber()
		{
			return _type == Jint.Runtime.Types.Number;
		}

		public bool IsBoolean()
		{
			return _type == Jint.Runtime.Types.Boolean;
		}

		public bool IsNull()
		{
			return _type == Jint.Runtime.Types.Null;
		}

		public ObjectInstance AsObject()
		{
			if (_type != Jint.Runtime.Types.Object)
			{
				throw new ArgumentException("The value is not an object but " + _type);
			}
			return _object as ObjectInstance;
		}

		public Dictionary<string, JsValue> AsDictionary()
		{
			return AsObject().AsDictionary();
		}

		public ArrayInstance AsArray()
		{
			if (!IsArray())
			{
				throw new ArgumentException("The value is not an array");
			}
			return _object as ArrayInstance;
		}

		public DateInstance AsDate()
		{
			if (!IsDate())
			{
				throw new ArgumentException("The value is not a date");
			}
			return _object as DateInstance;
		}

		public RegExpInstance AsRegExp()
		{
			if (!IsRegExp())
			{
				throw new ArgumentException("The value is not a date");
			}
			return _object as RegExpInstance;
		}

		public bool AsBoolean()
		{
			if (_type != Jint.Runtime.Types.Boolean)
			{
				throw new ArgumentException("The value is not a boolean");
			}
			return _double > 0.0;
		}

		public string AsString()
		{
			if (_type != Jint.Runtime.Types.String)
			{
				throw new ArgumentException("The value is not a string");
			}
			if (_object == null)
			{
				throw new ArgumentException("The value is not defined");
			}
			return _object as string;
		}

		public double AsNumber()
		{
			if (_type != Jint.Runtime.Types.Number)
			{
				try
				{
					if (ToString().Equals("undefined"))
					{
						UnityEngine.Debug.LogError("got undefined form JS");
						return 0.0;
					}
					return double.Parse(ToString());
				}
				catch (Exception)
				{
					throw new Exception("can't parse [" + ToString() + "] as double");
				}
			}
			return _double;
		}

		public int AsInteger()
		{
			return (int)AsNumber();
		}

		public long AsLong()
		{
			return (long)AsNumber();
		}

		public float AsFloat()
		{
			return (float)AsNumber();
		}

		public bool Equals(JsValue other)
		{
			if (other == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			if (_type != other._type)
			{
				return false;
			}
			switch (_type)
			{
			case Jint.Runtime.Types.None:
				return false;
			case Jint.Runtime.Types.Undefined:
				return true;
			case Jint.Runtime.Types.Null:
				return true;
			case Jint.Runtime.Types.Boolean:
			case Jint.Runtime.Types.Number:
				return _double == other._double;
			case Jint.Runtime.Types.String:
			case Jint.Runtime.Types.Object:
				return _object == other._object;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public static JsValue FromObject(Engine engine, object value)
		{
			if (value == null)
			{
				return Null;
			}
			foreach (IObjectConverter objectConverter in engine.Options._ObjectConverters)
			{
				JsValue result;
				if (objectConverter.TryConvert(value, out result))
				{
					return result;
				}
			}
			Type type = value.GetType();
			Dictionary<Type, Func<Engine, object, JsValue>> typeMappers = Engine.TypeMappers;
			Func<Engine, object, JsValue> value2;
			if (typeMappers.TryGetValue(type, out value2))
			{
				return value2(engine, value);
			}
			ObjectInstance objectInstance = value as ObjectInstance;
			if (objectInstance != null)
			{
				typeMappers.Add(type, (Engine e, object v) => new JsValue((ObjectInstance)v));
				return new JsValue(objectInstance);
			}
			System.Array array = value as System.Array;
			if (array != null)
			{
				Func<Engine, object, JsValue> func = delegate(Engine e, object v)
				{
					System.Array array2 = (System.Array)v;
					ObjectInstance objectInstance2 = engine.Array.Construct(Arguments.Empty);
					foreach (object item in array2)
					{
						JsValue jsValue = FromObject(engine, item);
						engine.Array.PrototypeObject.Push(objectInstance2, Arguments.From(jsValue));
					}
					return objectInstance2;
				};
				typeMappers.Add(type, func);
				return func(engine, array);
			}
			Delegate @delegate = value as Delegate;
			if ((object)@delegate != null)
			{
				return new DelegateWrapper(engine, @delegate);
			}
			if (value.GetType().IsEnum)
			{
				return new JsValue((int)value);
			}
			return new ObjectWrapper(engine, value);
		}

		public object ToObject()
		{
			switch (_type)
			{
			case Jint.Runtime.Types.None:
			case Jint.Runtime.Types.Undefined:
			case Jint.Runtime.Types.Null:
				return null;
			case Jint.Runtime.Types.String:
				return _object;
			case Jint.Runtime.Types.Boolean:
				return _double != 0.0;
			case Jint.Runtime.Types.Number:
				return _double;
			case Jint.Runtime.Types.Object:
			{
				IObjectWrapper objectWrapper = _object as IObjectWrapper;
				if (objectWrapper != null)
				{
					return objectWrapper.Target;
				}
				switch ((_object as ObjectInstance).Class)
				{
				case "Array":
				{
					ArrayInstance arrayInstance = _object as ArrayInstance;
					if (arrayInstance == null)
					{
						break;
					}
					int num = TypeConverter.ToInt32(arrayInstance.Get("length"));
					object[] array = new object[num];
					for (int i = 0; i < num; i++)
					{
						string propertyName = i.ToString();
						if (arrayInstance.HasProperty(propertyName))
						{
							JsValue jsValue = arrayInstance.Get(propertyName);
							array[i] = jsValue.ToObject();
						}
						else
						{
							array[i] = null;
						}
					}
					return array;
				}
				case "String":
				{
					StringInstance stringInstance = _object as StringInstance;
					if (stringInstance != null)
					{
						return stringInstance.PrimitiveValue.AsString();
					}
					break;
				}
				case "Date":
				{
					DateInstance dateInstance = _object as DateInstance;
					if (dateInstance != null)
					{
						return dateInstance.ToDateTime();
					}
					break;
				}
				case "Boolean":
				{
					BooleanInstance booleanInstance = _object as BooleanInstance;
					if (booleanInstance != null)
					{
						return booleanInstance.PrimitiveValue.AsBoolean();
					}
					break;
				}
				case "Function":
				{
					FunctionInstance functionInstance = _object as FunctionInstance;
					if (functionInstance != null)
					{
						return new Func<JsValue, JsValue[], JsValue>(functionInstance.Call);
					}
					break;
				}
				case "Number":
				{
					NumberInstance numberInstance = _object as NumberInstance;
					if (numberInstance != null)
					{
						return numberInstance.PrimitiveValue.AsNumber();
					}
					break;
				}
				case "RegExp":
				{
					RegExpInstance regExpInstance = _object as RegExpInstance;
					if (regExpInstance != null)
					{
						return regExpInstance.Value;
					}
					break;
				}
				case "Arguments":
				case "Object":
				{
					IDictionary<string, object> dictionary = new Dictionary<string, object>();
					{
						foreach (KeyValuePair<string, PropertyDescriptor> ownProperty in (_object as ObjectInstance).GetOwnProperties())
						{
							if (ownProperty.Value.Enumerable.HasValue && ownProperty.Value.Enumerable.Value)
							{
								dictionary.Add(ownProperty.Key, (_object as ObjectInstance).Get(ownProperty.Key).ToObject());
							}
						}
						return dictionary;
					}
				}
				}
				return _object;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		public JsValue Invoke(params JsValue[] arguments)
		{
			return Invoke(Undefined, arguments);
		}

		public JsValue Invoke(JsValue thisObj, JsValue[] arguments)
		{
			ICallable callable = TryCast<ICallable>();
			if (callable == null)
			{
				throw new ArgumentException("Can only invoke functions");
			}
			return callable.Call(thisObj, arguments);
		}

		public override string ToString()
		{
			switch (Type)
			{
			case Jint.Runtime.Types.None:
				return "None";
			case Jint.Runtime.Types.Undefined:
				return "undefined";
			case Jint.Runtime.Types.Null:
				return "null";
			case Jint.Runtime.Types.Boolean:
				return (_double == 0.0) ? bool.FalseString : bool.TrueString;
			case Jint.Runtime.Types.Number:
				return _double.ToString();
			case Jint.Runtime.Types.String:
			case Jint.Runtime.Types.Object:
				return _object.ToString();
			default:
				return string.Empty;
			}
		}

		public static bool operator ==(JsValue a, JsValue b)
		{
			if ((object)a == null)
			{
				if ((object)b == null)
				{
					return true;
				}
				return false;
			}
			return a.Equals(b);
		}

		public static bool operator !=(JsValue a, JsValue b)
		{
			if ((object)a == null)
			{
				if ((object)b == null)
				{
					return false;
				}
				return true;
			}
			return !a.Equals(b);
		}

		public static implicit operator JsValue(double value)
		{
			return new JsValue(value);
		}

		public static implicit operator JsValue(bool value)
		{
			return new JsValue(value);
		}

		public static implicit operator JsValue(string value)
		{
			return new JsValue(value);
		}

		public static implicit operator JsValue(ObjectInstance value)
		{
			return new JsValue(value);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			return obj is JsValue && Equals((JsValue)obj);
		}

		public override int GetHashCode()
		{
			int num = 0;
			num = (num * 397) ^ _double.GetHashCode();
			num = (num * 397) ^ ((_object != null) ? _object.GetHashCode() : 0);
			return (num * 397) ^ (int)_type;
		}
	}
}
