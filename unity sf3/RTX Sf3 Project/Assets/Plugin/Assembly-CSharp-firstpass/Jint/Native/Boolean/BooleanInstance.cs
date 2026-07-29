using Jint.Native.Object;
using Jint.Runtime;

namespace Jint.Native.Boolean
{
	public class BooleanInstance : ObjectInstance, IPrimitiveInstance
	{
		Types IPrimitiveInstance.Type
		{
			get
			{
				return Types.Boolean;
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
				return "Boolean";
			}
		}

		public JsValue PrimitiveValue { get; set; }

		public BooleanInstance(Engine engine)
			: base(engine)
		{
		}
	}
}
