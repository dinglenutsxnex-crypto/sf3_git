using System.Collections.Generic;
using Jint.Runtime;
using Jint.Runtime.Descriptors;

namespace Jint.Native.Object
{
	public class ObjectInstance
	{
		public Engine Engine { get; set; }

		protected IDictionary<string, PropertyDescriptor> Properties { get; private set; }

		public ObjectInstance Prototype { get; set; }

		public bool Extensible { get; set; }

		public virtual string Class
		{
			get
			{
				return "Object";
			}
		}

		public virtual JsValue this[string index]
		{
			get
			{
				return Get(index);
			}
		}

		public ObjectInstance(Engine engine)
		{
			Engine = engine;
			Properties = new MruPropertyCache2<string, PropertyDescriptor>();
		}

		public virtual IEnumerable<KeyValuePair<string, PropertyDescriptor>> GetOwnProperties()
		{
			EnsureInitialized();
			return Properties;
		}

		public Dictionary<string, JsValue> AsDictionary()
		{
			EnsureInitialized();
			Dictionary<string, JsValue> dictionary = new Dictionary<string, JsValue>();
			foreach (KeyValuePair<string, PropertyDescriptor> property in Properties)
			{
				dictionary.Add(property.Key, Get(property.Key));
			}
			return dictionary;
		}

		public virtual bool HasOwnProperty(string p)
		{
			EnsureInitialized();
			return Properties.ContainsKey(p);
		}

		public virtual void RemoveOwnProperty(string p)
		{
			EnsureInitialized();
			Properties.Remove(p);
		}

		public virtual JsValue Get(string propertyName)
		{
			PropertyDescriptor property = GetProperty(propertyName);
			if (property == PropertyDescriptor.Undefined)
			{
				return JsValue.Undefined;
			}
			if (property.IsDataDescriptor())
			{
				JsValue value = property.Value;
				return (!(value != null)) ? Undefined.Instance : value;
			}
			JsValue jsValue = ((!(property.Get != null)) ? Undefined.Instance : property.Get);
			if (jsValue.IsUndefined())
			{
				return Undefined.Instance;
			}
			ICallable callable = jsValue.TryCast<ICallable>();
			return callable.Call(this, Arguments.Empty);
		}

		public virtual PropertyDescriptor GetOwnProperty(string propertyName)
		{
			EnsureInitialized();
			PropertyDescriptor value;
			if (Properties.TryGetValue(propertyName, out value))
			{
				return value;
			}
			return PropertyDescriptor.Undefined;
		}

		protected virtual void SetOwnProperty(string propertyName, PropertyDescriptor desc)
		{
			EnsureInitialized();
			Properties[propertyName] = desc;
		}

		public PropertyDescriptor GetProperty(string propertyName)
		{
			PropertyDescriptor ownProperty = GetOwnProperty(propertyName);
			if (ownProperty != PropertyDescriptor.Undefined)
			{
				return ownProperty;
			}
			if (Prototype == null)
			{
				return PropertyDescriptor.Undefined;
			}
			return Prototype.GetProperty(propertyName);
		}

		public virtual void Put(string propertyName, JsValue value, bool throwOnError)
		{
			if (!CanPut(propertyName))
			{
				if (throwOnError)
				{
					throw new JavaScriptException(Engine.TypeError);
				}
				return;
			}
			PropertyDescriptor ownProperty = GetOwnProperty(propertyName);
			if (ownProperty.IsDataDescriptor())
			{
				ownProperty.Value = value;
				return;
			}
			PropertyDescriptor property = GetProperty(propertyName);
			if (property.IsAccessorDescriptor())
			{
				ICallable callable = property.Set.TryCast<ICallable>();
				callable.Call(new JsValue(this), new JsValue[1] { value });
			}
			else
			{
				PropertyDescriptor desc = new PropertyDescriptor(value, true, true, true);
				DefineOwnProperty(propertyName, desc, throwOnError);
			}
		}

		public bool CanPut(string propertyName)
		{
			PropertyDescriptor ownProperty = GetOwnProperty(propertyName);
			if (ownProperty != PropertyDescriptor.Undefined)
			{
				if (ownProperty.IsAccessorDescriptor())
				{
					if (ownProperty.Set == null || ownProperty.Set.IsUndefined())
					{
						return false;
					}
					return true;
				}
				return ownProperty.Writable.HasValue && ownProperty.Writable.Value;
			}
			if (Prototype == null)
			{
				return Extensible;
			}
			PropertyDescriptor property = Prototype.GetProperty(propertyName);
			if (property == PropertyDescriptor.Undefined)
			{
				return Extensible;
			}
			if (property.IsAccessorDescriptor())
			{
				if (property.Set == null || property.Set.IsUndefined())
				{
					return false;
				}
				return true;
			}
			if (!Extensible)
			{
				return false;
			}
			return property.Writable.HasValue && property.Writable.Value;
		}

		public bool HasProperty(string propertyName)
		{
			return GetProperty(propertyName) != PropertyDescriptor.Undefined;
		}

		public virtual bool Delete(string propertyName, bool throwOnError)
		{
			PropertyDescriptor ownProperty = GetOwnProperty(propertyName);
			if (ownProperty == PropertyDescriptor.Undefined)
			{
				return true;
			}
			if (ownProperty.Configurable.HasValue && ownProperty.Configurable.Value)
			{
				RemoveOwnProperty(propertyName);
				return true;
			}
			if (throwOnError)
			{
				throw new JavaScriptException(Engine.TypeError);
			}
			return false;
		}

		public JsValue DefaultValue(Types hint)
		{
			EnsureInitialized();
			if (hint == Types.String || (hint == Types.None && Class == "Date"))
			{
				ICallable callable = Get("toString").TryCast<ICallable>();
				if (callable != null)
				{
					JsValue jsValue = callable.Call(new JsValue(this), Arguments.Empty);
					if (jsValue.IsPrimitive())
					{
						return jsValue;
					}
				}
				ICallable callable2 = Get("valueOf").TryCast<ICallable>();
				if (callable2 != null)
				{
					JsValue jsValue2 = callable2.Call(new JsValue(this), Arguments.Empty);
					if (jsValue2.IsPrimitive())
					{
						return jsValue2;
					}
				}
				throw new JavaScriptException(Engine.TypeError);
			}
			if (hint == Types.Number || hint == Types.None)
			{
				ICallable callable3 = Get("valueOf").TryCast<ICallable>();
				if (callable3 != null)
				{
					JsValue jsValue3 = callable3.Call(new JsValue(this), Arguments.Empty);
					if (jsValue3.IsPrimitive())
					{
						return jsValue3;
					}
				}
				ICallable callable4 = Get("toString").TryCast<ICallable>();
				if (callable4 != null)
				{
					JsValue jsValue4 = callable4.Call(new JsValue(this), Arguments.Empty);
					if (jsValue4.IsPrimitive())
					{
						return jsValue4;
					}
				}
				throw new JavaScriptException(Engine.TypeError);
			}
			return ToString();
		}

		public virtual bool DefineOwnProperty(string propertyName, PropertyDescriptor desc, bool throwOnError)
		{
			PropertyDescriptor propertyDescriptor = GetOwnProperty(propertyName);
			if (propertyDescriptor == desc)
			{
				return true;
			}
			if (propertyDescriptor == PropertyDescriptor.Undefined)
			{
				if (!Extensible)
				{
					if (throwOnError)
					{
						throw new JavaScriptException(Engine.TypeError);
					}
					return false;
				}
				if (desc.IsGenericDescriptor() || desc.IsDataDescriptor())
				{
					SetOwnProperty(propertyName, new PropertyDescriptor(desc)
					{
						Value = ((!(desc.Value != null)) ? JsValue.Undefined : desc.Value),
						Writable = (desc.Writable.HasValue && desc.Writable.Value),
						Enumerable = (desc.Enumerable.HasValue && desc.Enumerable.Value),
						Configurable = (desc.Configurable.HasValue && desc.Configurable.Value)
					});
				}
				else
				{
					SetOwnProperty(propertyName, new PropertyDescriptor(desc)
					{
						Get = desc.Get,
						Set = desc.Set,
						Enumerable = ((!desc.Enumerable.HasValue) ? new bool?(false) : desc.Enumerable),
						Configurable = ((!desc.Configurable.HasValue) ? new bool?(false) : desc.Configurable)
					});
				}
				return true;
			}
			if (!propertyDescriptor.Configurable.HasValue && !propertyDescriptor.Enumerable.HasValue && !propertyDescriptor.Writable.HasValue && propertyDescriptor.Get == null && propertyDescriptor.Set == null && propertyDescriptor.Value == null)
			{
				return true;
			}
			bool? configurable = propertyDescriptor.Configurable;
			bool valueOrDefault = configurable.GetValueOrDefault();
			bool? configurable2 = desc.Configurable;
			if (valueOrDefault == configurable2.GetValueOrDefault() && configurable.HasValue == configurable2.HasValue)
			{
				bool? writable = propertyDescriptor.Writable;
				bool valueOrDefault2 = writable.GetValueOrDefault();
				bool? writable2 = desc.Writable;
				if (valueOrDefault2 == writable2.GetValueOrDefault() && writable.HasValue == writable2.HasValue)
				{
					bool? enumerable = propertyDescriptor.Enumerable;
					bool valueOrDefault3 = enumerable.GetValueOrDefault();
					bool? enumerable2 = desc.Enumerable;
					if (valueOrDefault3 == enumerable2.GetValueOrDefault() && enumerable.HasValue == enumerable2.HasValue && ((propertyDescriptor.Get == null && desc.Get == null) || (propertyDescriptor.Get != null && desc.Get != null && ExpressionInterpreter.SameValue(propertyDescriptor.Get, desc.Get))) && ((propertyDescriptor.Set == null && desc.Set == null) || (propertyDescriptor.Set != null && desc.Set != null && ExpressionInterpreter.SameValue(propertyDescriptor.Set, desc.Set))) && ((propertyDescriptor.Value == null && desc.Value == null) || (propertyDescriptor.Value != null && desc.Value != null && ExpressionInterpreter.StrictlyEqual(propertyDescriptor.Value, desc.Value))))
					{
						return true;
					}
				}
			}
			if (!propertyDescriptor.Configurable.HasValue || !propertyDescriptor.Configurable.Value)
			{
				if (desc.Configurable.HasValue && desc.Configurable.Value)
				{
					if (throwOnError)
					{
						throw new JavaScriptException(Engine.TypeError);
					}
					return false;
				}
				if (desc.Enumerable.HasValue && (!propertyDescriptor.Enumerable.HasValue || desc.Enumerable.Value != propertyDescriptor.Enumerable.Value))
				{
					if (throwOnError)
					{
						throw new JavaScriptException(Engine.TypeError);
					}
					return false;
				}
			}
			if (!desc.IsGenericDescriptor())
			{
				if (propertyDescriptor.IsDataDescriptor() != desc.IsDataDescriptor())
				{
					if (!propertyDescriptor.Configurable.HasValue || !propertyDescriptor.Configurable.Value)
					{
						if (throwOnError)
						{
							throw new JavaScriptException(Engine.TypeError);
						}
						return false;
					}
					if (propertyDescriptor.IsDataDescriptor())
					{
						SetOwnProperty(propertyName, propertyDescriptor = new PropertyDescriptor(Undefined.Instance, Undefined.Instance, propertyDescriptor.Enumerable, propertyDescriptor.Configurable));
					}
					else
					{
						SetOwnProperty(propertyName, propertyDescriptor = new PropertyDescriptor(Undefined.Instance, null, propertyDescriptor.Enumerable, propertyDescriptor.Configurable));
					}
				}
				else if (propertyDescriptor.IsDataDescriptor() && desc.IsDataDescriptor())
				{
					if (!propertyDescriptor.Configurable.HasValue || !propertyDescriptor.Configurable.Value)
					{
						if (!propertyDescriptor.Writable.HasValue || (!propertyDescriptor.Writable.Value && desc.Writable.HasValue && desc.Writable.Value))
						{
							if (throwOnError)
							{
								throw new JavaScriptException(Engine.TypeError);
							}
							return false;
						}
						if (!propertyDescriptor.Writable.Value && desc.Value != null && !ExpressionInterpreter.SameValue(desc.Value, propertyDescriptor.Value))
						{
							if (throwOnError)
							{
								throw new JavaScriptException(Engine.TypeError);
							}
							return false;
						}
					}
				}
				else if (propertyDescriptor.IsAccessorDescriptor() && desc.IsAccessorDescriptor() && (!propertyDescriptor.Configurable.HasValue || !propertyDescriptor.Configurable.Value) && ((desc.Set != null && !ExpressionInterpreter.SameValue(desc.Set, (!(propertyDescriptor.Set != null)) ? Undefined.Instance : propertyDescriptor.Set)) || (desc.Get != null && !ExpressionInterpreter.SameValue(desc.Get, (!(propertyDescriptor.Get != null)) ? Undefined.Instance : propertyDescriptor.Get))))
				{
					if (throwOnError)
					{
						throw new JavaScriptException(Engine.TypeError);
					}
					return false;
				}
			}
			if (desc.Value != null)
			{
				propertyDescriptor.Value = desc.Value;
			}
			if (desc.Writable.HasValue)
			{
				propertyDescriptor.Writable = desc.Writable;
			}
			if (desc.Enumerable.HasValue)
			{
				propertyDescriptor.Enumerable = desc.Enumerable;
			}
			if (desc.Configurable.HasValue)
			{
				propertyDescriptor.Configurable = desc.Configurable;
			}
			if (desc.Get != null)
			{
				propertyDescriptor.Get = desc.Get;
			}
			if (desc.Set != null)
			{
				propertyDescriptor.Set = desc.Set;
			}
			return true;
		}

		public void FastAddProperty(string name, JsValue value, bool writable, bool enumerable, bool configurable)
		{
			SetOwnProperty(name, new PropertyDescriptor(value, writable, enumerable, configurable));
		}

		public void FastSetProperty(string name, PropertyDescriptor value)
		{
			SetOwnProperty(name, value);
		}

		protected virtual void EnsureInitialized()
		{
		}

		public override string ToString()
		{
			return TypeConverter.ToString(this);
		}
	}
}
