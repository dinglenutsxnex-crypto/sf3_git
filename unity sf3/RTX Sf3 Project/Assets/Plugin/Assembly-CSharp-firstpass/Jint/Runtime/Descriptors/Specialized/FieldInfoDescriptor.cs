using System;
using System.Globalization;
using System.Reflection;
using Jint.Native;

namespace Jint.Runtime.Descriptors.Specialized
{
	public sealed class FieldInfoDescriptor : PropertyDescriptor
	{
		private readonly Engine _engine;

		private readonly FieldInfo _fieldInfo;

		private readonly object _item;

		public override JsValue Value
		{
			get
			{
				return JsValue.FromObject(_engine, _fieldInfo.GetValue(_item));
			}
			set
			{
				object obj;
				if (_fieldInfo.FieldType == typeof(JsValue))
				{
					obj = value;
				}
				else
				{
					obj = value.ToObject();
					if (obj.GetType() != _fieldInfo.FieldType)
					{
						obj = _engine.ClrTypeConverter.Convert(obj, _fieldInfo.FieldType, CultureInfo.InvariantCulture);
					}
				}
				_fieldInfo.SetValue(_item, obj);
			}
		}

		public FieldInfoDescriptor(Engine engine, FieldInfo fieldInfo, object item)
		{
			_engine = engine;
			_fieldInfo = fieldInfo;
			_item = item;
			base.Writable = !CheckInitOnly(fieldInfo.Attributes, FieldAttributes.InitOnly);
		}

		private bool CheckInitOnly(FieldAttributes attibutes, FieldAttributes attribute)
		{
			return (Convert.ToInt32(attribute) & Convert.ToInt32(attibutes)) != 0;
		}
	}
}
