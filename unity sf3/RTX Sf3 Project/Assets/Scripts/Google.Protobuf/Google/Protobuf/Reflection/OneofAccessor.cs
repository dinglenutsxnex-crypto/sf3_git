using System;
using System.Reflection;

namespace Google.Protobuf.Reflection
{
	public sealed class OneofAccessor
	{
		private readonly Func<IMessage, int> caseDelegate;

		private readonly Action<IMessage> clearDelegate;

		private OneofDescriptor descriptor;

		public OneofDescriptor Descriptor
		{
			get
			{
				return descriptor;
			}
		}

		internal OneofAccessor(PropertyInfo caseProperty, MethodInfo clearMethod, OneofDescriptor descriptor)
		{
			if (!caseProperty.CanRead)
			{
				throw new ArgumentException("Cannot read from property");
			}
			this.descriptor = descriptor;
			caseDelegate = ReflectionUtil.CreateFuncIMessageT<int>(caseProperty.GetGetMethod());
			this.descriptor = descriptor;
			clearDelegate = ReflectionUtil.CreateActionIMessage(clearMethod);
		}

		public void Clear(IMessage message)
		{
			clearDelegate(message);
		}

		public FieldDescriptor GetCaseFieldDescriptor(IMessage message)
		{
			int num = caseDelegate(message);
			if (num > 0)
			{
				return descriptor.ContainingType.FindFieldByNumber(num);
			}
			return null;
		}
	}
}
