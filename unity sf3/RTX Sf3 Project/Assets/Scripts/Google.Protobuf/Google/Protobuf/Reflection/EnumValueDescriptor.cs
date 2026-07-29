namespace Google.Protobuf.Reflection
{
	public sealed class EnumValueDescriptor : DescriptorBase
	{
		private readonly EnumDescriptor enumDescriptor;

		private readonly EnumValueDescriptorProto proto;

		internal EnumValueDescriptorProto Proto
		{
			get
			{
				return proto;
			}
		}

		public override string Name
		{
			get
			{
				return proto.Name;
			}
		}

		public int Number
		{
			get
			{
				return Proto.Number;
			}
		}

		public EnumDescriptor EnumDescriptor
		{
			get
			{
				return enumDescriptor;
			}
		}

		internal EnumValueDescriptor(EnumValueDescriptorProto proto, FileDescriptor file, EnumDescriptor parent, int index)
			: base(file, parent.FullName + "." + proto.Name, index)
		{
			this.proto = proto;
			enumDescriptor = parent;
			file.DescriptorPool.AddSymbol(this);
			file.DescriptorPool.AddEnumValueByNumber(this);
		}
	}
}
