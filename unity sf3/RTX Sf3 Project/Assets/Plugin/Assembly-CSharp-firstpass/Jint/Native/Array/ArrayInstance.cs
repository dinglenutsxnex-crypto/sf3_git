using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;

namespace Jint.Native.Array
{
	public class ArrayInstance : ObjectInstance
	{
		private readonly Engine _engine;

		private IDictionary<uint, PropertyDescriptor> _array = new MruPropertyCache2<uint, PropertyDescriptor>();

		private PropertyDescriptor _length;

		public new JsValue this[string index]
		{
			get
			{
				return Get(index);
			}
		}

		public override string Class
		{
			get
			{
				return "Array";
			}
		}

		public virtual JsValue this[int index]
		{
			get
			{
				return Get(index.ToString());
			}
		}

		public uint Length
		{
			get
			{
				return GetLength();
			}
		}

		public ArrayInstance(Engine engine)
			: base(engine)
		{
			_engine = engine;
		}

		public override void Put(string propertyName, JsValue value, bool throwOnError)
		{
			if (!CanPut(propertyName))
			{
				if (throwOnError)
				{
					throw new JavaScriptException(base.Engine.TypeError);
				}
				return;
			}
			PropertyDescriptor ownProperty = GetOwnProperty(propertyName);
			if (ownProperty.IsDataDescriptor())
			{
				PropertyDescriptor desc = new PropertyDescriptor(value, null, null, null);
				DefineOwnProperty(propertyName, desc, throwOnError);
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
				PropertyDescriptor desc2 = new PropertyDescriptor(value, true, true, true);
				DefineOwnProperty(propertyName, desc2, throwOnError);
			}
		}

		public override bool DefineOwnProperty(string propertyName, PropertyDescriptor desc, bool throwOnError)
		{
			PropertyDescriptor ownProperty = GetOwnProperty("length");
			uint num = (uint)TypeConverter.ToNumber(ownProperty.Value);
			if (propertyName == "length")
			{
				if (desc.Value == null)
				{
					return base.DefineOwnProperty("length", desc, throwOnError);
				}
				PropertyDescriptor propertyDescriptor = new PropertyDescriptor(desc);
				uint num2 = TypeConverter.ToUint32(desc.Value);
				if ((double)num2 != TypeConverter.ToNumber(desc.Value))
				{
					throw new JavaScriptException(_engine.RangeError);
				}
				propertyDescriptor.Value = num2;
				if (num2 >= num)
				{
					return base.DefineOwnProperty("length", _length = propertyDescriptor, throwOnError);
				}
				if (!ownProperty.Writable.Value)
				{
					if (throwOnError)
					{
						throw new JavaScriptException(_engine.TypeError);
					}
					return false;
				}
				bool flag;
				if (!propertyDescriptor.Writable.HasValue || propertyDescriptor.Writable.Value)
				{
					flag = true;
				}
				else
				{
					flag = false;
					propertyDescriptor.Writable = true;
				}
				if (!base.DefineOwnProperty("length", _length = propertyDescriptor, throwOnError))
				{
					return false;
				}
				if (_array.Count < num - num2)
				{
					uint[] array = _array.Keys.ToArray();
					uint[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						uint num3 = array2[i];
						uint index;
						if (IsArrayIndex(num3, out index) && index >= num2 && index < num && !Delete(num3.ToString(), false))
						{
							propertyDescriptor.Value = new JsValue(index + 1);
							if (!flag)
							{
								propertyDescriptor.Writable = false;
							}
							base.DefineOwnProperty("length", _length = propertyDescriptor, false);
							if (throwOnError)
							{
								throw new JavaScriptException(_engine.TypeError);
							}
							return false;
						}
					}
				}
				else
				{
					while (num2 < num)
					{
						num--;
						if (!Delete(TypeConverter.ToString(num), false))
						{
							propertyDescriptor.Value = num + 1;
							if (!flag)
							{
								propertyDescriptor.Writable = false;
							}
							base.DefineOwnProperty("length", _length = propertyDescriptor, false);
							if (throwOnError)
							{
								throw new JavaScriptException(_engine.TypeError);
							}
							return false;
						}
					}
				}
				if (!flag)
				{
					DefineOwnProperty("length", new PropertyDescriptor(null, false, null, null), false);
				}
				return true;
			}
			uint index2;
			if (IsArrayIndex(propertyName, out index2))
			{
				if (index2 >= num && !ownProperty.Writable.Value)
				{
					if (throwOnError)
					{
						throw new JavaScriptException(_engine.TypeError);
					}
					return false;
				}
				if (!base.DefineOwnProperty(propertyName, desc, false))
				{
					if (throwOnError)
					{
						throw new JavaScriptException(_engine.TypeError);
					}
					return false;
				}
				if (index2 >= num)
				{
					ownProperty.Value = index2 + 1;
					base.DefineOwnProperty("length", _length = ownProperty, false);
				}
				return true;
			}
			return base.DefineOwnProperty(propertyName, desc, throwOnError);
		}

		private uint GetLength()
		{
			return TypeConverter.ToUint32(_length.Value);
		}

		public override IEnumerable<KeyValuePair<string, PropertyDescriptor>> GetOwnProperties()
		{
			foreach (KeyValuePair<uint, PropertyDescriptor> entry in _array)
			{
				yield return new KeyValuePair<string, PropertyDescriptor>(entry.Key.ToString(), entry.Value);
			}
			foreach (KeyValuePair<string, PropertyDescriptor> item in _003CGetOwnProperties_003E__BaseCallProxy0())
			{
				yield return item;
			}
		}

		public override PropertyDescriptor GetOwnProperty(string propertyName)
		{
			uint index;
			if (IsArrayIndex(propertyName, out index))
			{
				PropertyDescriptor value;
				if (_array.TryGetValue(index, out value))
				{
					return value;
				}
				return PropertyDescriptor.Undefined;
			}
			return base.GetOwnProperty(propertyName);
		}

		protected override void SetOwnProperty(string propertyName, PropertyDescriptor desc)
		{
			uint index;
			if (IsArrayIndex(propertyName, out index))
			{
				_array[index] = desc;
				return;
			}
			if (propertyName == "length")
			{
				_length = desc;
			}
			base.SetOwnProperty(propertyName, desc);
		}

		public override bool HasOwnProperty(string p)
		{
			uint index;
			if (IsArrayIndex(p, out index))
			{
				return index < GetLength() && _array.ContainsKey(index);
			}
			return base.HasOwnProperty(p);
		}

		public override void RemoveOwnProperty(string p)
		{
			uint index;
			if (IsArrayIndex(p, out index))
			{
				_array.Remove(index);
			}
			base.RemoveOwnProperty(p);
		}

		public static bool IsArrayIndex(JsValue p, out uint index)
		{
			index = ParseArrayIndex(TypeConverter.ToString(p));
			return index != uint.MaxValue;
		}

		internal static uint ParseArrayIndex(string p)
		{
			int num = p[0] - 48;
			switch (num)
			{
			default:
				return uint.MaxValue;
			case 0:
				if (p.Length > 1)
				{
					return uint.MaxValue;
				}
				break;
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
			case 9:
				break;
			}
			ulong num2 = (uint)num;
			for (int i = 1; i < p.Length; i++)
			{
				num = p[i] - 48;
				if (num < 0 || num > 9)
				{
					return uint.MaxValue;
				}
				num2 = num2 * 10 + (uint)num;
				if (num2 >= uint.MaxValue)
				{
					return uint.MaxValue;
				}
			}
			return (uint)num2;
		}

		[CompilerGenerated]
		[DebuggerHidden]
		private IEnumerable<KeyValuePair<string, PropertyDescriptor>> _003CGetOwnProperties_003E__BaseCallProxy0()
		{
			return base.GetOwnProperties();
		}
	}
}
