using System;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;

namespace Jint.Native.String
{
	public class StringInstance : ObjectInstance, IPrimitiveInstance
	{
		Types IPrimitiveInstance.Type
		{
			get
			{
				return Types.String;
			}
		}

		JsValue IPrimitiveInstance.PrimitiveValue
		{
			get
			{
				return PrimitiveValue;
			}
		}

		public override string Class
		{
			get
			{
				return "String";
			}
		}

		public JsValue PrimitiveValue { get; set; }

		public StringInstance(Engine engine)
			: base(engine)
		{
		}

		private static bool IsInt(double d)
		{
			if (d >= -9.223372036854776E+18 && d <= 9.223372036854776E+18)
			{
				long num = (long)d;
				return num >= int.MinValue && num <= int.MaxValue;
			}
			return false;
		}

		public override PropertyDescriptor GetOwnProperty(string propertyName)
		{
			if (propertyName == "Infinity")
			{
				return PropertyDescriptor.Undefined;
			}
			PropertyDescriptor ownProperty = base.GetOwnProperty(propertyName);
			if (ownProperty != PropertyDescriptor.Undefined)
			{
				return ownProperty;
			}
			if (propertyName != System.Math.Abs(TypeConverter.ToInteger(propertyName)).ToString())
			{
				return PropertyDescriptor.Undefined;
			}
			JsValue primitiveValue = PrimitiveValue;
			double num = TypeConverter.ToInteger(propertyName);
			if (!IsInt(num))
			{
				return PropertyDescriptor.Undefined;
			}
			int num2 = (int)num;
			int length = primitiveValue.AsString().Length;
			if (length <= num2 || num2 < 0)
			{
				return PropertyDescriptor.Undefined;
			}
			string value = primitiveValue.AsString()[num2].ToString();
			return new PropertyDescriptor(new JsValue(value), false, true, false);
		}
	}
}
